using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpBehavior : SkierInteractionZoneBehavior {

    [Tooltip("A trigger volume that encloses the entire jump area. Used for enabling/disabling skier & boat water spray")]
    public BoxCollider jumpArea;

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
        }
    }
}
