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

    private AudioSource buttonPressSound;

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
    }

    public void SaveCloseOptionsScreen()
    {
        mainMenuPanel.gameObject.SetActive(true);
        optionsMenuPanel.gameObject.SetActive(false);

        buttonPressSound.Play();
    }

    public void CancelCloseOptionsScreen()
    {
        mainMenuPanel.gameObject.SetActive(true);
        optionsMenuPanel.gameObject.SetActive(false);

        buttonPressSound.Play();
    }
}
