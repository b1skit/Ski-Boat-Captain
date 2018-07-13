using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipZoneSubcontroller : MonoBehaviour {

    private DriftZoneController DriftZoneController;

    // Use this for initialization
    void Start () {
        DriftZoneController = this.GetComponentInParent<DriftZoneController>();
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            DriftZoneController.OnShipZoneEnter(other.transform);
    }

    //private void OnTriggerStay(Collider other)
    //{
    //    DriftZoneController.OnShipZoneStay();
    //}

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            DriftZoneController.OnShipZoneExit();
    }
}
