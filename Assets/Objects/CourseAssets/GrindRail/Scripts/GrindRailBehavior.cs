﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// A generic parent class for objects with skier zones that have explicit entry and exit points
public class SkierInteractionZoneBehavior : MonoBehaviour
{
    [Tooltip("The skier entry point transform")]
    public Transform skierEntryPosition;

    [Tooltip("The skier exit point transform")]
    public Transform skierExitPosition;

    // Returns 2 transform positions: [0] = entry position, [1] = exit position
    public Vector3[] GetEntryExitPositions()
    {
        Vector3[] entryExitPositions = new Vector3[2];
        entryExitPositions[0] = skierEntryPosition.transform.position;
        entryExitPositions[1] = skierExitPosition.transform.position;

        return entryExitPositions;
    }
}

public class GrindRailBehavior : SkierInteractionZoneBehavior {

    [Header("Points:")]
    [Tooltip("How much to scale the awarded points, which are calculated as skier.velocity.magnitude * ship.velocity.magnitude per second")]
    public float pointSpeedFactor = 1.0f;

    [Header("UI:")]
    [Tooltip("Points text popup prefab")]
    public GameObject grindPointsText;
    private GameObject pointsPopup;

    [Tooltip("The scene's main canvas")]
    public Canvas mainCanvas;
    private RectTransform mainCanvasRectTransform;

    [Tooltip("How long the points points should remain visible, in seconds")]
    public float pointsPopupStayTime = 3.0f;

    [Header("Interaction settings:")]

    [Tooltip("The Z height that the skier should grind at when interacting with this object (will be negative, assuming camera is looking down Z+)")]
    public float grindHeight = -3.0f;

    private bool isScoringSkier;
    private float currentPoints;

    private Transform shipTransform;
    private Transform skierTransform;
    private Vector2 pointsLocation;

    // Use this for initialization
    void Start () {
        isScoringSkier = false;
        currentPoints = 0.0f;

        mainCanvasRectTransform = mainCanvas.GetComponent<RectTransform>();

        shipTransform = null;
        skierTransform = null;
        pointsLocation = Vector2.zero;
    }
	
	// Update is called once per frame
	void Update () {
        if (isScoringSkier)
        {
            currentPoints += pointSpeedFactor * Time.deltaTime * shipTransform.gameObject.GetComponentInParent<Rigidbody>().velocity.magnitude * skierTransform.gameObject.GetComponent<Rigidbody>().velocity.magnitude;

            if (pointsPopup)
            {
                Destroy(pointsPopup);
            }

            pointsPopup = Instantiate<GameObject>(grindPointsText, mainCanvas.transform);
        }

        if (pointsPopup)
        {
            if (skierTransform && shipTransform)
            {
                pointsLocation = Vector2.Lerp(this.skierTransform.position, this.shipTransform.position, 0.5f);
                pointsLocation = RectTransformUtility.WorldToScreenPoint(mainCanvas.worldCamera, pointsLocation);

                Vector2 hoverPoint = new Vector2();
                RectTransformUtility.ScreenPointToLocalPointInRectangle(mainCanvasRectTransform, pointsLocation, mainCanvas.worldCamera, out hoverPoint);

                pointsPopup.GetComponent<RectTransform>().anchoredPosition = hoverPoint;

                pointsPopup.GetComponent<Text>().text = Mathf.Round(currentPoints).ToString();
            }

            // Destroy the  popup if the skier has died
            if (!SceneManager.instance.IsPlaying)
                Destroy(pointsPopup);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.CompareTag("Skier") && SceneManager.instance.IsPlaying)
        {
            other.gameObject.GetComponent<Rigidbody>().useGravity = false;
            other.gameObject.transform.position = new Vector3(other.gameObject.transform.position.x, other.gameObject.transform.position.y, grindHeight);
            //other.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.back * 30,ForceMode.Impulse);
        }
    }

    // Sets the skier's velocity to the right/X axis of this object's transform, with the same magnitude
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Skier") && SceneManager.instance.IsPlaying)
        {
            isScoringSkier = true;

            skierTransform = other.gameObject.transform;
            shipTransform = other.gameObject.GetComponent<ConfigurableJoint>().connectedBody.transform;

            Rigidbody otherRigidBody = other.GetComponent<Rigidbody>();




            otherRigidBody.velocity = this.gameObject.transform.right * otherRigidBody.velocity.magnitude;
            //otherRigidBody.velocity = (this.gameObject.transform.right * otherRigidBody.velocity.magnitude) + new Vector3(0, 0, otherRigidBody.velocity.z);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Skier") && SceneManager.instance.IsPlaying)
        {
            isScoringSkier = false;

            GameManager.Instance.AddPoints((int)Mathf.Round(currentPoints));

            skierTransform = null;
            shipTransform = null;

            Invoke("RemovePointsPopup", pointsPopupStayTime);

            other.gameObject.GetComponent<Rigidbody>().useGravity = true;
        }            
    }

    private void RemovePointsPopup()
    {
        currentPoints = 0.0f;
        Destroy(pointsPopup);
    }
}
