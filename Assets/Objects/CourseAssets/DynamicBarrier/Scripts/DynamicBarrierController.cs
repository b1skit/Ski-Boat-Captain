using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class DynamicBarrierController : MonoBehaviour {

    [Header("Dynamic barrier meshes:")]

    [Tooltip("The main pontoon buoy at the end of every dynamic barrier")]
    public GameObject startBuoy;

    [Tooltip("Should the last mesh placed be a repeat of the start buoy?")]
    public bool endWithStartBuoy = false;

    [Tooltip("The smaller filler buoys that extend between the main pontoons")]
    public GameObject fillerBuoy;

    [Tooltip("The distance between each of the dynamically spawned meshes")]
    public float objectSpacing = 1.5f;


    [Header("Script objects (These should be pre-configured in the prefab, with no reason to change them)")]


    [Tooltip("The end position of the dynamically generated barrier")]
    public Transform endPoint;

    [Tooltip("The child object under which all dynamic objects are parented. This object should not be manually modified")]
    public GameObject DynamicObjectsGroup;

    [Tooltip("The child component containing the box collider")]
    public BoxCollider theBoxCollider;

    [Header("Dynamic collision settings:")]

    public float boxColliderYHeight = 1.0f;
    public float boxColliderZHeight = 10.0f;

    [Tooltip("Extra units of length to add to the box collider. Should be the width of the end buoy. Compensates for the end buoys overhaning the start/end positions")]
    public float boxColliderAdditionalLength = 1f;

	
	// Update is called once per frame
	void Update () {
        
        foreach(Transform current in DynamicObjectsGroup.GetComponentsInChildren<Transform>())
        {
            if (current && current != DynamicObjectsGroup.transform)
                DestroyImmediate(current.gameObject);
        }

        // Add the starting buoy:
        GameObject theStartBuoy = Instantiate<GameObject>(startBuoy, this.transform.position, new Quaternion(), DynamicObjectsGroup.transform);

        // Add the filler buoys:
        Vector3 barrierDirection = (endPoint.position - this.transform.position).normalized;
        float barrierLength = (endPoint.position - this.transform.position).magnitude;

        // Average out the object spacing length by adding an increment of the remainder to the spacing
        int numObjects = (int)Mathf.Floor(barrierLength / objectSpacing);
        float delta = barrierLength - (objectSpacing * numObjects);
        float averagedSpacing = objectSpacing + delta/numObjects;

        float placementPosition = averagedSpacing;

        while (placementPosition < barrierLength)
        {
            Vector3 objectSpawn = this.transform.position + (barrierDirection * placementPosition);

            // Check if we're placing a filler buoy, or ending with a repeat of the start buoy
            if (endWithStartBuoy && (placementPosition + averagedSpacing) > barrierLength)
            {
                GameObject newFillerBuoy = Instantiate<GameObject>(startBuoy, objectSpawn, new Quaternion(), DynamicObjectsGroup.transform);
            }
            else
            {
                GameObject newFillerBuoy = Instantiate<GameObject>(fillerBuoy, objectSpawn, new Quaternion(), DynamicObjectsGroup.transform);
            }
            placementPosition += averagedSpacing;
        }

        theBoxCollider.transform.localRotation = Quaternion.FromToRotation(this.transform.right, endPoint.position - this.transform.position);
        theBoxCollider.size = new Vector3((endPoint.position - this.transform.position).magnitude + boxColliderAdditionalLength, boxColliderYHeight, boxColliderZHeight);
        theBoxCollider.transform.localPosition = Vector3.Lerp(Vector3.zero, endPoint.localPosition, 0.5f);
    }
}
