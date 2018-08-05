using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DriftZoneController : SkierInteractionZoneBehavior {

    private bool isScoringShip;

    private Transform shipTransform;
    private Transform skierTransform;

    private Vector2 worldSpacePointsLocation; // These allow us to blend the points location instead of a hard stop once the scoring phase is over
    private float worldSpaceLerp = 0.5f;

    // Use this for initialization
    new void Start () {
        base.Start();

        isScoringShip = false;

        shipTransform = null;
        skierTransform = null;
        pointsLocation = Vector2.zero;
    }

	// Update is called once per frame
	void Update () {
        if (isScoringShip && isScoringSkier)
        {
            currentPoints += (float)pointSpeedFactor * Time.deltaTime * shipTransform.gameObject.GetComponentInParent<Rigidbody>().velocity.magnitude * skierTransform.gameObject.GetComponent<Rigidbody>().velocity.magnitude;

            if (pointsPopup)
            {
                Destroy(pointsPopup);
            }

            pointsPopup = Instantiate<GameObject>(pointsPopupText, mainCanvas.transform);


            if (skierTransform && shipTransform)
            {
                pointsLocation = Vector2.Lerp(this.skierTransform.position, this.shipTransform.position, 0.5f);
                worldSpacePointsLocation = pointsLocation;
                pointsLocation = RectTransformUtility.WorldToScreenPoint(mainCanvas.worldCamera, pointsLocation);

                Vector2 hoverPoint = new Vector2();
                RectTransformUtility.ScreenPointToLocalPointInRectangle(mainCanvasRectTransform, pointsLocation, mainCanvas.worldCamera, out hoverPoint);

                pointsPopup.GetComponent<RectTransform>().anchoredPosition = hoverPoint;

                pointsPopup.GetComponent<Text>().text = Mathf.Round(currentPoints).ToString();
            }


            pointsPopup.transform.rotation = Camera.main.transform.rotation; // Maintain orientation with the camera at all times
        }
        else if (pointsPopup && skierTransform && shipTransform)
        {
            Vector2 blendedLocation = Vector2.Lerp(worldSpacePointsLocation, Vector2.Lerp(this.skierTransform.position, this.shipTransform.position, 0.5f), worldSpaceLerp);
            worldSpaceLerp = Mathf.Clamp(worldSpaceLerp - (0.1f * Time.deltaTime), 0, 1);
            blendedLocation = RectTransformUtility.WorldToScreenPoint(mainCanvas.worldCamera, blendedLocation);

            Vector2 hoverPoint = new Vector2();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(mainCanvasRectTransform, blendedLocation, mainCanvas.worldCamera, out hoverPoint);

            pointsPopup.GetComponent<RectTransform>().anchoredPosition = hoverPoint;
            pointsPopup.transform.rotation = Camera.main.transform.rotation; // Maintain orientation with the camera at all times
            
        }

        // Destroy the  popup if the skier has died
        if (pointsPopup && !SceneManager.Instance.IsPlaying)
        {
            Destroy(pointsPopup);
        }
    }



    // Note: newShipTransform is assumed to be the ship transform
    public void OnShipZoneEnter(Transform newShipTransform)
    {
        isScoringShip = true;
        shipTransform = newShipTransform;
    }

    //public void OnShipZoneStay()
    //{

    //}

    // Note: It is assumed we've already checked that it is, in fact, the ship that is leaving the trigger area
    public void OnShipZoneExit()
    {
        isScoringShip = false;

        if (isScoringSkier)
            SceneManager.Instance.AddPoints((int)Mathf.Round(currentPoints));

        //shipTransform = null;

        Invoke("RemovePointsPopup", pointsPopupStayTime);
    }

    // Note: newShipTransform is assumed to be the skier transform
    public void OnSkierZoneEnter(Transform newSkierTransform)
    {
        isScoringSkier = true;
        skierTransform = newSkierTransform;
    }

    //public void OnSkierZoneStay()
    //{

    //}

    // Note: It is assumed we've already checked that it is, in fact, the skier that is leaving the trigger area
    public void OnSkierZoneExit()
    {
        isScoringSkier = false;

        if (isScoringShip)
            SceneManager.Instance.AddPoints((int)Mathf.Round(currentPoints));

        //skierTransform = null;

        Invoke("RemovePointsPopup", pointsPopupStayTime);
    }
}
