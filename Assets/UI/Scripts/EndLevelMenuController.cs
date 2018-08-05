using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[Serializable]
public struct ScoreboardTextElements
{
    public Image backgroundBar;
    public Text nameText;
    public Text timeText;
    public Text pointsText;
}

// Inherits from the CommonUIController defined in PauseScreenController.cs
public class EndLevelMenuController : CommonUIController {

    [Tooltip("The of the background bar to set for a new scoreboard entry")]
    public Color newScoreboardEntryColor = new Color();

    [Header("Scoreboard text elements:")]
    public ScoreboardTextElements[] scoreBoardTextElements = new ScoreboardTextElements[5];

    public void DoDisplayEndLevelMenu(int currentPlayerScoresEntryIndex)
    {
        #if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE // Toggle the touch screen on-screen pause button visibility
        TouchScreenPauseButton.SetActive(false);
        #endif

        this.gameObject.SetActive(true);
        this.TouchScreenPauseButton.SetActive(false);

        

        for (int i = 0; i < scoreBoardTextElements.Length; i++)
        {
            scoreBoardTextElements[i].nameText.text = SceneManager.Instance.playerScores[i].name;
            scoreBoardTextElements[i].timeText.text = SceneManager.Instance.SecondsToFormattedTimeString(SceneManager.Instance.playerScores[i].time);
            scoreBoardTextElements[i].pointsText.text = SceneManager.Instance.PointsToFormattedString(SceneManager.Instance.playerScores[i].points);
        }

        if (currentPlayerScoresEntryIndex >= 0 && currentPlayerScoresEntryIndex < SceneManager.Instance.playerScores.Length)
        {
            scoreBoardTextElements[currentPlayerScoresEntryIndex].backgroundBar.color = newScoreboardEntryColor;
        }
    }

    public void DoLoadNextLevel()
    {
        #if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE // Toggle the touch screen on-screen pause button visibility
        TouchScreenPauseButton.SetActive(true);
        #endif

        this.gameObject.SetActive(false);
        LoadingScreenUIPanel.SetActive(true);

        menuButtonPress.Play();

        GameManager.Instance.LoadNextLevel();
    }
}
