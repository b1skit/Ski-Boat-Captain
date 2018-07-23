using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpLaunchZoneSubController : MonoBehaviour {

    private JumpBehavior theJumpBehaviorController;

    private void Start()
    {
        theJumpBehaviorController = this.GetComponentInParent<JumpBehavior>();
    }

    private void OnTriggerEnter(Collider other)
    {
        theJumpBehaviorController.OnLaunchRampEntry();
    }
}
