using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGateController : MonoBehaviour {

    private int lapsRemaining;

	// Use this for initialization
	void Start () {
        lapsRemaining = SceneManager.instance.numberOfLaps;
	}
	
	// Update is called once per frame
	void Update () {

	}

    // TO DO: Implement a check to ensure the player is entering this gate from the correct direction
    private void OnTriggerEnter(Collider other) {

        if (other.gameObject.tag == "Player" && lapsRemaining == 0)
        {
            SceneManager.instance.EndLevel();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" && SceneManager.instance.IsPlaying)
        {   
            lapsRemaining--;
            SceneManager.instance.UpdateLapText(lapsRemaining);
        }
    }
}
