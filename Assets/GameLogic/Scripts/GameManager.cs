using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    [Header("Global settings:")]
    [Tooltip("How long should the game wait before loading the next level? (Seconds)")]
    public float nextLevelLoadTime = 5.0f;

    public static GameManager instance = null;

    private int score;
    private int level;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this.gameObject);

        DontDestroyOnLoad(this.gameObject); // Temporarily disabling this until I complete the full game loop flow
    }

    // Use this for initialization
    void Start () {
        score = 0;
        level = 1;

        // DEV HACK: Check if we've launched a level directly (ie. via the editor), and only load a level if we haven't
        SceneManager checkforSceneManager = GameObject.FindObjectOfType<SceneManager>();
        if (checkforSceneManager == null)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Level001"); // Temp hack
        }
        else
            Debug.Log("DEBUG: Detected level launched directly via editor! Skipping level load...");

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void AddPoints(int newPoints)
    {
        // Note: Max score string length = "999,999,999". Will wrap arount to "000,000,000", but actual score value is maintained... Shouldn't be a problem.
        score += newPoints;

        string scoreStr = score.ToString();
        scoreStr = "00000000" + scoreStr;
        scoreStr = scoreStr.Substring(scoreStr.Length - 9, 3) + "," + scoreStr.Substring(scoreStr.Length - 6, 3) + "," + scoreStr.Substring(scoreStr.Length - 3, 3);

        SceneManager.instance.scoreText.text = scoreStr;
    }

    public void LoadNextLevel()
    {
        Invoke("DoLoadNextLevel", nextLevelLoadTime);
    }

    private void DoLoadNextLevel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name); // TEMP HACK
    }
}
