using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGateController : MonoBehaviour {

    private bool hasLeft;

	// Use this for initialization
	void Start () {
        hasLeft = false;
	}
	
	// Update is called once per frame
	void Update () {

	}

    private void OnTriggerEnter(Collider other) {

        if (hasLeft && other.gameObject.tag == "Player")
        {
            GameManager.StaticManager.StopTimer();
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (!hasLeft)
        {
            GameManager.StaticManager.StartTimer();

        }

        hasLeft = true;
    }
}
