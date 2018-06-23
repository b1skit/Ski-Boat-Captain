using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseScreenController : MonoBehaviour {

    [Tooltip("The loading screen UI panel that is part of the pause panel. A reference is required to turn it on/off")]
    public GameObject LoadingScreenUIPanel;

    public void DoPause()
    {
        // Since we call this when the player presses "escape", also allow escape to unpause
        if (!this.gameObject.activeSelf)
        {
            Time.timeScale = 0;
            this.gameObject.SetActive(true);
        }
        else
        {
            this.gameObject.SetActive(false);
            Time.timeScale = 1;
        }
    }

    public void DoResume()
    {
        this.gameObject.SetActive(false);
        Time.timeScale = 1;
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
