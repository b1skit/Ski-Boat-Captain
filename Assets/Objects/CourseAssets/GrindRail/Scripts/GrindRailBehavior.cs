using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GrindRailBehavior : SkierInteractionZoneBehavior {

    [Header("Interaction settings:")]

    [Tooltip("The Z height that the skier should grind at when interacting with this object (will be negative, assuming camera is looking down Z+). Note: WILL influence rope/breaking")]
    public float grindHeight = -0.2f;

    private Transform shipTransform;
    private Transform skierTransform;

    // Use this for initialization
    new void Start () {
        base.Start();

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

            pointsPopup = Instantiate<GameObject>(pointsPopupText, mainCanvas.transform);
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
                pointsPopup.transform.rotation = Camera.main.transform.rotation;

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
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Skier") && SceneManager.instance.IsPlaying)
        {
            isScoringSkier = false;

            SceneManager.instance.AddPoints((int)Mathf.Round(currentPoints));

            skierTransform = null;
            shipTransform = null;

            Invoke("RemovePointsPopup", pointsPopupStayTime);

            other.gameObject.GetComponent<Rigidbody>().useGravity = true;
        }            
    }
}
