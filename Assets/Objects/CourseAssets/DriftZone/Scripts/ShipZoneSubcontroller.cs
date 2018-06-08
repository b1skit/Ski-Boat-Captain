using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipZoneSubcontroller : MonoBehaviour {

    //private BoxCollider ShipZoneTrigger;
    public DriftZoneController DriftZoneController;

    // Use this for initialization
    void Start () {
        //ShipZoneTrigger = this.GetComponent<BoxCollider>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
            DriftZoneController.OnShipZoneEnter();
    }

    private void OnTriggerStay(Collider other)
    {
        DriftZoneController.OnShipZoneStay();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
            DriftZoneController.OnShipZoneExit();
    }
}
