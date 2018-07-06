using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkierAIController : MonoBehaviour {

    [Tooltip("The main rigidbody controlling the skier")]
    public Rigidbody skierRigidbody;

    private GameObject currentTarget;

    private float dotLimit = -0.5f; // Minimum dot product result to consider a target "in front" of the skier

    // Use this for initialization
    void Start () {
        currentTarget = null;
	}

    private void FixedUpdate()
    {
        if (currentTarget)
        {
            float dotResult = Vector3.Dot((currentTarget.transform.position - this.gameObject.transform.position).normalized, this.transform.right);
            if (dotResult >= dotLimit)
            {
                float angleFactor = 0.5f + (1 - dotResult);

                skierRigidbody.AddForce((currentTarget.transform.position - this.transform.position).normalized * skierRigidbody.velocity.magnitude * angleFactor * Time.fixedDeltaTime, ForceMode.VelocityChange);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.instance.IsPlaying && skierRigidbody.velocity.normalized != Vector3.zero)
            this.transform.rotation = Quaternion.LookRotation(Vector3.Cross(skierRigidbody.velocity.normalized, Vector3.forward), Vector3.forward);
    }

    private void OnTriggerStay(Collider other)
    {
        // Ensure the potential target is a collectable, and is in front of the skier
        if (other.gameObject.tag == "Collectable" && Vector3.Dot((other.gameObject.transform.position - this.gameObject.transform.position).normalized, this.transform.right) >= dotLimit)
        {
            float currentTargetDistance;
            float potentialTargetDistance;
            
            // Check: Is the potental target closer? If so, set it as the new target
            if (currentTarget)
            {
                currentTargetDistance = Vector3.Distance(this.transform.position, currentTarget.transform.position);
                potentialTargetDistance = Vector3.Distance(this.transform.position, other.transform.position);

                if (potentialTargetDistance < currentTargetDistance)
                {
                    currentTarget = other.gameObject;
                }
            }
            else
            {
                currentTarget = other.gameObject;
            }
        } // End collectable check
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == currentTarget)
        {
            currentTarget = null;
        }
    }
}
