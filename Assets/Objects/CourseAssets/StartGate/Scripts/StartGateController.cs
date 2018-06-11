using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGateController : MonoBehaviour {

    private bool hasLeft;
    private int lapsRemaining;

	// Use this for initialization
	void Start () {
        hasLeft = false;
        lapsRemaining = SceneManager.instance.numberOfLaps;
	}
	
	// Update is called once per frame
	void Update () {

	}

    private void OnTriggerEnter(Collider other) {

        if (hasLeft && other.gameObject.tag == "Player" && lapsRemaining == 0)
        {
            SceneManager.instance.EndLevel();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!hasLeft)
        {
            SceneManager.instance.StartLevel();
        }

        hasLeft = true;

        if (other.tag == "Player" && SceneManager.instance.IsPlaying)
        {   
            lapsRemaining--;
            SceneManager.instance.UpdateLapText(lapsRemaining);
        }
    }
}
