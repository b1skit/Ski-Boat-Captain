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
       

	// Use this for initialization
	void Start () {
        isScoringShip = false;
        isScoringSkier = false;

        currentPoints = 0.0f;

        mainCanvasRectTransform = mainCanvas.GetComponent<RectTransform>();
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
            Vector2 pointsLocation = new Vector2(this.transform.position.x, this.transform.position.y);
            pointsLocation = RectTransformUtility.WorldToScreenPoint(mainCanvas.worldCamera, pointsLocation);

            Vector2 hoverPoint = new Vector2();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(mainCanvasRectTransform, pointsLocation, mainCanvas.worldCamera, out hoverPoint);
            
            pointsPopup.GetComponent<RectTransform>().anchoredPosition = hoverPoint;

            pointsPopup.GetComponent<Text>().text = Mathf.Round(currentPoints).ToString();
        }

    }

    public void OnShipZoneEnter()
    {
        isScoringShip = true;
    }

    public void OnShipZoneStay()
    {

    }

    public void OnShipZoneExit()
    {
        isScoringShip = false;

        if (isScoringSkier)
            GameManager.instance.AddPoints((int)Mathf.Round(currentPoints));

        Invoke("RemovePointsPopup", pointsPopupStayTime);
    }

    public void OnSkierZoneEnter()
    {
        isScoringSkier = true;
    }

    public void OnSkierZoneStay()
    {

    }

    public void OnSkierZoneExit()
    {
        isScoringSkier = false;

        if (isScoringShip)
            GameManager.instance.AddPoints((int)Mathf.Round(currentPoints));

        Invoke("RemovePointsPopup", pointsPopupStayTime);
    }

    private void RemovePointsPopup()
    {
        currentPoints = 0.0f;
        Destroy(pointsPopup);
    }
}
