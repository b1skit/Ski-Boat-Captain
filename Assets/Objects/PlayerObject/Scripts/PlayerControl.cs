using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour {
    public float drag = .99f;
    [Tooltip("Drag to apply when turning, between 0 and 1")]
    [Range(0.0f, 1.0f)]
    public float turnDrag = 0.1f;
    private float turnInertia;
    public float rotationSpeed = 150.0f;
    public float acceleration = 0.75f;
    [Tooltip("Strength of boat's tendency of the boat to maintain it's current velocity, between 0 and 1")]
    [Range(0.0f, 1.0f)]
    public float inertia = 0.99f;

    private Vector3 rotatedVelocity;
    private Vector3 unrotatedVelocity;

    // Use this for initialization
    void Start () {

        rotatedVelocity = new Vector3(0, 0, 0);
        unrotatedVelocity = new Vector3(0, 0, 0);

        turnInertia = 1 - turnDrag;
    }

    // Update is called once per frame
    void Update() {
        //Horizontal and Vertical are mapped to w, a, s, d and the arrow keys.

        //Debug.Log("Hello, loggy world!");
        float turnFactor = unrotatedVelocity.magnitude;
        if (turnFactor > 1.0f)
            turnFactor = 1.0f;
        if (turnFactor < 0.1f)
            turnFactor = 0.1f;


        Vector3 rotationAmount = new Vector3();
        rotationAmount.z -= rotationSpeed * turnFactor * Input.GetAxis("Horizontal") * Time.deltaTime;

        Quaternion rotation = Quaternion.Euler(rotationAmount);

        if (Input.GetAxis("Vertical") > 0) {
            unrotatedVelocity += this.transform.right.normalized * Input.GetAxis("Vertical") * acceleration * Time.deltaTime;
            rotatedVelocity += this.transform.right.normalized * Input.GetAxis("Vertical") * acceleration * Time.deltaTime;
        }

        rotatedVelocity = rotation * rotatedVelocity;

        
        unrotatedVelocity = (unrotatedVelocity * inertia) + (rotatedVelocity * (1 - inertia));

        // Bleed off speed when we bank into a turn:
        float dragFactor = Vector3.Dot(unrotatedVelocity.normalized, rotatedVelocity.normalized);
        dragFactor = turnInertia + (turnDrag * dragFactor);


        unrotatedVelocity *= drag * dragFactor;
        rotatedVelocity *= drag * dragFactor;

        this.gameObject.transform.Rotate(rotationAmount);


        this.gameObject.transform.Translate(unrotatedVelocity, Space.World);
    }
}
