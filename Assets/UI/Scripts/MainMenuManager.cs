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

        GameManager.Instance.LoadSpecificLevel(level);
    }

}
