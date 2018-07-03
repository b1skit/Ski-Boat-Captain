﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrindRailBehavior : MonoBehaviour {

    [Header("Points:")]
    [Tooltip("How many points to award per second")]
    public int pointsPerSecond = 500;

    [Header("UI:")]
    [Tooltip("Points text popup prefab")]
    public GameObject grindPointsText;
    private GameObject pointsPopup;

    [Tooltip("The scene's main canvas")]
    public Canvas mainCanvas;
    private RectTransform mainCanvasRectTransform;

    [Tooltip("How long the points points should remain visible, in seconds")]
    public float pointsPopupStayTime = 3.0f;


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
            currentPoints += (float)pointsPerSecond * Time.deltaTime;

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
        }
    }

    private void OnTriggerStay(Collider other)
    {
        
        if (other.gameObject.tag == "Skier" && SceneManager.instance.IsPlaying)
        {
            isScoringSkier = true;

            skierTransform = other.gameObject.transform;
            shipTransform = other.gameObject.GetComponent<ConfigurableJoint>().connectedBody.transform;

            Vector3 skierVelocity = other.GetComponent<Rigidbody>().velocity;
            skierVelocity = this.gameObject.transform.right * skierVelocity.magnitude;
            other.GetComponent<Rigidbody>().velocity = skierVelocity; // IS  THIS NEEDED? PRETTY SURE WE PASS BY REFERENCE FOR CLASSES?
        }
    }

    private void OnTriggerExit(Collider other)
    {
        isScoringSkier = false;

        GameManager.Instance.AddPoints((int)Mathf.Round(currentPoints));

        skierTransform = null;
        shipTransform = null;

        Invoke("RemovePointsPopup", pointsPopupStayTime);
    }

    private void RemovePointsPopup()
    {
        currentPoints = 0.0f;
        Destroy(pointsPopup);
    }
}