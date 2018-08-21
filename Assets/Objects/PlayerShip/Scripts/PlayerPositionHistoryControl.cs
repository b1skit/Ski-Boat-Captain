using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerPositionHistoryControl : MonoBehaviour
{
    private struct PositionHistory
    {
        public Vector3 shipPosition;
        public Quaternion shipRotation;

        public Vector3 skierPosition;
        public Quaternion skierRotation;

        private const float MAX_SKIER_TO_SHIP_DISTANCE = 4.0f;

        public PositionHistory(Transform shipTransform, Transform skierTransform)
        {
            shipPosition = shipTransform.position;
            shipRotation = shipTransform.rotation;

            skierPosition = skierTransform.position;
            skierRotation = skierTransform.rotation;

            // Move the skier closer if it's too far away
            Vector3 shipToSkier = shipPosition - skierPosition;
            if (shipToSkier.magnitude > MAX_SKIER_TO_SHIP_DISTANCE)
            {
                skierPosition = shipPosition - (shipToSkier.normalized * MAX_SKIER_TO_SHIP_DISTANCE);
            }
        }
    }


    [Tooltip("The in-game skier object")]
    public GameObject theSkier;

    [Tooltip("The skier prefab. Used to re-instantiate the skier if it is lost/destroyed/disconnected")]
    public GameObject skierPrefab;

    [Tooltip("The in-game rope object")]
    public GameObject theRope;


    private ConfigurableJoint skiRopeJoint;

    [Tooltip("The frequency to save the ships position, in seconds")]
    public float saveInterval = 0.25f;
    private float intervalTime;

    [Tooltip("The amount of time to rewind when the player fails, in seconds")]
    public float rewindAmount = 2f;

    [Tooltip("The amount of time to wait before rewinding, in seconds")]
    public float rewindDelay = 2f;

    private Queue<PositionHistory> thePositionHistory;
    private int queueSize;

    private bool isRewinding;
    private PositionHistory resetPosition;


    // Use this for initialization
    void Start () {
        intervalTime = 0f;

        queueSize = (int)(rewindAmount / saveInterval);
        if ((rewindAmount / saveInterval) % 1 != 0)
        {
            Debug.LogError("ERROR! rewindamount/saveInterval MUST equal an integer.");
        }

        thePositionHistory = new Queue<PositionHistory>(queueSize + 1); // Add an extra element to allow the queue to expand without requiring a new allocation

        skiRopeJoint = theSkier.GetComponent<ConfigurableJoint>();

        isRewinding = false;
	}
	

	// Update is called once per frame
	void Update () {
        // Store our position data:
        intervalTime += Time.deltaTime;
        if (intervalTime >= saveInterval)
        {
            intervalTime %= saveInterval;

            thePositionHistory.Enqueue(new PositionHistory(this.transform, theSkier.transform) );

            if (thePositionHistory.Count > queueSize)
            {
                thePositionHistory.Dequeue();
            }
        }

        // Check if the player needs to be reset:
        if (skiRopeJoint == null && SceneManager.Instance.IsPlaying && !isRewinding)
        {
            isRewinding = true;
            resetPosition = thePositionHistory.Dequeue();

            theRope.SetActive(false);

            Invoke("DoRewind", rewindDelay);
        }
    }

    private void DoRewind()
    {
        isRewinding = false;

        // Reset the ship:
        this.gameObject.transform.SetPositionAndRotation(resetPosition.shipPosition, resetPosition.shipRotation);

        // Reset the skier:
        Destroy(theSkier.gameObject);

        theSkier = Instantiate<GameObject>(skierPrefab);
        theSkier.transform.SetPositionAndRotation(resetPosition.skierPosition, resetPosition.skierRotation);

        skiRopeJoint = theSkier.GetComponent<ConfigurableJoint>();
        skiRopeJoint.connectedBody = this.GetComponent<Rigidbody>();

        theSkier.GetComponent<SkierBehavior>().ropeObject = theRope;

        theSkier.GetComponentInChildren<SkierAIController>().playerShipTransform = this.transform;

        // Reset the rope:
        theRope.SetActive(true);
        RopeBehavior theRopeController = theRope.GetComponent<RopeBehavior>();
        theRopeController.skierRopeAttachPointTransform = theSkier.transform;

        Transform[] shipChildTransforms = this.GetComponentsInChildren<Transform>();
        foreach (Transform current in shipChildTransforms)
        {
            if (current.name == "PlayerShipRopeAttachPoint") // TO DO: Replace this with a (much faster) tag comparison
            {
                theRopeController.playerShipRopeAttachPointTransform = current;
                break;
            }
        }
    }
}
