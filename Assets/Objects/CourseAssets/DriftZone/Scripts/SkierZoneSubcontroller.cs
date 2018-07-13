using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkierZoneSubcontroller : SkierInteractionZoneBehavior
{

    //private BoxCollider SkierZoneTrigger;
    private DriftZoneController DriftZoneController;

    // Use this for initialization
    void Start()
    {
        DriftZoneController = this.GetComponentInParent<DriftZoneController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Skier"))
        {
            DriftZoneController.OnSkierZoneEnter(other.transform);
        }
    }

    //private void OnTriggerStay(Collider other)
    //{
        
    //}

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Skier"))
        {
            DriftZoneController.OnSkierZoneExit();
        }
    }
}
