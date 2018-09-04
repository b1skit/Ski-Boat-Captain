using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public abstract class SkierMovingInteractionZoneBehavior : SkierInteractionZoneBehavior
{
    protected Transform shipTransform;

    protected Vector2 worldSpacePointsLocation; // These allow us to blend the points location instead of a hard stop once the scoring phase is over
    protected float worldSpaceLerp = 0.5f;


    // Use this for initialization
    new void Start()
    {
        base.Start();

        shipTransform = null;
        skierTransform = null;
        pointsLocation = Vector2.zero;
    }

    public new void Update()
    {
        base.Update();
    }


    protected void ShowPointsWhileScoringAndTransforms()
    {
        if (Time.timeScale != 0)
        {
            pointsPopup = Instantiate<GameObject>(pointsPopupText, mainCanvas.transform);

            pointsLocation = Vector2.Lerp(this.skierTransform.position, this.shipTransform.position, 0.5f);
            worldSpacePointsLocation = pointsLocation;
            pointsLocation = RectTransformUtility.WorldToScreenPoint(mainCanvas.worldCamera, pointsLocation);

            Vector2 hoverPoint = new Vector2();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(mainCanvasRectTransform, pointsLocation, null, out hoverPoint);

            pointsPopup.GetComponent<RectTransform>().anchoredPosition = hoverPoint;

            pointsPopup.GetComponent<Text>().text = Mathf.Round(currentPoints).ToString();
        }
    }


    protected void BlendPopupPosition()
    {
        Vector2 blendedLocation = Vector2.Lerp(worldSpacePointsLocation, Vector2.Lerp(this.skierTransform.position, this.shipTransform.position, 0.5f), worldSpaceLerp);
        worldSpaceLerp = Mathf.Clamp(worldSpaceLerp - (0.1f * Time.deltaTime), 0, 1);
        blendedLocation = RectTransformUtility.WorldToScreenPoint(mainCanvas.worldCamera, blendedLocation);

        Vector2 hoverPoint = new Vector2();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(mainCanvasRectTransform, blendedLocation, null, out hoverPoint);

        pointsPopup.GetComponent<RectTransform>().anchoredPosition = hoverPoint;
    }
}


// A generic parent class for objects with skier zones that have explicit entry and exit points
public abstract class SkierInteractionZoneBehavior : MonoBehaviour
{
    [Header("Point Settings:")]

    [Tooltip("How much to scale the awarded points, which are calculated as skier.velocity.magnitude * ship.velocity.magnitude per second")]
    public float pointSpeedFactor = 1.0f;

    [Header("UI:")]

    [Tooltip("Points text popup prefab")]
    public GameObject pointsPopupText;
    protected GameObject pointsPopup;

    [Tooltip("The in-game HUD UI Canvas")]
    public Canvas mainCanvas;
    protected RectTransform mainCanvasRectTransform;

    [Tooltip("How long the points points should remain visible, in seconds")]
    public float pointsPopupStayTime = 3.0f;

    [Header("Entry/Exit targets:")]

    [Tooltip("Skier AI entry/exit target positions. n transforms MUST be supplied with entry index 0...exit in index (n-1)")]
    public Transform[] skierTargetPositions;

    protected float currentPoints;
    protected bool isScoringSkier;

    protected Transform skierTransform;

    protected Vector2 pointsLocation;
    

    public void Start()
    {
        isScoringSkier = false;
        currentPoints = 0.0f;

        mainCanvasRectTransform = mainCanvas.GetComponent<RectTransform>();
    }


    public void Update()
    {
        if (pointsPopup && (!SceneManager.Instance.IsPlaying || Time.timeScale == 0))
        {
            Destroy(pointsPopup);
        }
    }


    // Returns 2 transform positions: [0] = entry position, [1] = exit position
    public Vector3[] GetEntryExitPositions()
    {
        Vector3[] entryExitPositions = new Vector3[skierTargetPositions.Length];
        for (int i = 0; i < skierTargetPositions.Length; i++)
        {
            entryExitPositions[i] = skierTargetPositions[i].position;
        }
        return entryExitPositions;
    }


    private void RemovePointsPopup()
    {
        currentPoints = 0.0f;
        Destroy(pointsPopup);
    }
}

