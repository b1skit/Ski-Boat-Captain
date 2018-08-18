using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    [Header("Global settings:")]

    [Tooltip("Factor to multipy the player's time by when calculating scoreboard rank weighting")]
    public float timeScoreFactor = 10.0f;

    [Tooltip("The default name to use if no custom player name has been set")]
    public string defaultPlayerName = "Player";

    [Tooltip("How long should the game wait before loading the next level? (Seconds)")]
    public float nextLevelLoadTime = 5.0f;

    [Tooltip("How long should the game wait before restarting the current level after the player has failed? (Seconds)")]
    public float failRestartTime = 3.0f;

    [Header("Ignore these values:")]
    [Tooltip("These values are overwritten. Ignore them")]
    public bool enableMusic;

    [Tooltip("These values are overwritten. Ignore them")]
    public bool invertSteering;

    private int level;
   

    private static GameManager _instance = null;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
                if (_instance == null)
                {
                    GameObject theGameObject = new GameObject("theGameManager");
                    _instance = theGameObject.AddComponent<GameManager>();
                }
            }
            return _instance;
        }
    }


    private void Awake()
    {
        GameManager existingGameManager = FindObjectOfType<GameManager>();
        if (existingGameManager != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            DontDestroyOnLoad(this.gameObject);
        }

        // Note: We must set values loaded from player prefs here, as sometimes it is too late to set them via Start()
        invertSteering = PlayerPrefs.GetInt("invertSteering", 0) != 0; // Evaluate our ints to convert to a bool
        enableMusic = PlayerPrefs.GetInt("enableMusic", 0) != 0;
    }


    // Use this for initialization
    private void Start ()
    {
        level = 1;
    }


    public void RestartLevel()
    {
        Invoke("DoRestartLevel", failRestartTime);
    }


    private void DoRestartLevel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }


    public void LoadNextLevel()
    {
        Invoke("DoLoadNextLevel", nextLevelLoadTime);
    }


    private void DoLoadNextLevel()
    {
        if (level < UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings - 1)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(level + 1);
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
            
    }


    public void LoadSpecificLevel(int level)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(level);
    }


    public void SetLevelNumber(int newLevelBuildIndex)
    {
        level = newLevelBuildIndex;
    }
}
