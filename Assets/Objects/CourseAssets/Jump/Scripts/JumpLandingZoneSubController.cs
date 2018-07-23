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
        if (other.gameObject.CompareTag("Skier"))
        {
            theJumpBehaviorController.OnLandingRampEntry(other.transform);
        }
        
    }
}
