using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpLandingZoneSubController : MonoBehaviour {

    private JumpBehavior theJumpBehaviorController;

    private void Start()
    {
        theJumpBehaviorController = this.GetComponentInParent<JumpBehavior>();
    }

    private void OnTriggerEnter(Collider other)
    {
        theJumpBehaviorController.OnLandingRampEntry();
    }

    private void OnTriggerExit(Collider other)
    {
        theJumpBehaviorController.OnLandingRampExit();
    }
}
