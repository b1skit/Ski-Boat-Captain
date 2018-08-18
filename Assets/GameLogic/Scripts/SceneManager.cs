using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


// A container for score data. Used to save/load a collection of ScoreElement objects
[Serializable]
public class ScoreData
{
    public ScoreElement[] theScores;

    // Constructor
    public ScoreData(ScoreElement[] allScores)
    {
        theScores = allScores;
    }
}


// An individual score element. NOTE: We must make this serializable so that it displays in the inspector
[Serializable] 
public struct ScoreElement : IComparable
{
    public string name;
    public float time; // In seconds
    public int points;

    public ScoreElement(string newName, float newTime, int newPoints)
    {
        this.name = newName;
        this.time = newTime;
        this.points = newPoints;
    }

    // Decides how the scores are ranked by balancing points and timme
    public float CalculateScoreRank()
    {
        return (float)this.points - (this.time * GameManager.Instance.timeScoreFactor);
    }

    // Compares this object with the received object:
    //      If this.score > otherScore, return 1
    //      If this.score == otherScore, return 0
    //      If this.score < otherScore, return -1
    public int CompareTo(object obj)
    {
        if (obj == null)
            return 1;

        ScoreElement otherScore = (ScoreElement)obj;

        return (int)Mathf.Sign(otherScore.CalculateScoreRank() - this.CalculateScoreRank()); // Rank is calculated as: points - (time in seconds)
    }
}


public class SceneManager : MonoBehaviour {

    [Header("Level settings:")]

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

    [Header("Additional UI Panels:")]
    public GameObject tutorialElement;

    [Tooltip("The level failed UI element")]
    public Text levelFailedText;

    [Space(10)]

    [Header("Dynamic UI Elements:")]
    [Tooltip("The popup throttle text element")]
    public GameObject throttleUIPopupText;

    [Tooltip("Apply an offset to the user's finger position so the popup is readable")]
    public Vector2 throttleTextOffset = new Vector2(-100.0f, 200.0f);

    [Tooltip("How long should the throttle be displayed after input has stopped?")]
    [Range(0, 5.0f)]
    public float throttleUIPopupLifetime = 1.0f;

    [Space(10)]

    [Tooltip("The start countdown timer text element")]
    public GameObject countdownText;

    [Tooltip("Countdown timer font size increase rate (Scales deltaTime)")]
    public int countdownFontGrowthAmount = 1000;

    [Tooltip("How many seconds to wait before commencing the race start countdown")]
    public float countdownStartDelay = 3.0f;

    [Space(10)]

    [Tooltip("The blip sound to play for 3-2-1")]
    public AudioSource startTimerBlip;

    [Tooltip("The blip sound to play for \"GO!\"")]
    public AudioSource goBlip;

    [Space(10)]

    [Tooltip("The level complete popup UI text object")]
    public GameObject levelCompleteText;

    [Tooltip("The number of seconds to display the level complete message")]
    public float levelCompletePopupTime = 2.0f;

    private GameObject countdownTextPopup;
    private Text countdownTextComponent;

    private float startTimeOffset;
    
    private GameObject throttlePopup;
    private float previousNormalizedThrottleValue;

    public bool IsPlaying { get; private set; }
    public bool isWarmingUp { get; private set; }

    public static SceneManager Instance = null;

    private PauseScreenController thePauseScreenController;
    private EndLevelMenuController theEndLevelMenuController;

    [Header("Default scoreboard entries:")]

    [Tooltip("The default score data")]
    public ScoreElement[] playerScores = new ScoreElement[5];

    private int currentLevelScore;
    private int currentPlayerScoresEntryIndex; // -1 indicates the player's score is not held in the scoreboard array


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
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

        currentLevelScore = 0;
        currentPlayerScoresEntryIndex = -1;

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

        EndLevelMenuController[] endLevelMenuControllers = Resources.FindObjectsOfTypeAll<EndLevelMenuController>();
        foreach (EndLevelMenuController current in endLevelMenuControllers)
        {
            if (current.gameObject.scene == UnityEngine.SceneManagement.SceneManager.GetActiveScene())
            {
                theEndLevelMenuController = current;
            }
        }

        // Ensure the level failed UI element is hidden:
        levelFailedText.gameObject.SetActive(false);

        // Display the tutorial popup on mobile only:
        #if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
        tutorialElement.gameObject.SetActive(true);
        #endif

        // Display the level start countdown:
        countdownTextPopup = null;
        StartCoroutine("UpdateCountdownText");

        LoadScores(); // Load scores now, to avoid a hitch at the end of the race...

        isWarmingUp = true;
    }


    IEnumerator UpdateCountdownText()
    {
        yield return new WaitForSeconds(countdownStartDelay); // Wait before startinng the countdown

        startTimerBlip.pitch = 1.0f;
        startTimerBlip.Play(); // Play the FIRST blip

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

                if (countdownValue == 0)
                {
                    goBlip.Play();
                }
                else
                {
                    startTimerBlip.Play(); // Play the REMAINING countdown blips
                }
                
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
            timerText.text = SecondsToFormattedTimeString(timeVal);
        }
        #if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
        else if (throttlePopup && !isWarmingUp)
        {
            Destroy(throttlePopup);
        }
        #endif


        #if UNITY_STANDALONE || UNITY_WEBPLAYER // Handle Unity editor/standalone build
        if (Input.GetKeyDown("escape"))
        {
            thePauseScreenController.DoPause();
        }
        #endif
    }


    // Utility function: Converts a number of secconds to a formatted time string mm:ss:msms (eg. 12:34:56)
    public string SecondsToFormattedTimeString(float seconds)
    {
        int ms = (int)((seconds % 1) * 100);
        string msStr = ms.ToString();
        if (msStr.Length < 2)
            msStr = "0" + msStr;

        seconds = (int)seconds;

        int sec = (int)(seconds % 60);
        string secStr = sec.ToString();
        if (secStr.Length < 2)
            secStr = "0" + secStr;

        seconds /= 60;

        int min = (int)(seconds % 60);
        string minStr = min.ToString();
        if (minStr.Length < 2)
            minStr = "0" + minStr;

        return minStr + ":" + secStr + ":" + msStr;
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

            if (isNewTouch || isWarmingUp) // Allow the throttle popup to follow the user's finger during warmup (only)
            {
                throttleTouchPosition = newTouchPosition;

                throttleTouchPosition += throttleTextOffset;
            }

            throttlePopup = Instantiate<GameObject>(throttleUIPopupText, mainCanvas.transform);           

            Vector2 hoverPoint = new Vector2();

            RectTransformUtility.ScreenPointToLocalPointInRectangle(mainCanvasRectTransform, throttleTouchPosition, mainCanvas.worldCamera, out hoverPoint);

            throttlePopup.GetComponent<RectTransform>().anchoredPosition = hoverPoint;

            throttlePopup.transform.SetParent(mainCanvas.transform, false); // Is this needed???

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

        IsPlaying = true;
        isWarmingUp = false;

        // Display the tutorial popup on mobile only:
        #if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
        tutorialElement.gameObject.SetActive(false);
        #endif
    }


    public void EndLevel()
    {
        IsPlaying = false;

        GameObject levelCompletePopup = Instantiate<GameObject>(levelCompleteText);
        levelCompletePopup.transform.SetParent(mainCanvas.transform, false);

        Destroy(levelCompletePopup, levelCompletePopupTime);

        Invoke("DisplayEndLevelUI", levelCompletePopupTime + 1);
    }


    private void UpdatePlayerScoresArray()
    {
        Array.Sort(playerScores); // Even though the aray *should* be sorted, we sort here incase it was initialized out-of-order via the property inspector

        ScoreElement newScore = new ScoreElement(PlayerPrefs.GetString("PlayerName", GameManager.Instance.defaultPlayerName), Time.timeSinceLevelLoad - startTimeOffset, currentLevelScore);
        float newScoreRank = newScore.CalculateScoreRank();
        for (int i = 0; i < playerScores.Length; i++)
        {
            bool hasInserted = false;
            float currentScoreRank = playerScores[i].CalculateScoreRank();

            if (newScoreRank > currentScoreRank)
            {
                hasInserted = true;
                currentPlayerScoresEntryIndex = i;

                for (int j = i; j < playerScores.Length; j++)
                {
                    ScoreElement temp = playerScores[j];
                    playerScores[j] = newScore;
                    newScore = temp;
                }
            }

            if (hasInserted) // If we've inserted the new element, we're done
                break;
        } // End score insertion loop

        SaveScores();
    }
   

    // Note: This causes a minor hitch on mobile
    public void LoadScores()
    {
        if (File.Exists(Application.persistentDataPath + "/level" + UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex.ToString() + ".dat"))
        {
            BinaryFormatter theBinaryFormatter = new BinaryFormatter();
            FileStream theFileStream = File.Open(Application.persistentDataPath + "/level" + UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex.ToString() + ".dat", FileMode.Open);
            ScoreData loadedScores = (ScoreData)theBinaryFormatter.Deserialize(theFileStream);
            theFileStream.Close();

            this.playerScores = loadedScores.theScores;
        }
    }


    // Note: This causes a minor hitch on mobile
    public void SaveScores()
    {
        ScoreData theScores = new ScoreData(playerScores);

        BinaryFormatter theBinaryFormatter = new BinaryFormatter();
        FileStream theFileStream = File.Create(Application.persistentDataPath + "/level" + UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex.ToString() + ".dat");
        theBinaryFormatter.Serialize(theFileStream, theScores);
        theFileStream.Close();
    }


    public void DisplayEndLevelUI()
    {
        UpdatePlayerScoresArray(); // Prepare the scoreboard data here, once the race is over (causes a minor hitch on mobile)

        theEndLevelMenuController.DoDisplayEndLevelMenu(currentPlayerScoresEntryIndex);
        currentPlayerScoresEntryIndex = -1;
    }


    public void FailLevel()
    {
        Destroy(throttlePopup);

        IsPlaying = false;

        timerText.text = "--:--:--";
        levelFailedText.gameObject.SetActive(true);

        GameManager.Instance.RestartLevel();
    }


    public void UpdateLapText(int lapsRemaining)
    {
        lapText.text = (numberOfLaps - lapsRemaining).ToString() + "/" + numberOfLaps.ToString();
    }


    public void AddPoints(int newPoints)
    {
        currentLevelScore += newPoints;
        scoreText.text = PointsToFormattedString(currentLevelScore);
    }


    // Note: Max score string length = "999,999,999". Will wrap arount to "000,000,000", but actual score value is maintained... Shouldn't be a problem.
    public string PointsToFormattedString(int newPoints)
    {
        string scoreStr = newPoints.ToString();
        scoreStr = "00000000" + scoreStr;
        scoreStr = scoreStr.Substring(scoreStr.Length - 9, 3) + "," + scoreStr.Substring(scoreStr.Length - 6, 3) + "," + scoreStr.Substring(scoreStr.Length - 3, 3);

        return scoreStr;
    }
}
