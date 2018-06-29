using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneManager : MonoBehaviour {

    [Header("Level settings:")]
    //[Tooltip("How long should the game wait before restarting after the player has failed? (Seconds)")]
    //public float failRestartTime = 3.0f;

    [Tooltip("How many laps are required to complete this level")]
    public int numberOfLaps = 3;

    [Space(10)]

    [Header("Core UI Elements:")]
    public Canvas mainCanvas;
    private RectTransform mainCanvasRectTransform;

#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
    private Vector2 throttleTouchPosition; // Is used for Android/mobile builds ONLY
#endif

    [Tooltip("The canvas's timer text element")]
    public Text timerText;

    [Tooltip("The canvas's score text element")]
    public Text scoreText;

    [Tooltip("The canvas's permanent throttle text element")]
    public Text throttleText;

    [Tooltip("The canvas's lap text element")]
    public Text lapText;

    [Space(10)]

    [Header("Dynamic UI Elements:")]
    [Tooltip("The popup throttle text element")]
    public GameObject throttleUIPopupText;

    [Tooltip("Apply an offset to the user's finger position so the popup is readable")]
    public float throttleTextOffsetX = -100.0f;

    [Tooltip("Apply an offset to the user's finger position so the popup is readable")]
    public float throttleTextOffsetY = 200.0f;

    [Tooltip("How long should the throttle be displayed after input has stopped?")]
    [Range(0, 5.0f)]
    public float throttleUIPopupLifetime = 1.0f;

    [Tooltip("The level failed text element")]
    public GameObject levelFailedText;

    [Space(10)]

    [Tooltip("The start countdown timer text element")]
    public GameObject countdownText;

    [Tooltip("Countdown timer font size increase rate (Scales deltaTime)")]
    public int countdownFontGrowthAmount = 1000;

    [Tooltip("How many seconds to wait before commencing the race start countdown")]
    public float countdownStartDelay = 3.0f;

    private GameObject countdownTextPopup;
    private Text countdownTextComponent;

    private float startTimeOffset;
    
    private GameObject throttlePopup;
    private float previousNormalizedThrottleValue;

    public bool IsPlaying { get; set; } // TO DO: Figure out why I can't have a getter ONLY???? C# 6+...  
    //public bool IsPlaying
    //{
    //    get
    //    {
    //        return IsPlaying;
    //    }
    //}
    
    //public bool isTiming;

    public static SceneManager instance = null;

    private PauseScreenController thePauseScreenController;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this.gameObject);

        GameManager.Instance.SetLevelNumber(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex); // We set the level number here, which allows us to load in from any level without issues
    }

    // Use this for initialization
    void Start () {

        timerText.text = "00:00:00";
        scoreText.text = "000,000,000";
        throttleText.text = "0%";
        lapText.text = "1/" + numberOfLaps.ToString();

        startTimeOffset = 0.0f;

        //isTiming = false;

        throttlePopup = null;

        mainCanvasRectTransform = mainCanvas.GetComponent<RectTransform>();

        PauseScreenController[] pauseControllers = Resources.FindObjectsOfTypeAll<PauseScreenController>();
        foreach (PauseScreenController current in pauseControllers)
        {
            if (current.gameObject.scene == UnityEngine.SceneManagement.SceneManager.GetActiveScene())
            {
                thePauseScreenController = current;
            }
        }

        // Level start countdown:
        countdownTextPopup = null;
        
        StartCoroutine("UpdateCountdownText");
    }

    IEnumerator UpdateCountdownText()
    {
        yield return new WaitForSeconds(countdownStartDelay); // Wait before startinng the countdown

        float timeRemaining = 3.0f;
        int countdownValue = 3;
        
        countdownTextPopup = Instantiate<GameObject>(countdownText, mainCanvas.transform);
        countdownTextComponent = countdownTextPopup.GetComponent<Text>();

        int fontStartSize = countdownTextComponent.fontSize;
        
        //Color original = countdownTextComponent.color;

        while (timeRemaining > 0)
        {
            countdownTextComponent.text = countdownValue.ToString();
            countdownTextComponent.fontSize += Mathf.RoundToInt(countdownFontGrowthAmount * Time.deltaTime);

            //countdownTextComponent.color = Color.Lerp(countdownTextComponent.color, Color.clear, Time.deltaTime);

            timeRemaining -= Time.deltaTime;

            if (timeRemaining < countdownValue - 1)
            {
                countdownValue--;
                countdownTextComponent.fontSize = fontStartSize;
                //countdownTextComponent.color = original;
            }

            yield return null;
        }

        countdownTextComponent.text = "GO!";
        countdownTextComponent.fontSize = fontStartSize;
        //countdownTextComponent.color = original;

        timeRemaining = 1.0f;
        while (timeRemaining > 0)
        {
            countdownTextComponent.fontSize += Mathf.RoundToInt(countdownFontGrowthAmount * Time.deltaTime);

            //countdownTextComponent.color = Color.Lerp(countdownTextComponent.color, Color.clear, 1.0f * Time.deltaTime);

            timeRemaining -= Time.deltaTime;

            yield return null;
        }

        Destroy(countdownTextPopup);
        

        // Cleanup:
        Destroy(countdownTextComponent, 1.0f); // Display the final "GO!" message for 1 second

        StartLevel();
    }
	
	// Update is called once per frame
	void Update () {
        
        if (IsPlaying)
        {
            // Update the timer text:
            float timeVal = Time.timeSinceLevelLoad - startTimeOffset;

            int ms = (int)((timeVal % 1) * 100);
            string msStr = ms.ToString();
            if (msStr.Length < 2)
                msStr = "0" + msStr;

            timeVal = (int)timeVal;

            int sec = (int)(timeVal % 60);
            string secStr = sec.ToString();
            if (secStr.Length < 2)
                secStr = "0" + secStr;

            timeVal /= 60;

            int min = (int)(timeVal % 60);
            string minStr = min.ToString();
            if (minStr.Length < 2)
                minStr = "0" + minStr;

            timerText.text = minStr + ":" + secStr + ":" + msStr;
        }

#if UNITY_STANDALONE || UNITY_WEBPLAYER // Handle Unity editor/standalone build
        if (Input.GetKeyDown("escape"))
        {
            thePauseScreenController.DoPause();
        }
#endif
    }

    public void UpdateThrottleValue(float normalizedThrottleValue, Vector2 newTouchPosition, bool isNewTouch = false)
    {
        if (normalizedThrottleValue >= 0 && previousNormalizedThrottleValue != normalizedThrottleValue && Time.timeScale > 0.0f)
        {
            previousNormalizedThrottleValue = normalizedThrottleValue;

            // Handle permanent UI throttle:
            normalizedThrottleValue *= 100;
            normalizedThrottleValue = Mathf.Round(normalizedThrottleValue);
            throttleText.text = normalizedThrottleValue.ToString() + "%";

#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE // Handle iOS/Android/Windows Phone 8/Unity iPhone
            // Handle popup UI throtte:
            if (throttlePopup)
            {
                Destroy(throttlePopup);
            }

            if (isNewTouch)
            {
                throttleTouchPosition = newTouchPosition;

                throttleTouchPosition.x += throttleTextOffsetX;
                throttleTouchPosition.y += throttleTextOffsetY;
            }

            throttlePopup = Instantiate<GameObject>(throttleUIPopupText, mainCanvas.transform);           

            Vector2 hoverPoint = new Vector2();

            RectTransformUtility.ScreenPointToLocalPointInRectangle(mainCanvasRectTransform, throttleTouchPosition, mainCanvas.worldCamera, out hoverPoint);

            throttlePopup.GetComponent<RectTransform>().anchoredPosition = hoverPoint;

            throttlePopup.transform.SetParent(mainCanvas.transform, false); // Is this even needed??????????????

            throttlePopup.GetComponent<Text>().text = throttleText.text;          

            Destroy(throttlePopup, throttleUIPopupLifetime);
            
#endif
        }

        // Destroy the touchscreen popup immediately if the game has been paused
#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
        if (Time.timeScale == 0.0f && throttlePopup)
            Destroy(throttlePopup);
#endif

    }

    public void UpdateThrottleValue(float normalizedThrottleValue)
    {
        UpdateThrottleValue(normalizedThrottleValue, new Vector2(0.0f, 0.0f));
    }

    // Handles everything that needs to happen when the countdown has finished and the level starts
    public void StartLevel()
    {
        timerText.text = "00:00:00";

        startTimeOffset = Time.timeSinceLevelLoad;

        this.IsPlaying = true;
    }

    public void EndLevel()
    {
        IsPlaying = false;

        GameManager.Instance.LoadNextLevel();
    }

    public void FailLevel()
    {
        Destroy(throttlePopup);

        IsPlaying = false;

        timerText.text = "--:--:--";

        GameObject levelFailedPopup = Instantiate<GameObject>(levelFailedText);
        levelFailedPopup.transform.SetParent(mainCanvas.transform, false);

        // Do do: Use a switch to select different failure messages?

        GameManager.Instance.RestartLevel();
    }

    public void UpdateLapText(int lapsRemaining)
    {
        lapText.text = (numberOfLaps - lapsRemaining).ToString() + "/" + numberOfLaps.ToString();
    }
}
