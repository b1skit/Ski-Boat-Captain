using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpLaunchZoneSubController : MonoBehaviour {

    private JumpBehavior theJumpBehaviorController;

    private AudioSource jumpSound;

    private void Start()
    {
        theJumpBehaviorController = this.GetComponentInParent<JumpBehavior>();

        jumpSound = this.GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Skier"))
        {
            theJumpBehaviorController.OnLaunchRampEntry(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Skier"))
        {
            jumpSound.Play();
        }
    }
}
