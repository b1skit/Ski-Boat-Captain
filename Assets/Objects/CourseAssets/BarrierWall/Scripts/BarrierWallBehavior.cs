using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierWallBehavior : MonoBehaviour {

    private Rigidbody theRigidbody;

	// Use this for initialization
	void Start () {
        theRigidbody = this.GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        theRigidbody.velocity = Vector3.zero;
    }

    private void OnCollisionStay(Collision collision)
    {
        theRigidbody.velocity = Vector3.zero;
    }

    private void OnCollisionExit(Collision collision)
    {
        theRigidbody.velocity = Vector3.zero;
    }
}
