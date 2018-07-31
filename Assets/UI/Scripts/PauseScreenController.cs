using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// A CommonUIController parent class, that handles functionality/properties common to multiple UI screens
public class CommonUIController : MonoBehaviour {

    [Tooltip("The in-game HUD elements panel")]
    public GameObject GameHUD;

    [Tooltip("The loading screen UI panel that is part of the main HUD. This reference is used to turn it on/off")]
    public GameObject LoadingScreenUIPanel;

    [Tooltip("The pause screen button UI element used on mobile platforms")]
    public GameObject TouchScreenPauseButton;

    protected AudioSource menuButtonPress;

    private void Start()
    {
        menuButtonPress = this.GetComponent<AudioSource>();
    }

    public void DoRestart()
    {
        Time.timeScale = 1;
        LoadingScreenUIPanel.gameObject.SetActive(true);

        menuButtonPress.Play();

        GameManager.Instance.RestartLevel();
    }

    public void DoExitToMenu()
    {
        Time.timeScale = 1;
        LoadingScreenUIPanel.gameObject.SetActive(true);

        menuButtonPress.Play();

        GameManager.Instance.LoadSpecificLevel(0); // Main menu scene is expected to be build index 0
    }

    public void DoQuit()
    {
        menuButtonPress.Play();

        Application.Quit(); // Ignored in the editor
    }
}


public class PauseScreenController : CommonUIController
{
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
            menuButtonPress.Play();

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

        menuButtonPress.Play();

        #if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE // Toggle the touch screen on-screen pause button visibility
        TouchScreenPauseButton.SetActive(true);
        #endif
    }
}
