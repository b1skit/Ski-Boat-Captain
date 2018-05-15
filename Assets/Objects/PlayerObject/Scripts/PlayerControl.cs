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

    private Vector2 steeringTouchPositionn;
    private int steeringTouchFingerId;
    private int throttleTouchFingerId;
    private int halfScreenDeadzoneHeight;
    private int halfScreenHeight;
    private int halfScreenWidth;

    [Tooltip("Percentage of the screen remaining before we consider a user's touch to be a full turn, between 0 and 1")]
    [Range(0.0f, 1.0f)]
    public float touchSteeringDeadzoneAmount = 0.25f;

    private Rigidbody theRigidBody;

    // Use this for initialization
    void Start () {

        rotatedVelocity = new Vector3(0, 0, 0);
        unrotatedVelocity = new Vector3(0, 0, 0);

        turnInertia = 1 - turnDrag;

        steeringTouchFingerId = -1;
        throttleTouchFingerId = -1;
        halfScreenDeadzoneHeight = (int)((Screen.height * (1 - touchSteeringDeadzoneAmount)) / 2);
        halfScreenHeight = (int)(Screen.height / 2);
        halfScreenWidth = Screen.width / 2;

        theRigidBody = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update() {
        //Horizontal and Vertical are mapped to w, a, s, d and the arrow keys.
        //Debug.Log("Hello, loggy world!");

        float horizontalInput;
        float verticalInput;

        //Check if we are running either in the Unity editor or in a standalone build.
        #if UNITY_STANDALONE || UNITY_WEBPLAYER

        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        //Check if we are running on iOS, Android, Windows Phone 8 or Unity iPhone
        #elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
        
        horizontalInput = 0;
        verticalInput = 0;
        int steeringTouchIndex = 0; // This will always be overwritten before it's used

        for (int i = 0; i < Input.touches.Length; i++)
        {
            if (Input.touches[i].phase == TouchPhase.Began || Input.touches[i].phase == TouchPhase.Moved || Input.touches[i].phase == TouchPhase.Stationary)
            {
                // Steering side of the screen:
                if (Input.touches[i].position.x < halfScreenWidth)
                {
                    steeringTouchPositionn = Input.touches[i].position;
                    steeringTouchFingerId = Input.touches[i].fingerId;
                    steeringTouchIndex = i;
                }
                // Throttle side of the screen:
                else if (Input.touches[i].position.x >= halfScreenWidth)
                {
                    throttleTouchFingerId = Input.touches[i].fingerId;
                }
            }
            else if (Input.touches[i].phase == TouchPhase.Ended || Input.touches[i].phase == TouchPhase.Canceled)
            {
                // Steering side of the screen:
                if (Input.touches[i].fingerId == steeringTouchFingerId)
                {
                    steeringTouchFingerId = -1;
                }
                // Throttle side of the screen:
                else if (Input.touches[i].fingerId == throttleTouchFingerId)
                {
                    throttleTouchFingerId = -1;
                }
            }

        } // end for

        if (steeringTouchFingerId >= 0) 
        {
            //Vector2 touchLength = Input.touches[steeringTouchIndex].position - theTouchPosition;
            //horizontalInput = (touchLength.magnitude) / halfScreenDeadzoneHeight;

            //if (Vector2.Dot(Vector2.up, touchLength.normalized) < 0) // Invert the steering direction if required
            //    horizontalInput *= -1;

            float touchY = steeringTouchPositionn.y;
            if (touchY < halfScreenHeight)
            {
                horizontalInput = (halfScreenHeight - touchY) / -halfScreenDeadzoneHeight;
            }
            else if (touchY > halfScreenHeight)
            {
                horizontalInput = (touchY - halfScreenHeight) / halfScreenDeadzoneHeight;
            }

            // Need to adjust this so steering strength is simply based on how far up/down the Y axis the touch is

            if (horizontalInput > 1.0f)
                horizontalInput = 1.0f;
            else if (horizontalInput < -1.0f)
                horizontalInput = -1.0f;
        } // End steering touch handling

        if (throttleTouchFingerId >= 0)
        {
            verticalInput = 1.0f; // temp hack
        }
        else
        {
            verticalInput = 0; // temp hack
        }
        #endif

        float turnFactor = unrotatedVelocity.magnitude;
        if (turnFactor > 1.0f)
            turnFactor = 1.0f;
        if (turnFactor < 0.1f)
            turnFactor = 0.1f;

        Vector3 rotationAmount = new Vector3();
        rotationAmount.z -= rotationSpeed * turnFactor * horizontalInput * Time.deltaTime;

        Quaternion newRotation = Quaternion.Euler(rotationAmount);

        if (verticalInput > 0) {
            unrotatedVelocity += this.transform.right.normalized * verticalInput * acceleration * Time.deltaTime;
            rotatedVelocity += this.transform.right.normalized * verticalInput * acceleration * Time.deltaTime;
        }

        rotatedVelocity = newRotation * rotatedVelocity;

        unrotatedVelocity = (unrotatedVelocity * inertia) + (rotatedVelocity * (1 - inertia));

        // Bleed off speed when we bank into a turn:
        float dragFactor = Vector3.Dot(unrotatedVelocity.normalized, rotatedVelocity.normalized);
        dragFactor = turnInertia + (turnDrag * dragFactor);

        unrotatedVelocity *= drag * dragFactor;
        rotatedVelocity *= drag * dragFactor;

        theRigidBody.MoveRotation(this.transform.rotation * newRotation);
        theRigidBody.MovePosition(this.transform.position + unrotatedVelocity);

    }
}
