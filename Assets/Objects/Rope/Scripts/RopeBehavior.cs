using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeBehavior : MonoBehaviour {

    public Transform playerShipTransform;
    public Transform skierTransform;

    private Transform[] ropePlaneTransforms;

	// Use this for initialization
	void Start () {
        ropePlaneTransforms = this.GetComponentsInChildren<Transform>();
	}
	
	// Update is called once per frame
	void Update () {
        this.transform.position = Vector3.Lerp(playerShipTransform.position, skierTransform.position, 0.5f);

        this.transform.LookAt(playerShipTransform.position);

        foreach (Transform current in ropePlaneTransforms)
        {
            current.localScale = new Vector3(Vector3.Distance(playerShipTransform.position, skierTransform.position), current.localScale.y, current.localScale.z);
        }
	}
}
