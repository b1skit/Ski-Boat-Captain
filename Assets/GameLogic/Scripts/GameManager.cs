﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    [Header("Global settings:")]
    [Tooltip("How long should the game wait before loading the next level? (Seconds)")]
    public float nextLevelLoadTime = 5.0f;

    
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

    private int score;
    private int level;


    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    // Use this for initialization
    void Start () {
        score = 0;
        level = 1;
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
