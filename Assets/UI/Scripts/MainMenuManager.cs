using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour {
    
    public Image loadingScreen;

    public void LoadLevel(int level)
    {
        loadingScreen.transform.gameObject.SetActive(true);

        UnityEngine.SceneManagement.SceneManager.LoadScene(level);

    }

}
