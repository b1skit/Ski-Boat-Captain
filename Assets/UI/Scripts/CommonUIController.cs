using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// A CommonUIController parent class, that handles functionality/properties common to multiple UI screens
public class CommonUIController : MonoBehaviour
{
    [Tooltip("The loading screen UI panel that is part of the main HUD. This reference is used to turn it on/off")]
    public GameObject LoadingScreenUIPanel;

    [Tooltip("The pause screen button UI element used on mobile platforms")]
    public GameObject touchScreenPauseButton;

    protected AudioSource menuButtonPress;


    private void Awake()
    {
        menuButtonPress = this.GetComponent<AudioSource>(); // We need to do this in Awake() instead of Start(), otherwise AudioSources will be null for a short while
    }


    public void DoRestart()
    {
        LoadingScreenUIPanel.gameObject.SetActive(true);

        menuButtonPress.Play();

        GameManager.Instance.RestartLevelImmediate();
    }


    public void DoExitToMenu()
    {
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

