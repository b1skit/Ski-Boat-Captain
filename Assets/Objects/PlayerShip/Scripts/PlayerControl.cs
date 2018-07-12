using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour {

    [Header("Ship control:")]
    [Tooltip("Minimum fraction of velocity to maintain when applying drag between each frame (ie. The maximum drag factor that can be applied)")]
    [Range(0.0f, 1.0f)]
    public float forwardInertia = 0.80f;

    [Tooltip("Basic drag factor applied during every frame to reduce the ship's velocity (regardless of rotation etc). (ie. Fraction of 1-forwardInertia to retain of velocity")]
    public float constantDrag = .9f;

    private float oneMinusForwardInertia;

    [Tooltip("How quickly the ship turns, from 0 to infinity")]
    [Range(1.0f, 1000.0f)]
    public float rotationSpeed = 500.0f;

    [Tooltip("How quickly the ship accelerates, between 0 and 1")]
    [Range(0.0f, 1.0f)]
    public float acceleration = 0.15f;

    [Tooltip("Strength of boat's tendency of the boat to maintain it's current velocity when turning, between 0 and 1")]
    [Range(0.0f, 1.0f)]
    public float turnInertia = 0.999f;

    private float horizontalInput;
    private float verticalInput;
    private float oneMinusInertia;
    private Vector3 rotatedVelocity;
    private Vector3 unrotatedVelocity;
    private Quaternion newRotation;
    private Rigidbody theRigidBody;

    [Space(10)]

    [Header("Touch screen settings:")]
    [Tooltip("Percentage of the screen remaining before we consider a user's touch to be a full turn, between 0 and 1")]
    [Range(0.0f, 1.0f)]
    public float touchSteeringDeadzoneAmount = 0.5f;
    [Tooltip("Percentage of the screen remaining before we consider a user's touch to be full throttle, between 0 and 1")]
    [Range(0.0f, 1.0f)]
    public float touchThrottleDeadzoneAmount = 0.2f;

    private Vector2 steeringTouchPosition; // TO DO: Replace these with a local variable? No need to maintain a copy between frames (?)
    private Vector2 throttleTouchPosition;
    private int steeringTouchFingerId;
    private int throttleTouchFingerId;
    private int halfScreenDeadzoneHeight;
    private int halfScreenHeight;
    private int halfScreenWidth;
    private int halfScreenDeadzoneWidth;

    [Space(10)]

    [Header("Visuals:")]
    [Tooltip("The transform of the child player ship object (ie. the child that contains the visible mesh filter and mesh renderer)")]
    public Transform viewMeshTransform;

    [Tooltip("The transform for the left motor mesh object")]
    public Transform motorLTransform;

    [Tooltip("The transform for the right motor mesh object")]
    public Transform motorRTransfrom;

    [Tooltip("The max angle the motor should rotate when turning. Corresponds to player input direction")]
    public float maxMotorRotationAngle = 25.0f;

    [Tooltip("Tilt factor for how quickly the ship visually banks (does NOT influence control)")]
    public float shipTiltSpeed = 20.0f;
    public float maxTiltAngle = 45.0f;
    public float minTiltAngle = 5.0f;
    [Tooltip("Base angle the nose raises when the throttle is applied (nose tilt sine oscillation is added to this)")]
    [Range(0.0f, 90.0f)]
    public float throttleBaseNoseTilt = 15.0f;
    [Tooltip("Factor used to scale the amplitude of the throttle nose tilt sine oscillation")]
    [Range(0.0f, 50.0f)]
    public float throttleNoseTiltOscillationAmplitude = 10.0f;
    [Tooltip("Factor used to scale the time delta of the throttle nose tilt sine oscillation")]
    [Range(0.0f, 50.0f)]
    public float throttleNoseTiltOscillationPeriod = 4.0f;

    private float twoPI;
    private Vector3 shipLocalRotation;

    [Header("Camera:")]
    public Rigidbody cameraRigidbody;
    public Camera theCamera;
    public float maxCameraSize = 10.0f;
    public float cameraVelocityScaleFactor = 10.0f;
    public float cameraShrinkFactor = 0.99f;

    [Tooltip("The speed of the lerp between the camera rotation and the ship rotation, smaller is slower. [0, 1]")]
    [Range(0.0f, 1.0f)]
    public float cameraRotationFollowSpeed = 0.05f;

    private float minCameraSize;


    // TO DO: Surround Android-specific variables and initialization steps in #if #elif stuff!!!!!!


    // Use this for initialization
    void Start () {

        rotatedVelocity = Vector3.zero;
        unrotatedVelocity = Vector3.zero;
        newRotation = Quaternion.Euler(Vector3.zero);

        oneMinusForwardInertia = 1.0f - forwardInertia;

        steeringTouchFingerId = -1; // TO DO: Replace these with a boolean: isSteering, isThrottling
        throttleTouchFingerId = -1;
        halfScreenDeadzoneHeight = (int)((Screen.height * (1 - touchSteeringDeadzoneAmount)) / 2);
        halfScreenHeight = (int)(Screen.height / 2);
        halfScreenDeadzoneWidth = (int)((Screen.width * (1 - touchThrottleDeadzoneAmount)) / 2);
        halfScreenWidth = Screen.width / 2;

        theRigidBody = this.GetComponent<Rigidbody>();

        shipLocalRotation = new Vector3(0.0f, 0.0f, 0.0f);

        shipTiltSpeed *= -1; // Negate our tilt speed once here. Done so the public script input takes positive values
        twoPI = 2 * Mathf.PI;
        oneMinusInertia = 1.0f - turnInertia;

        minCameraSize = theCamera.orthographicSize;

        horizontalInput = 0;
        verticalInput = 0;

        throttleTouchPosition = Vector2.zero;
    }

    // TO DO: Neaten this up by breaking each task in Update into sub-functions, but set them as force inline!!!!;

    // Update is called once per frame
    void Update() { //Horizontal and Vertical are mapped to w, a, s, d and the arrow keys...

#if UNITY_STANDALONE || UNITY_WEBPLAYER // Handle Unity editor/standalone build

        horizontalInput = Input.GetAxis("Horizontal");
        if (SceneManager.instance.IsPlaying)
        {
            verticalInput = Input.GetAxis("Vertical");
        }
        else
            verticalInput = 0.0f;

        // Pass throttle value to the GameManager to update the UI:
        SceneManager.instance.UpdateThrottleValue(verticalInput, throttleTouchPosition);
        
#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE // Handle iOS/Android/Windows Phone 8/Unity iPhone

        horizontalInput = 0;
        bool isNewTouch = false;

        for (int i = 0; i < Input.touches.Length; i++)
        {
            if (Input.touches[i].phase == TouchPhase.Began || Input.touches[i].phase == TouchPhase.Moved || Input.touches[i].phase == TouchPhase.Stationary)
            {
                // Steering side of the screen:
                if (Input.touches[i].position.x < halfScreenWidth)
                {
                    steeringTouchPosition = Input.touches[i].position;
                    steeringTouchFingerId = Input.touches[i].fingerId;
                }
                // Throttle side of the screen:
                else if (Input.touches[i].position.x >= halfScreenWidth)
                {
                    throttleTouchPosition = Input.touches[i].position;
                    throttleTouchFingerId = Input.touches[i].fingerId;

                    if (Input.touches[i].phase == TouchPhase.Began)
                        isNewTouch = true;
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

        if (!SceneManager.instance.IsPlaying)
        {
            verticalInput = 0.0f;
        }

        // Pass throttle value to the SceneManager to update the UI:
        SceneManager.instance.UpdateThrottleValue(verticalInput, throttleTouchPosition, isNewTouch);
#endif

        float bankAmount;
        if (unrotatedVelocity.magnitude != 0)
            bankAmount = 1.0f - Vector3.Dot(unrotatedVelocity.normalized, rotatedVelocity.normalized);
        else // BUG FIX: Prevents ship jittering when game first starts, due to vectors being 0 and .normalized undefined
            bankAmount = 0;

        // Animate the ship's visible mesh:
        if (Mathf.Abs(shipLocalRotation.x) <= maxTiltAngle) // Increment
        {
            shipLocalRotation.x += bankAmount * horizontalInput * shipTiltSpeed;
        }

        if (Mathf.Abs(shipLocalRotation.x) >= minTiltAngle) // Decrement
        {
            shipLocalRotation.x -= Mathf.Sign(shipLocalRotation.x);
        }
        
        shipLocalRotation.y = (shipLocalRotation.y + (throttleNoseTiltOscillationPeriod * Time.deltaTime)) % (twoPI);

        viewMeshTransform.localRotation = Quaternion.Euler(shipLocalRotation.x, (verticalInput * throttleBaseNoseTilt) + (1 + unrotatedVelocity.magnitude) * throttleNoseTiltOscillationAmplitude * Mathf.Sin(shipLocalRotation.y), shipLocalRotation.z);

        // Rotate the motor view models:
        float motorRotationValue = horizontalInput * maxMotorRotationAngle;
        motorLTransform.localRotation = Quaternion.Euler(motorLTransform.localRotation.eulerAngles.x, motorLTransform.localRotation.eulerAngles.y, motorRotationValue);
        motorRTransfrom.localRotation = Quaternion.Euler(motorRTransfrom.localRotation.eulerAngles.x, motorRTransfrom.localRotation.eulerAngles.y, motorRotationValue);

        // Rotate/scale the camera to match the ship
        if (theCamera.orthographicSize < maxCameraSize)
        {
            theCamera.orthographicSize = minCameraSize + cameraVelocityScaleFactor * rotatedVelocity.magnitude;
        }
        else if (theCamera.orthographicSize > minCameraSize)
        {
            theCamera.orthographicSize *= cameraShrinkFactor;

            if (theCamera.orthographicSize < minCameraSize)
            {
                theCamera.orthographicSize = minCameraSize;
            }
        }
        cameraRigidbody.MoveRotation(Quaternion.Lerp(cameraRigidbody.transform.rotation, this.theRigidBody.transform.rotation, cameraRotationFollowSpeed)); // From, To, Speed

    }
    

    private void FixedUpdate()
    {
        float turnFactor = unrotatedVelocity.magnitude;
        if (turnFactor > 1.0f)
            turnFactor = 1.0f;
        if (turnFactor < 0.1f)
            turnFactor = 0.1f;

        Vector3 rotationAmount = new Vector3();
        rotationAmount.z -= rotationSpeed * turnFactor * horizontalInput * Time.fixedDeltaTime;

        newRotation = Quaternion.Euler(rotationAmount);

        // Add new throttle input to the velocities
        if (verticalInput > 0)
        {
            unrotatedVelocity += this.transform.right.normalized * verticalInput * acceleration * Time.fixedDeltaTime;
            rotatedVelocity += this.transform.right.normalized * verticalInput * acceleration * Time.fixedDeltaTime;
        }
        
        rotatedVelocity = newRotation * rotatedVelocity;
        unrotatedVelocity = (unrotatedVelocity * turnInertia) + (rotatedVelocity * oneMinusInertia);

        // Bleed off speed when we bank into a turn:
        float dragFactor;
        if (unrotatedVelocity.magnitude != 0)
            dragFactor = Vector3.Dot(unrotatedVelocity.normalized, rotatedVelocity.normalized);
        else // BUG FIX: Prevents ship jittering when game first starts, due to vectors being 0 and .normalized undefined
            dragFactor = 1;
       
        dragFactor = forwardInertia + (oneMinusForwardInertia * constantDrag * dragFactor);
        unrotatedVelocity *= dragFactor;
        rotatedVelocity *= dragFactor;
        
        theRigidBody.MoveRotation(this.transform.rotation * newRotation); // Rotates, with interpolation
        theRigidBody.velocity = unrotatedVelocity  * 100; // TO DO: Parameterize this!
    }

}
