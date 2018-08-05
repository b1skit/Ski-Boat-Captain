using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JumpBehavior : SkierInteractionZoneBehavior {

    [Tooltip("How many points to award for completing this jump")]
    public int points = 100;

    [Tooltip("A trigger volume that encloses the entire jump area. Used for enabling/disabling skier & boat water spray")]
    public BoxCollider jumpArea;

    private Transform skierTransform;

    private bool hasLaunched;
    private bool hasLanded;


    new private void Start()
    {
        base.Start();

        hasLaunched = hasLanded = false;
    }

    private void Update()
    {
        if (pointsPopup)
        {
            pointsLocation = Vector3.Lerp(skierTransform.position, this.gameObject.transform.position, 0.5f);
            pointsLocation = RectTransformUtility.WorldToScreenPoint(mainCanvas.worldCamera, pointsLocation);

            Vector2 hoverPoint = new Vector2();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(mainCanvasRectTransform, pointsLocation, mainCanvas.worldCamera, out hoverPoint);

            pointsPopup.GetComponent<RectTransform>().anchoredPosition = hoverPoint;
            pointsPopup.transform.rotation = Camera.main.transform.rotation;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Skier") || other.gameObject.CompareTag("Player"))
        {
            TrailRenderer[] wakeTrails = other.gameObject.GetComponentsInChildren<TrailRenderer>();
            foreach (TrailRenderer current in wakeTrails)
            {
                current.enabled = false;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Skier") || other.gameObject.CompareTag("Player"))
        {
            TrailRenderer[] wakeTrails = other.gameObject.GetComponentsInChildren<TrailRenderer>();
            foreach (TrailRenderer current in wakeTrails)
            {
                current.enabled = true;
            }

            // Score popup:
            if (other.gameObject.CompareTag("Skier") && hasLaunched && hasLanded)
            {
                if (pointsPopup)
                {
                    Destroy(pointsPopup);
                }

                pointsPopup = Instantiate<GameObject>(pointsPopupText, mainCanvas.transform);

                pointsLocation = Vector3.Lerp(skierTransform.position, this.gameObject.transform.position, 0.5f);
                pointsLocation = RectTransformUtility.WorldToScreenPoint(mainCanvas.worldCamera, pointsLocation);

                Vector2 hoverPoint = new Vector2();
                RectTransformUtility.ScreenPointToLocalPointInRectangle(mainCanvasRectTransform, pointsLocation, mainCanvas.worldCamera, out hoverPoint);

                pointsPopup.GetComponent<RectTransform>().anchoredPosition = hoverPoint;

                pointsPopup.GetComponent<Text>().text = points.ToString();
                SceneManager.Instance.AddPoints(points);

                Destroy(pointsPopup, pointsPopupStayTime);
            }
            
            hasLaunched = hasLanded = false;
        }
    }

    public void OnLaunchRampEntry()
    {
        hasLaunched = true;
    }

    public void OnLandingRampEntry(Transform newSkierTransform)
    {
        skierTransform = newSkierTransform;
        hasLanded = true;
    }
}
