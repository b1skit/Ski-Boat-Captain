#define VISUAL_DEBUG // Display a visual debug marker if enabled

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SkierAIController : MonoBehaviour {

    [Tooltip("The rigidbody controlling the skier")]
    public Rigidbody skierRigidbody;

    [Tooltip("The transform of the player ship")]
    public Transform playerShipTransform;

    private GameObject currentTargetObject;
    private Vector3 currentTargetPosition;

    private float dotLimit = -0.5f; // Minimum dot product result to consider a target "in front" of the skier

#if VISUAL_DEBUG
    public GameObject debugTarget;
    private GameObject theDebugTarget;
#endif

    // Use this for initialization
    void Start () {
        currentTargetObject = null;
    }   

    private void FixedUpdate()
    {
        if (currentTargetObject)
        {
            // POTENTIAL BUG HERE: Might miss target if it's close, but FACING the wrong direction(b/c using transform.right)!! Instead, aim at things "ahead" of skier regardless of its facing
            // ie. Use a vector from skier to boat?
            float fwdCheckDotResult = Vector3.Dot((currentTargetPosition - this.gameObject.transform.position).normalized, this.transform.right); // Replacee transform.right??
            if (fwdCheckDotResult >= dotLimit)
            {
                float angleFactor = 0.5f + (1 - fwdCheckDotResult);

                // Aim "behind" the current target's position:
                float leadAmount = 0.5f;
                Vector3 leadingPosition = currentTargetPosition - ( (playerShipTransform.position - skierRigidbody.transform.position).normalized * (currentTargetPosition - skierRigidbody.transform.position).magnitude * leadAmount);

                skierRigidbody.AddForce((leadingPosition - this.transform.position).normalized * skierRigidbody.velocity.magnitude * angleFactor * Time.fixedDeltaTime, ForceMode.VelocityChange);

#if VISUAL_DEBUG 
                if (theDebugTarget)
                    Destroy(theDebugTarget);

                theDebugTarget = Instantiate<GameObject>(debugTarget);
                theDebugTarget.transform.position = leadingPosition;
                theDebugTarget.gameObject.GetComponent<MeshRenderer>().material.color = new Color(0, 1, 0);
#endif
            }

#if VISUAL_DEBUG
            else
            {
                if (theDebugTarget)
                    theDebugTarget.gameObject.GetComponent<MeshRenderer>().material.color = new Color(1, 0, 0);
            }
#endif
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Update the view model's direction:
        if (SceneManager.instance.IsPlaying && skierRigidbody.velocity.normalized != Vector3.zero)
            this.transform.rotation = Quaternion.LookRotation(Vector3.Cross(skierRigidbody.velocity.normalized, Vector3.forward), Vector3.forward); 
    }

    // Target things in the trigger zone
    private void OnTriggerStay(Collider other)
    {
        // Ensure the potential target is a collectable, and is in front of the skier
        if (other.gameObject.CompareTag("Collectable") || other.gameObject.CompareTag("Rideable"))
        {
            // Handle objects with specific entry/exit positions: Decide which transform to target
            Vector3 otherPosition;
            if (other.gameObject.CompareTag("Rideable"))
            {
                Vector3[] entryExitPoints = other.gameObject.GetComponent<SkierInteractionZoneBehavior>().GetEntryExitPositions();

                // Check if the start, middle, or exit is in front: Choose the first one that is, or the end point
                if (Vector3.Dot((entryExitPoints[0] - this.gameObject.transform.position).normalized, this.transform.right) >= dotLimit)
                    otherPosition = entryExitPoints[0];
                else if (Vector3.Dot((other.transform.position - this.gameObject.transform.position).normalized, this.transform.right) >= dotLimit)
                    otherPosition = other.transform.position;
                else
                    otherPosition = entryExitPoints[1];
            }
            else // If object is not a rideable, then just use its main transform
                otherPosition = other.transform.position;


            if (Vector3.Dot((otherPosition - this.gameObject.transform.position).normalized, this.transform.right) >= dotLimit)
            {
                // Check: Is the potental target closer? If so, set it as the new target
                if (currentTargetObject)
                {
                    float currentTargetDistance = Vector3.Distance(this.transform.position, currentTargetPosition);
                    float potentialTargetDistance = Vector3.Distance(this.transform.position, otherPosition);

                    if (potentialTargetDistance < currentTargetDistance)
                    {
                        currentTargetObject = other.gameObject;
                        currentTargetPosition = otherPosition;
                    }
                }
                else // If we don't have a current target, set this object as the target
                {
                    currentTargetObject = other.gameObject;
                    currentTargetPosition = otherPosition;
                }
            }
            
        } // End collectable check
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == currentTargetObject)
        {
            currentTargetObject = null;
        }
    }
}
