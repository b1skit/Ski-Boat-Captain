using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Inherits from the CommonUIController defined in PauseScreenController.cs
public class EndLevelMenuController : CommonUIController {

    [Header("Scoreboard text elements:")]
    public Text score0Name;
    public Text score0Time;
    public Text score0Points;

    public Text score1Name;
    public Text score1Time;
    public Text score1Points;

    public Text score2Name;
    public Text score2Time;
    public Text score2Points;

    public Text score3Name;
    public Text score3Time;
    public Text score3Points;

    public Text score4Name;
    public Text score4Time;
    public Text score4Points;



    public void DoDisplayEndLevelMenu()
    {
        #if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE // Toggle the touch screen on-screen pause button visibility
        TouchScreenPauseButton.SetActive(false);
        #endif

        //this.GameHUD.SetActive(false);
        this.gameObject.SetActive(true);

        // Need to calculate the user's score, display it on the screen somewhere, and then insert it into the scoreboard if it's a new top score
        // Implement a name string length check/handling

        // Assumption: Our array of scores is already sorted when it arrives here:
        score0Name.text = SceneManager.instance.playerScores[0].name;
        score0Time.text = SceneManager.instance.SecondsToFormattedTimeString(SceneManager.instance.playerScores[0].time);
        score0Points.text = SceneManager.instance.PointsToFormattedString(SceneManager.instance.playerScores[0].points);

        score1Name.text = SceneManager.instance.playerScores[1].name;
        score1Time.text = SceneManager.instance.SecondsToFormattedTimeString(SceneManager.instance.playerScores[1].time);
        score1Points.text = SceneManager.instance.PointsToFormattedString(SceneManager.instance.playerScores[1].points);

        score2Name.text = SceneManager.instance.playerScores[2].name;
        score2Time.text = SceneManager.instance.SecondsToFormattedTimeString(SceneManager.instance.playerScores[2].time);
        score2Points.text = SceneManager.instance.PointsToFormattedString(SceneManager.instance.playerScores[2].points);

        score3Name.text = SceneManager.instance.playerScores[3].name;
        score3Time.text = SceneManager.instance.SecondsToFormattedTimeString(SceneManager.instance.playerScores[3].time);
        score3Points.text = SceneManager.instance.PointsToFormattedString(SceneManager.instance.playerScores[3].points);

        score4Name.text = SceneManager.instance.playerScores[4].name;
        score4Time.text = SceneManager.instance.SecondsToFormattedTimeString(SceneManager.instance.playerScores[4].time);
        score4Points.text = SceneManager.instance.PointsToFormattedString(SceneManager.instance.playerScores[4].points);
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

        this.GameHUD.SetActive(true);
    }
}
