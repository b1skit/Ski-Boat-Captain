using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DriftZoneController : SkierMovingInteractionZoneBehavior {

    private bool isScoringShip;


    // Use this for initialization
    new void Start()
    {
        base.Start();

        isScoringShip = false;
    }


    // Update is called once per frame
    new void Update () {
        base.Update();

        if (isScoringShip && isScoringSkier)
        {
            if (pointsPopup)
            {
                Destroy(pointsPopup);
            }

            if (skierTransform && shipTransform)
            {
                currentPoints += (float)pointSpeedFactor * Time.deltaTime * shipTransform.gameObject.GetComponentInParent<Rigidbody>().velocity.magnitude * skierTransform.gameObject.GetComponent<Rigidbody>().velocity.magnitude;

                ShowPointsWhileScoringAndTransforms();
            }
        }
        else if (pointsPopup && skierTransform && shipTransform)
        {
            BlendPopupPosition();
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

        worldSpaceLerp = 0.5f;

        Invoke("RemovePointsPopup", pointsPopupStayTime);
    }
}
