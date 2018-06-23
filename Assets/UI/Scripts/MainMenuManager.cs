using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour {
    
    public Image loadingScreen;

    private void Awake()
    {
        loadingScreen.gameObject.SetActive(false);
    }

    public void LoadLevel(int level)
    {
        loadingScreen.gameObject.SetActive(true);

        //UnityEngine.SceneManagement.SceneManager.LoadScene(level);
        GameManager.Instance.LoadSpecificLevel(level);
    }

}
