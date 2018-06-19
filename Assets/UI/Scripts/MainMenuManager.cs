using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour {

    public void LoadLevel(int level)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(level);

    }

}
