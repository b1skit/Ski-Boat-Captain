using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneManager : MonoBehaviour {

    [Header("Level settings:")]
    [Tooltip("How long should the game wait before restarting after the player has failed? (Seconds)")]
    public float failRestartTime = 3.0f;

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
    public bool isTiming;

    public static SceneManager instance = null;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this.gameObject);
    }

    // Use this for initialization
    void Start () {

        timerText.text = "00:00:00";
        scoreText.text = "000,000,000";
        throttleText.text = "0%";
        lapText.text = "1/" + numberOfLaps.ToString();

        startTimeOffset = 0.0f;
        isTiming = false;

        this.IsPlaying = true;

        throttlePopup = null;

        mainCanvasRectTransform = mainCanvas.GetComponent<RectTransform>();
    }
	
	// Update is called once per frame
	void Update () {
        if (isTiming)
        {
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
            else
                timerText.text = "--:--:--";
        }
	}

    public void UpdateThrottleValue(float normalizedThrottleValue, Vector2 newTouchPosition, bool isNewTouch = false)
    {
        if (normalizedThrottleValue >= 0 && previousNormalizedThrottleValue != normalizedThrottleValue)
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
    }

    public void UpdateThrottleValue(float normalizedThrottleValue)
    {
        UpdateThrottleValue(normalizedThrottleValue, new Vector2(0.0f, 0.0f));
    }


    public void StartLevel()
    {
        startTimeOffset = Time.timeSinceLevelLoad;
        isTiming = true;
    }

    public void EndLevel()
    {
        isTiming = false;
        IsPlaying = false;

        GameManager.Instance.LoadNextLevel();
    }

    public void FailLevel()
    {
        Destroy(throttlePopup);

        IsPlaying = false;

        GameObject levelFailedPopup = Instantiate<GameObject>(levelFailedText);
        levelFailedPopup.transform.SetParent(mainCanvas.transform, false);


        // Do do: Include a switch to select between different failure messages?


        Invoke("RestartLevel", failRestartTime);
    }

    public void RestartLevel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public void UpdateLapText(int lapsRemaining)
    {
        lapText.text = (numberOfLaps - lapsRemaining).ToString() + "/" + numberOfLaps.ToString();
    }
}
