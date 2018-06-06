using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    [Header("Game settings:")]
    [Tooltip("How long should the game wait before restarting after the player has failed? (Seconds)")]
    public float failRestartTime = 3.0f;

    [Tooltip("How long should the game wait to load the next level after the player has won? (Seconds)")]
    public float nextLevelLoadTime = 5.0f;

    [Space(10)]

    [Header("Core UI Elements:")]
    public Canvas MainCanvas;

    [Tooltip("The canvas's timer text element")]
    public Text timerText;

    [Tooltip("The canvas's score text element")]
    public Text scoreText;

    [Tooltip("The canvas's permanent throttle text element")]
    public Text throttleText;

    [Space(10)]

    [Header("Dynamic UI Elements:")]
    [Tooltip("The popup throttle text element")]
    public GameObject ThrottleUIPopupText;
    [Tooltip("How long should the throttle be displayed after input has stopped?")]
    [Range(0, 5.0f)]
    public float ThrottleUIPopupLifetime = 1.0f;

    [Tooltip("The level failed text element")]
    public GameObject LevelFailedText;

    public bool IsPlaying { get; set; } // TO DO: Figure out why I can't have a getter ONLY???? C# 6+...  

    private int score;
    private float startTimeOffset;
    private bool isTiming;
    private GameObject throttlePopup;
    private float previousNormalizedThrottleValue;

    public static GameManager instance = null;
    

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this.gameObject);

        //DontDestroyOnLoad(this.gameObject); // Temporarily disabling this until I complete the full game loop flow
    }

    // Use this for initialization
    void Start () {
        score = 0;

        timerText.text = "00:00:00";
        scoreText.text = "000,000,000";
        throttleText.text = "0%";

        startTimeOffset = 0.0f;
        isTiming = false;

        IsPlaying = true;

        throttlePopup = null;

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

    public void UpdateThrottleValue(float normalizedThrottleValue, Vector2 touchPosition)
    {
        if (normalizedThrottleValue >= 0 && previousNormalizedThrottleValue != normalizedThrottleValue)
        {
            previousNormalizedThrottleValue = normalizedThrottleValue;

            // Handle permanent UI throttle:
            normalizedThrottleValue *= 100;
            normalizedThrottleValue = Mathf.Round(normalizedThrottleValue);
            throttleText.text = normalizedThrottleValue.ToString() + "%";

            // Handle popup UI throtte:

            if (throttlePopup)
                Destroy(throttlePopup);
            //Transform modifiedTransform = MainCanvas.transform;
            //modifiedTransform.SetPositionAndRotation(modifiedTransform.position + new Vector3(touchPosition.x, touchPosition.y, 0.0f), modifiedTransform.rotation);
            //throttlePopup = Instantiate<GameObject>(ThrottleUIPopupText, modifiedTransform);

            Vector3 touchPositionAsVec3 = new Vector3(touchPosition.x, touchPosition.y, 0.0f);
            //modifiedTransform.localPosition = touchPositionAsVec3;
            

            throttlePopup = Instantiate<GameObject>(ThrottleUIPopupText, MainCanvas.transform);

            RectTransform popupTransform = throttlePopup.GetComponent<RectTransform>();
            //popupTransform.position = touchPositionAsVec3;
            //popupTransform.SetPositionAndRotation(touchPositionAsVec3, MainCanvas.transform.rotation);

            //Debug.Log("Main canvas: " + MainCanvas.transform.position.ToString());
            //Debug.Log("Touch position: " + touchPositionAsVec3.ToString());

            throttlePopup.transform.SetParent(MainCanvas.transform, false);
            throttlePopup.GetComponent<Text>().text = throttleText.text;

            

            Destroy(throttlePopup, ThrottleUIPopupLifetime);

        }
    }

    public void UpdateThrottleValue(float normalizedThrottleValue)
    {
        UpdateThrottleValue(normalizedThrottleValue, new Vector2(0.0f, 0.0f));
    }

    public void AddPoints(int newPoints)
    {
        // Note: Max score string length = "999,999,999". Will wrap arount to "000,000,000", but actual score value is maintained... Shouldn't be a problem.
        score += newPoints;

        string scoreStr = score.ToString();
        scoreStr = "00000000" + scoreStr;
        scoreStr = scoreStr.Substring(scoreStr.Length - 9, 3) + "," + scoreStr.Substring(scoreStr.Length - 6, 3) + "," + scoreStr.Substring(scoreStr.Length - 3, 3);
        
        scoreText.text = scoreStr;
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

        Invoke("RestartLevel", nextLevelLoadTime); // TEMP HACK
    }
    
    public void FailLevel()
    {
        Destroy(throttlePopup);

        IsPlaying = false;

        GameObject levelFailedPopup = Instantiate<GameObject>(LevelFailedText);
        levelFailedPopup.transform.SetParent(MainCanvas.transform, false);


        // Do do: Include a switch to select between different failure messages?


        Invoke("RestartLevel", failRestartTime);
    }
    
    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
