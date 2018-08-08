using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GrindRailBehavior : SkierMovingInteractionZoneBehavior
{
    [Header("Interaction settings:")]

    [Tooltip("The Z height that the skier should grind at when interacting with this object (will be negative, assuming camera is looking down Z+). Note: WILL influence rope/breaking")]
    public float grindHeight = -0.2f;


    // Use this for initialization
    new void Start()
    {
        base.Start();
    }


    // Update is called once per frame
    void Update () {
        if (isScoringSkier)
        {
            currentPoints += (float)pointSpeedFactor * Time.deltaTime * shipTransform.gameObject.GetComponentInParent<Rigidbody>().velocity.magnitude * skierTransform.gameObject.GetComponent<Rigidbody>().velocity.magnitude;

            if (pointsPopup)
            {
                Destroy(pointsPopup);
            }

            pointsPopup = Instantiate<GameObject>(pointsPopupText, mainCanvas.transform);
            pointsPopup.transform.rotation = Camera.main.transform.rotation; // Maintain orientation with the camera at all times

            if (skierTransform && shipTransform)
            {
                ShowPointsWhileScoringAndTransforms();
            }
        }
        else if (pointsPopup && skierTransform)
        {
            BlendPopupPosition();
        }

        // Destroy the  popup if the skier has died
        if (pointsPopup && !SceneManager.Instance.IsPlaying)
        {
            Destroy(pointsPopup);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.CompareTag("Skier") && SceneManager.Instance.IsPlaying)
        {
            other.gameObject.GetComponent<Rigidbody>().useGravity = false;
            other.gameObject.transform.position = new Vector3(other.gameObject.transform.position.x, other.gameObject.transform.position.y, grindHeight);
        }
    }


    // Sets the skier's velocity to the right/X axis of this object's transform, with the same magnitude
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Skier") && SceneManager.Instance.IsPlaying)
        {
            isScoringSkier = true;

            skierTransform = other.gameObject.transform;
            shipTransform = other.gameObject.GetComponent<ConfigurableJoint>().connectedBody.transform;

            Rigidbody otherRigidBody = other.GetComponent<Rigidbody>();

            otherRigidBody.velocity = this.gameObject.transform.right * otherRigidBody.velocity.magnitude;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Skier") && SceneManager.Instance.IsPlaying)
        {
            isScoringSkier = false;

            SceneManager.Instance.AddPoints((int)Mathf.Round(currentPoints));

            worldSpaceLerp = 0.5f;

            Invoke("RemovePointsPopup", pointsPopupStayTime);

            other.gameObject.GetComponent<Rigidbody>().useGravity = true;
        }            
    }
}
