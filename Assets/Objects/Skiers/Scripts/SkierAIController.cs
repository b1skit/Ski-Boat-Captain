#define VISUAL_DEBUG // Display a visual debug marker (uncomment to enable, comment to disable)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SkierAIController : MonoBehaviour {

    [Tooltip("The rigidbody controlling the skier")]
    public Rigidbody skierRigidbody;

    [Tooltip("The transform of the player ship")]
    public Transform playerShipTransform;

    [Tooltip("The transform of the skier's view model. Used to rotate/orientate the viewmesh independently of the rigidbody")]
    public Transform skierViewModelTransform;

    [Tooltip("Strength of the boost applied when the skier turns towards a new target")]
    public float targetBoostPower = 250.0f;
    private bool hasBoosted;

    private GameObject currentTargetObject;
    private Vector3 currentTargetPosition;

    private float dotLimit = -0.25f; // Minimum dot product result to consider a target "in front" of the skier

    #if VISUAL_DEBUG
        public GameObject debugTarget;
        private GameObject theDebugTarget;
        private GameObject rigidBodyRight;
        private float viewDirectionOffset = 1.0f;
    #endif

    void Start () {
        currentTargetObject = null;
        hasBoosted = false;

        #if VISUAL_DEBUG
            rigidBodyRight = Instantiate(debugTarget, skierRigidbody.gameObject.transform);
            rigidBodyRight.transform.position += skierRigidbody.velocity.normalized * viewDirectionOffset;
            rigidBodyRight.GetComponent<MeshRenderer>().material.color = new Color(0, 0, 1);
        #endif
    }

    void Update()
    {
        // Update the view model's direction:
        if (SceneManager.instance.IsPlaying && skierRigidbody.velocity != Vector3.zero)
            skierViewModelTransform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.Cross(skierRigidbody.velocity.normalized, Vector3.back));
    }

    private void FixedUpdate()
    {
        if (currentTargetObject && SceneManager.instance.IsPlaying) // Ensure we're playing, to prevent skier from trying to move before the round starts
        {
            float fwdCheckDotResult = Vector3.Dot((currentTargetPosition - this.gameObject.transform.position).normalized, skierRigidbody.velocity.normalized);
            if (fwdCheckDotResult >= dotLimit)
            {
                float angleFactor = 0.5f + (1 - fwdCheckDotResult);

                // Aim "behind" the current target's position:
                float leadAmount = 0.5f;
                Vector3 leadingPosition = currentTargetPosition - ( (playerShipTransform.position - skierRigidbody.transform.position).normalized * (currentTargetPosition - skierRigidbody.transform.position).magnitude * leadAmount);
                leadingPosition.z = skierRigidbody.position.z; // Aim horizontally (Ignore the Z position of the target transform)
                skierRigidbody.AddForce((leadingPosition - this.transform.position).normalized * skierRigidbody.velocity.magnitude * angleFactor * Time.fixedDeltaTime, ForceMode.VelocityChange);

                if (!hasBoosted) // Add a 1-time boost towards the target
                {
                    skierRigidbody.AddForce((leadingPosition - this.transform.position).normalized * targetBoostPower, ForceMode.Impulse);
                    hasBoosted = true;
                }

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

        #if VISUAL_DEBUG
        rigidBodyRight.transform.position = skierRigidbody.gameObject.transform.position + skierRigidbody.velocity.normalized * viewDirectionOffset;
        #endif
    }    

    // Target things in the trigger zone
    private void OnTriggerStay(Collider other)
    {
        // Ensure the potential target is a collectable, and is in front of the skier
        if (other.gameObject.CompareTag("Collectable") || other.gameObject.CompareTag("Rideable"))
        {
            // Handle objects with specific entry/exit positions: Select a transform position to base our targetting decision upon:
            Vector3 otherPosition;
            if (other.gameObject.CompareTag("Rideable"))
            {
                Vector3[] entryExitPoints = other.gameObject.GetComponent<SkierInteractionZoneBehavior>().GetEntryExitPositions();

                // Choose the first target position that is in front of the skier
                otherPosition = entryExitPoints[entryExitPoints.Length - 1]; // Default to the last possible target
                foreach (Vector3 current in entryExitPoints)
                {
                    if (Vector3.Dot((current - this.gameObject.transform.position).normalized, this.transform.right) >= dotLimit)
                    {
                        otherPosition = current;
                        break;
                    }
                }
            }
            else // Collectables: If object is not a rideable, then it must be a collectable and we just use its main transform
                otherPosition = other.transform.position;

            // Check if the selected position is in front of us:
            if (Vector3.Dot((otherPosition - this.gameObject.transform.position).normalized, skierRigidbody.velocity.normalized) >= dotLimit)
            {
                // Early out: If we don't have a current target and this new object is in front of us, set it as the current target
                if (!currentTargetObject)
                {
                    currentTargetObject = other.gameObject;
                    currentTargetPosition = otherPosition;
                    hasBoosted = false;
                    return;
                }
                else // We must already have a currentTargetObject:
                {
                    // Check: Is the potental target closer? If so, set it as the new target
                    Transform shipTransform = this.GetComponentInParent<ConfigurableJoint>().connectedBody.transform;
                    if (shipTransform)
                    {
                        float currentTargetDistance = Vector3.Distance(shipTransform.position, currentTargetPosition);
                        float potentialTargetDistance = Vector3.Distance(shipTransform.position, otherPosition);

                        if (potentialTargetDistance < currentTargetDistance || Vector3.Dot((currentTargetPosition - this.gameObject.transform.position).normalized, skierRigidbody.velocity.normalized) <= dotLimit)
                        {
                            currentTargetObject = other.gameObject;
                            currentTargetPosition = otherPosition;
                            hasBoosted = false;
                        }
                    }
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
