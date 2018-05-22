using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour {

    [Header("Ship control")]
    public float drag = .99f;

    [Tooltip("Drag to apply when turning, between 0 and 1")]
    [Range(0.0f, 1.0f)]
    public float turnDrag = 0.1f;

    private float turnInertia;

    [Tooltip("How quickly the ship turns, from 0 to infinity")]
    public float rotationSpeed = 150.0f; // TO DO: Range slider???

    [Tooltip("How quickly the ship accelerates, between 0 and ???")]
    public float acceleration = 0.75f; // TO DO: Range slider???

    [Tooltip("Strength of boat's tendency of the boat to maintain it's current velocity, between 0 and 1")]
    [Range(0.0f, 1.0f)]
    public float inertia = 0.99f;

    [Space(10)]

    [Header("Touch screen settings")]
    [Tooltip("Percentage of the screen remaining before we consider a user's touch to be a full turn, between 0 and 1")]
    [Range(0.0f, 1.0f)]
    public float touchSteeringDeadzoneAmount = 0.25f;
    public float touchThrottleDeadzoneAmount = 0.25f;

    private Vector2 steeringTouchPosition; // TO DO: Replace these with a local variable? No need to maintain a copy between frames (?)
    private Vector2 throttleTouchPosition;
    private int steeringTouchFingerId;
    private int throttleTouchFingerId;
    private int halfScreenDeadzoneHeight;
    private int halfScreenHeight;
    private int halfScreenWidth;
    private int halfScreenDeadzoneWidth;

    [Space(10)]

    [Header("Visuals")]
    [Tooltip("The transform of the child player ship object (ie. the child that contains the visible mesh filter and mesh renderer)")]
    public Transform viewMeshTransform;

    [Tooltip("Tilt factor for how quickly the ship visually banks (does not influence control)")]
    public float shipTiltSpeed = 10;

    private Vector3 rotatedVelocity;
    private Vector3 unrotatedVelocity;

    private float shipRotation;

    private Rigidbody theRigidBody;
    private Mesh thePlayerShip;

    // Use this for initialization
    void Start () {

        rotatedVelocity = new Vector3(0, 0, 0);
        unrotatedVelocity = new Vector3(0, 0, 0);

        turnInertia = 1 - turnDrag;

        steeringTouchFingerId = -1; // TO DO: Replace these with a boolean: isSteering, isThrottling
        throttleTouchFingerId = -1;
        halfScreenDeadzoneHeight = (int)((Screen.height * (1 - touchSteeringDeadzoneAmount)) / 2);
        halfScreenHeight = (int)(Screen.height / 2);
        halfScreenDeadzoneWidth = (int)((Screen.width * (1 - touchThrottleDeadzoneAmount)) / 2);
        halfScreenWidth = Screen.width / 2;

        theRigidBody = this.GetComponent<Rigidbody>();
        thePlayerShip = this.gameObject.GetComponent<Mesh>();

        shipRotation = 0.0f;

        shipTiltSpeed *= -1; // Negate our tilt speed once here. Done so the public script input takes positive values
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
        int steeringTouchIndex = 0; // These will always be overwritten before they're used
        int throttleTouchIndex = 0;

        for (int i = 0; i < Input.touches.Length; i++)
        {
            if (Input.touches[i].phase == TouchPhase.Began || Input.touches[i].phase == TouchPhase.Moved || Input.touches[i].phase == TouchPhase.Stationary)
            {
                // Steering side of the screen:
                if (Input.touches[i].position.x < halfScreenWidth)
                {
                    steeringTouchPosition = Input.touches[i].position;
                    steeringTouchFingerId = Input.touches[i].fingerId;
                    steeringTouchIndex = i;
                }
                // Throttle side of the screen:
                else if (Input.touches[i].position.x >= halfScreenWidth)
                {
                    throttleTouchPosition = Input.touches[i].position;
                    throttleTouchFingerId = Input.touches[i].fingerId;
                    throttleTouchIndex = i;
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

        // Handle steering
        if (steeringTouchFingerId >= 0) 
        {
            float touchY = steeringTouchPosition.y;
            if (touchY < halfScreenHeight)
            {
                horizontalInput = (halfScreenHeight - touchY) / -halfScreenDeadzoneHeight;
            }
            else if (touchY > halfScreenHeight)
            {
                horizontalInput = (touchY - halfScreenHeight) / halfScreenDeadzoneHeight;
            }

            if (horizontalInput > 1.0f)
                horizontalInput = 1.0f;
            else if (horizontalInput < -1.0f)
                horizontalInput = -1.0f;
        } // End steering touch handling

        // Handle throttling
        if (throttleTouchFingerId >= 0)
        {
            float touchX = throttleTouchPosition.x; // At this point, we already know the user IS applying throttle

            verticalInput = (touchX - halfScreenWidth) / halfScreenDeadzoneWidth;

            if (verticalInput > 1.0f)
                verticalInput = 1.0f;
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

        // Add new throttle input to the velocities
        if (verticalInput > 0) {
            unrotatedVelocity += this.transform.right.normalized * verticalInput * acceleration * Time.deltaTime;
            rotatedVelocity += this.transform.right.normalized * verticalInput * acceleration * Time.deltaTime;
        }

        rotatedVelocity = newRotation * rotatedVelocity;
        unrotatedVelocity = (unrotatedVelocity * inertia) + (rotatedVelocity * (1 - inertia)); // TO DO: Precalculate 1-inertia

        // Bleed off speed when we bank into a turn:
        float dragFactor;
        if (unrotatedVelocity.magnitude != 0)
            dragFactor = Vector3.Dot(unrotatedVelocity.normalized, rotatedVelocity.normalized);
        else // BUG FIX: Prevents ship jittering when game first starts, due to vectors being 0 and .normalized undefined
            dragFactor = 1;
        
        float bankAmount = 1.0f - (dragFactor); // Store this for visible mesh rotation

        dragFactor = turnInertia + (turnDrag * dragFactor);
        unrotatedVelocity *= drag * dragFactor;
        rotatedVelocity *= drag * dragFactor;

        // Transform the player object based on our updated velocities:
        theRigidBody.MoveRotation(this.transform.rotation * newRotation);
        theRigidBody.MovePosition(this.transform.position + unrotatedVelocity);

        // Rotate the ship's visible mesh:
        if (Mathf.Abs(shipRotation) < 45.0f)
        {
            //shipRotation += horizontalInput * -3;
            shipRotation += bankAmount * Mathf.Sign(horizontalInput) * shipTiltSpeed;
        }
        if (Mathf.Abs(shipRotation) > 5.0f)
        {
            shipRotation -= Mathf.Sign(shipRotation) * 1; // temp hack
        }
        viewMeshTransform.localRotation = Quaternion.Euler(shipRotation, 0, 0);
    }
}
