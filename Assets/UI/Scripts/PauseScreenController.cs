using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PauseScreenController : CommonUIController
{
    #if UNITY_STANDALONE || UNITY_WEBPLAYER
    private void Start()
    {
        touchScreenPauseButton.SetActive(false);
    }
    #endif


    public void DoPause()
    {
        if (SceneManager.Instance.IsPlaying)
        {
            // Since we call this when the player presses "escape", also allow escape to unpause
            if (!this.gameObject.activeSelf)
            { // PAUSE:
                Time.timeScale = 0;
                this.gameObject.SetActive(true);

                menuButtonPress.Play();

                #if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE // Toggle the touch screen on-screen pause button visibility
                touchScreenPauseButton.SetActive(false);
                #endif
            }
            else
            { // UNPAUSE:
                menuButtonPress.Play();

                this.gameObject.SetActive(false);
                Time.timeScale = 1;

                #if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE // Toggle the touch screen on-screen pause button visibility
                touchScreenPauseButton.SetActive(true);
                #endif
            }
        }
    }


    public void DoResume()
    {
        menuButtonPress.Play();

        this.gameObject.SetActive(false);
        Time.timeScale = 1;

        #if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE // Toggle the touch screen on-screen pause button visibility
        touchScreenPauseButton.SetActive(true);
        #endif
    }
}
