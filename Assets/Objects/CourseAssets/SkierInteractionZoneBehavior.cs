﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    protected Canvas mainCanvas;
    protected RectTransform mainCanvasRectTransform;

    [Tooltip("How long the points points should remain visible, in seconds")]
    public float pointsPopupStayTime = 3.0f;

    [Header("Entry/Exit targets:")]

    [Tooltip("Skier AI entry/exit target positions. n transforms MUST be supplied with entry index 0...exit in index (n-1)")]
    public Transform[] skierTargetPositions;

    protected float currentPoints;
    protected bool isScoringSkier;

    protected Vector2 pointsLocation;

    public void Start()
    {
        isScoringSkier = false;
        currentPoints = 0.0f;

        Canvas[] allCanvas = Resources.FindObjectsOfTypeAll<Canvas>();
        foreach (Canvas current in allCanvas)
        {
            if (current.gameObject.scene == UnityEngine.SceneManagement.SceneManager.GetActiveScene())
            {
                mainCanvas = current;
            }
        }

        mainCanvasRectTransform = mainCanvas.GetComponent<RectTransform>();
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
