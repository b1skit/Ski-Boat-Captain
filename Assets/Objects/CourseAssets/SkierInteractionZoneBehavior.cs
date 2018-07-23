using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// A generic parent class for objects with skier zones that have explicit entry and exit points
public abstract class SkierInteractionZoneBehavior : MonoBehaviour
{
    [Header("Entry/Exit targets:")]
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

