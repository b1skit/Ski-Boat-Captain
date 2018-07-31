using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour {

    [Tooltip("The main menu panel")]
    public Image mainMenuPanel;

    [Tooltip("The options panel")]
    public Image optionsMenuPanel;

    [Tooltip("The loading screen UI element")]
    public Image loadingScreenPanel;

    //[Tooltip("The default name to use if no custom player name has been set")]
    //public string defaultPlayerName = "Player";

    private AudioSource buttonPressSound;

    private Text playerNamePlaceholderText;
    private Text playerNameText;
    private InputField playerNameInputField;

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
            }
        }

        playerNameInputField.text = PlayerPrefs.GetString("PlayerName", GameManager.Instance.defaultPlayerName);
    }

    public void SaveCloseOptionsScreen()
    {
        mainMenuPanel.gameObject.SetActive(true);
        optionsMenuPanel.gameObject.SetActive(false);

        PlayerPrefs.SetString("PlayerName", playerNameInputField.text);

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
