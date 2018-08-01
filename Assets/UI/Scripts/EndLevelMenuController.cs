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

        this.GameHUD.SetActive(false);
        this.gameObject.SetActive(true);

        // Need to calculate the user's score, display it on the screen somewhere, and then insert it into the scoreboard if it's a new top score
        // Assumption: Our array of scores is already sorted when it arrives here?

        // WE NEED TO IMPLEMENT A FLOAT->TIME FORMATTED STRING FUNCTION HERE!!!
        // Must implement a score formatting function to insert commmas (eg. 000,987,123)
        // Implement a name string length check/handling

        score0Name.text = SceneManager.instance.playerScores[0].name;
        score0Time.text = SceneManager.instance.playerScores[0].time.ToString(); 
        score0Points.text = SceneManager.instance.playerScores[0].points.ToString();

        score1Name.text = SceneManager.instance.playerScores[1].name;
        score1Time.text = SceneManager.instance.playerScores[1].time.ToString();
        score1Points.text = SceneManager.instance.playerScores[1].points.ToString();

        score2Name.text = SceneManager.instance.playerScores[2].name;
        score2Time.text = SceneManager.instance.playerScores[2].time.ToString();
        score2Points.text = SceneManager.instance.playerScores[2].points.ToString();

        score3Name.text = SceneManager.instance.playerScores[3].name;
        score3Time.text = SceneManager.instance.playerScores[3].time.ToString();
        score3Points.text = SceneManager.instance.playerScores[3].points.ToString();

        score4Name.text = SceneManager.instance.playerScores[4].name;
        score4Time.text = SceneManager.instance.playerScores[4].time.ToString();
        score4Points.text = SceneManager.instance.playerScores[4].points.ToString();


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
