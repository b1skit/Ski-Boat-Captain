using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;


public class MainMenuManager : MonoBehaviour {

    [Tooltip("The main menu panel")]
    public Image mainMenuPanel;

    [Tooltip("The options panel")]
    public Image optionsMenuPanel;

    [Tooltip("The loading screen UI element")]
    public Image loadingScreenPanel;

    private AudioSource buttonPressSound;

    private InputField playerNameInputField;
    private Toggle enableMusicToggle;
    private Toggle invertSteeringToggle;


    private void Awake()
    {
        loadingScreenPanel.gameObject.SetActive(false);

        buttonPressSound = this.gameObject.GetComponent<AudioSource>();
    }

    public void LoadLevel(int level)
    {
        loadingScreenPanel.gameObject.SetActive(true);

        buttonPressSound.Play();

        GameManager.Instance.LoadSpecificLevel(level);
    }

    public void ContinueGame()
    {
        // TBC!!!!

        buttonPressSound.Play();
    }

    public void ShowOptionsScreen()
    {
        mainMenuPanel.gameObject.SetActive(false);
        optionsMenuPanel.gameObject.SetActive(true);

        buttonPressSound.Play();

        InputField[] inputFields = this.GetComponentsInChildren<InputField>();
        foreach (InputField currentInputField in inputFields)
        {
            if (currentInputField.name == "PlayerNameInputField")
            {
                playerNameInputField = currentInputField;
                break;
            }
        }
        playerNameInputField.text = PlayerPrefs.GetString("PlayerName", GameManager.Instance.defaultPlayerName);
        
        Toggle[] toggles = this.GetComponentsInChildren<Toggle>();
        foreach (Toggle currentToggle in toggles)
        {
            if (currentToggle.name == "Enable Music Toggle")
            {
                currentToggle.isOn = GameManager.Instance.enableMusic;
                enableMusicToggle = currentToggle;
            }
            else if (currentToggle.name == "Invert Steering Toggle")
            {
                #if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE // Set the state of the toggle if we're playing on mobile
                currentToggle.isOn = GameManager.Instance.invertSteering;
                invertSteeringToggle = currentToggle;
                
                #elif UNITY_STANDALONE || UNITY_WEBPLAYER // Disable the option if we're playing on PC
                currentToggle.gameObject.SetActive(false);
                #endif
            }
        }

    }

    public void DeleteSavedScores()
    {
        // Start at index 1, assuming index 0 is the main menu scene
        for (int i = 1; i < UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings; i++)
        {
            string filePath = Application.persistentDataPath + "/level" + i.ToString() + ".dat";
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }

    public void SaveCloseOptionsScreen()
    {
        mainMenuPanel.gameObject.SetActive(true);
        optionsMenuPanel.gameObject.SetActive(false);

        PlayerPrefs.SetString("PlayerName", playerNameInputField.text);

        PlayerPrefs.SetInt("enableMusic", enableMusicToggle.isOn ? 1 : 0);
        GameManager.Instance.enableMusic = enableMusicToggle.isOn;
        if (GameManager.Instance.enableMusic && !MusicManager.Instance.IsPlaying)
        {
            MusicManager.Instance.PlayMusic();
        }
        else if (!GameManager.Instance.enableMusic)
        {
            MusicManager.Instance.StopMusic();
        }

        #if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
        PlayerPrefs.SetInt("invertSteering", invertSteeringToggle.isOn ? 1 : 0);
        GameManager.Instance.invertSteering = invertSteeringToggle.isOn;
        #endif

        PlayerPrefs.Save();

        buttonPressSound.Play();
    }

    public void CancelCloseOptionsScreen()
    {
        playerNameInputField.text = "";

        mainMenuPanel.gameObject.SetActive(true);
        optionsMenuPanel.gameObject.SetActive(false);

        buttonPressSound.Play();
    }

    public void DoQuit()
    {
        buttonPressSound.Play();

        Application.Quit(); // Ignored in the editor
    }
}
