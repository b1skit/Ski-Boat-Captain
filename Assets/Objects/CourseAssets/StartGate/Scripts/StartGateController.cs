using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGateController : MonoBehaviour {

    private int lapsRemaining;
    private bool isLegalLap;


    void Start()
    {
        lapsRemaining = SceneManager.Instance.numberOfLaps;

        isLegalLap = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (Vector3.Dot(other.gameObject.transform.right, this.gameObject.transform.right) > 0)
            {
                isLegalLap = true;
                lapsRemaining--;
            }
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (SceneManager.Instance.IsPlaying && isLegalLap && Vector3.Dot(other.gameObject.transform.right, this.gameObject.transform.right) > 0)
            {
                if (lapsRemaining < 0)
                {
                    SceneManager.Instance.EndLevel();
                }
            }
            else
            {
                isLegalLap = false;
                lapsRemaining++;
            }

            if (SceneManager.Instance.IsPlaying)
            {
                SceneManager.Instance.UpdateLapText(lapsRemaining);
            }
        }
    }
}
