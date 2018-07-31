using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Inherits from the CommonUIController defined in PauseScreenController.cs
public class EndLevelMenuController : CommonUIController {

    public void DoDisplayEndLevelMenu()
    {
#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE // Toggle the touch screen on-screen pause button visibility
        TouchScreenPauseButton.SetActive(false);
#endif

        this.GameHUD.SetActive(false);
        this.gameObject.SetActive(true);
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
