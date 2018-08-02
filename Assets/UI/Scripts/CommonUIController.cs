using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// A CommonUIController parent class, that handles functionality/properties common to multiple UI screens
public class CommonUIController : MonoBehaviour
{

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

