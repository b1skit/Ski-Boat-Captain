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

        // Implement a name string length check/handling

        for (int i = 0; i < scoreBoardTextElements.Length; i++)
        {
            scoreBoardTextElements[i].nameText.text = SceneManager.instance.playerScores[i].name;
            scoreBoardTextElements[i].timeText.text = SceneManager.instance.SecondsToFormattedTimeString(SceneManager.instance.playerScores[i].time);
            scoreBoardTextElements[i].pointsText.text = SceneManager.instance.PointsToFormattedString(SceneManager.instance.playerScores[i].points);
        }

        if (currentPlayerScoresEntryIndex >= 0 && currentPlayerScoresEntryIndex < SceneManager.instance.playerScores.Length)
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
