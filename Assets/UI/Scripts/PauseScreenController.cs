using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseScreenController : MonoBehaviour {

    [Tooltip("The loading screen UI panel that is part of the pause panel. A reference is required to turn it on/off")]
    public GameObject LoadingScreenUIPanel;

    public GameObject TouchScreenPauseButton;

#if UNITY_STANDALONE || UNITY_WEBPLAYER
    private void Start()
    {
        TouchScreenPauseButton.SetActive(false);
    }
#endif

    public void DoPause()
    {
        if (SceneManager.instance.IsPlaying)
        {
            // Since we call this when the player presses "escape", also allow escape to unpause
            if (!this.gameObject.activeSelf)
            { // PAUSE:
                Time.timeScale = 0;
                this.gameObject.SetActive(true);

#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE // Toggle the touch screen on-screen pause button visibility
                TouchScreenPauseButton.SetActive(false);
#endif
            }
            else
            { // UNPAUSE:
                this.gameObject.SetActive(false);
                Time.timeScale = 1;

#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE // Toggle the touch screen on-screen pause button visibility
                TouchScreenPauseButton.SetActive(true);
#endif
            }
        }
    }

    public void DoResume()
    {
        this.gameObject.SetActive(false);
        Time.timeScale = 1;

#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE // Toggle the touch screen on-screen pause button visibility
        TouchScreenPauseButton.SetActive(true);
#endif
    }

    public void DoRestart()
    {
        Time.timeScale = 1;
        LoadingScreenUIPanel.gameObject.SetActive(true);

        GameManager.Instance.RestartLevel();
    }

    public void DoExitToMenu()
    {
        Time.timeScale = 1;
        LoadingScreenUIPanel.gameObject.SetActive(true);

        GameManager.Instance.LoadSpecificLevel(0);
    }

    public void DoQuit()
    {
        Application.Quit(); // Ignored in the editor
    }
}
