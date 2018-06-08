using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkierZoneSubcontroller : MonoBehaviour {

    //private BoxCollider SkierZoneTrigger;
    public DriftZoneController DriftZoneController;

    // Use this for initialization
    void Start()
    {
        //SkierZoneTrigger = this.GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Skier")
            DriftZoneController.OnSkierZoneEnter();
    }

    private void OnTriggerStay(Collider other)
    {
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Skier")
            DriftZoneController.OnSkierZoneExit();
    }
}
