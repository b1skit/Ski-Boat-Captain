using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndLevelMenuController : MonoBehaviour {

    [Tooltip("The loading screen UI panel that is part of the pause panel. A reference is required to turn it on/off")]
    public Image loadingScreen;

    [Tooltip("The pause screen button UI element used on mobile platforms")]
    public GameObject TouchScreenPauseButton;

    private void Start()
    {
        
    }

    public void DoDisplayEndLevelMenu()
    {
        #if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE // Toggle the touch screen on-screen pause button visibility
        TouchScreenPauseButton.gameObject.SetActive(false);
        #endif

        this.gameObject.SetActive(true);
    }

    public void DoLoadNextLevel()
    {
        #if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE // Toggle the touch screen on-screen pause button visibility
        TouchScreenPauseButton.gameObject.SetActive(true);
        #endif

        loadingScreen.gameObject.SetActive(true);

        GameManager.Instance.LoadNextLevel();
    }
}
