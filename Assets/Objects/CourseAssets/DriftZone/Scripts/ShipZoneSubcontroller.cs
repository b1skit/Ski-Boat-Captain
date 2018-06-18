using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipZoneSubcontroller : MonoBehaviour {

    //private BoxCollider ShipZoneTrigger;
    private DriftZoneController DriftZoneController;

    // Use this for initialization
    void Start () {
        DriftZoneController = this.GetComponentInParent<DriftZoneController>();
        //ShipZoneTrigger = this.GetComponent<BoxCollider>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
            DriftZoneController.OnShipZoneEnter(other.transform);
    }

    //private void OnTriggerStay(Collider other)
    //{
    //    DriftZoneController.OnShipZoneStay();
    //}

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
            DriftZoneController.OnShipZoneExit();
    }
}
