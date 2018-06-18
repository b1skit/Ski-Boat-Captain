using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkierZoneSubcontroller : MonoBehaviour {

    //private BoxCollider SkierZoneTrigger;
    private DriftZoneController DriftZoneController;

    // Use this for initialization
    void Start()
    {
        //SkierZoneTrigger = this.GetComponent<BoxCollider>();
        DriftZoneController = this.GetComponentInParent<DriftZoneController>();
    }

    // Update is called once per frame
    void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Skier")
        {
            DriftZoneController.OnSkierZoneEnter(other.transform);
        }
    }

    //private void OnTriggerStay(Collider other)
    //{
        
    //}

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Skier")
        {
            DriftZoneController.OnSkierZoneExit();
        }
    }
}
