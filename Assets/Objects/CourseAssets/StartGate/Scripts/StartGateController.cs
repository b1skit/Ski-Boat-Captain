using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGateController : MonoBehaviour {

    private int lapsRemaining;
    private bool isLegalLap;

	// Use this for initialization
	void Start () {
        lapsRemaining = SceneManager.Instance.numberOfLaps;

        isLegalLap = true;
	}

    private void OnTriggerEnter(Collider other) {

        if (other.gameObject.CompareTag("Player") )
        {
            if (SceneManager.Instance.IsPlaying && Vector3.Dot(other.gameObject.transform.right, this.gameObject.transform.right) < 0)
                isLegalLap = false;

            if (isLegalLap && lapsRemaining == 0)
            {
                SceneManager.Instance.EndLevel();
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player"))
        {
            if (SceneManager.Instance.IsPlaying && isLegalLap && Vector3.Dot(other.gameObject.transform.right, this.gameObject.transform.right) > 0)
            {
                lapsRemaining--;
                SceneManager.Instance.UpdateLapText(lapsRemaining);
            }

            // Ensure the player only receives credit for valid laps:
            if (Vector3.Dot(other.gameObject.transform.right, this.gameObject.transform.right) > 0)
            {
                isLegalLap = true;
            }
            else
                isLegalLap = false;
        }
        
    }
}
