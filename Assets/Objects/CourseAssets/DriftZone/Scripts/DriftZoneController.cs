using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DriftZoneController : MonoBehaviour {
    [Header("Points:")]
    [Tooltip("How many points to award per second")]
    public int pointsPerSecond = 500;

    [Header("UI:")]
    [Tooltip("Points text popup prefab")]
    public GameObject driftPointText;
    private GameObject pointsPopup;

    [Tooltip("The scene's main canvas")]
    public Canvas mainCanvas;
    private RectTransform mainCanvasRectTransform;

    [Tooltip("How long the points points should remain visible, in seconds")]
    public float pointsPopupStayTime = 3.0f;

    private bool isScoringSkier;
    private bool isScoringShip;
    private float currentPoints;

    private Transform shipTransform;
    private Transform skierTransform;
    private Vector2 pointsLocation;
       

	// Use this for initialization
	void Start () {
        isScoringShip = false;
        isScoringSkier = false;

        currentPoints = 0.0f;

        mainCanvasRectTransform = mainCanvas.GetComponent<RectTransform>();

        shipTransform = null;
        skierTransform = null;
        pointsLocation = Vector2.zero;
    }
	
	// Update is called once per frame
	void Update () {
        if (isScoringShip && isScoringSkier)
        {
            currentPoints += (float)pointsPerSecond * Time.deltaTime;

            if (pointsPopup)
            {
                Destroy(pointsPopup);
            }

            pointsPopup = Instantiate<GameObject>(driftPointText, mainCanvas.transform);
        }

        if (pointsPopup) {

            //Vector2 pointsLocation = new Vector2(this.transform.position.x, this.transform.position.y);
            if (skierTransform && shipTransform)
            {
                pointsLocation = new Vector2((this.skierTransform.position.x + this.shipTransform.position.x) / 2, (this.skierTransform.position.y + this.shipTransform.position.y) / 2);
                // TO DO: ^^^^^^ Replace this with Vector2.Lerp(,,0.5f);
                pointsLocation = RectTransformUtility.WorldToScreenPoint(mainCanvas.worldCamera, pointsLocation);

                Vector2 hoverPoint = new Vector2();
                RectTransformUtility.ScreenPointToLocalPointInRectangle(mainCanvasRectTransform, pointsLocation, mainCanvas.worldCamera, out hoverPoint);

                pointsPopup.GetComponent<RectTransform>().anchoredPosition = hoverPoint;

                pointsPopup.GetComponent<Text>().text = Mathf.Round(currentPoints).ToString();

                //GrowPopupText();
            }
            
        }

    }

    //IEnumerator GrowPopupText()
    //{
    //    Debug.Log("entered!");
        
    //    for (int i = pointsPopup.GetComponent<Text>().fontSize; i < 100; i++)
    //    {
    //        if (pointsPopup)
    //        {
    //            Debug.Log(i);
    //            pointsPopup.GetComponent<Text>().fontSize = i * 10; // TO DO: Store this instead of getting the component every time here and in update!!!
                
    //        }
    //        yield return null;
    //    }
    //}

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
            GameManager.Instance.AddPoints((int)Mathf.Round(currentPoints));

        shipTransform = null;

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
            GameManager.Instance.AddPoints((int)Mathf.Round(currentPoints));

        skierTransform = null;

        Invoke("RemovePointsPopup", pointsPopupStayTime);
    }

    private void RemovePointsPopup()
    {
        currentPoints = 0.0f;
        Destroy(pointsPopup);
    }
}
