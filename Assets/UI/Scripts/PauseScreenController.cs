using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PauseScreenController : CommonUIController
{
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
            }
            else
            { // UNPAUSE:
                menuButtonPress.Play();

                this.gameObject.SetActive(false);
                Time.timeScale = 1;
            }
        }
    }


    public void DoResume()
    {
        menuButtonPress.Play();

        this.gameObject.SetActive(false);
        Time.timeScale = 1;
    }
}
