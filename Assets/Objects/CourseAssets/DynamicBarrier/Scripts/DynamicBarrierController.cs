using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode, SelectionBase]
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

    [Header("Placement variation:")]

    [Tooltip("Max random offset value for each axis, +/- this value")]
    public Vector3 randomPositionOffset = new Vector3(0,0,0);

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

    // Only execute our dynamic placement if we're in the editor: This should NEVER run during gameplay
    #if (UNITY_EDITOR)

    // Update is called once per frame
    void Update () {

        if (Application.isEditor && !Application.isPlaying) // Another check to disable any updates during in-editor play mode
        {
            foreach (Transform current in DynamicObjectsGroup.GetComponentsInChildren<Transform>())
            {
                if (current && current != DynamicObjectsGroup.transform)
                {
                    DestroyImmediate(current.gameObject);
                }
            }

            Quaternion commonRotation = Quaternion.FromToRotation(this.transform.right, endPoint.position - this.transform.position);

            // Add the starting buoy:
            Instantiate<GameObject>(startBuoy, this.transform.position, commonRotation, DynamicObjectsGroup.transform);

            // Add the filler buoys:
            Vector3 barrierDirection = (endPoint.position - this.transform.position).normalized;
            float barrierLength = (endPoint.position - this.transform.position).magnitude;

            // Average out the object spacing length by adding an increment of the remainder to the spacing:
            int numObjects = (int)Mathf.Floor(barrierLength / objectSpacing);
            float delta = barrierLength - (objectSpacing * numObjects);
            float averagedSpacing = objectSpacing + delta / numObjects;

            float placementPosition = averagedSpacing;

            while (placementPosition < barrierLength)
            {
                Vector3 objectSpawn = this.transform.position + (barrierDirection * placementPosition)
                    + new Vector3(
                        Random.Range(-randomPositionOffset.x, randomPositionOffset.x),
                        Random.Range(-randomPositionOffset.y, randomPositionOffset.y),
                        Random.Range(-randomPositionOffset.z, randomPositionOffset.z)
                        );

                // Check if we're placing a filler buoy, or ending with a repeat of the start buoy
                if (endWithStartBuoy && (placementPosition + averagedSpacing) > barrierLength) // Placing end object:
                {
                    Instantiate<GameObject>(startBuoy, objectSpawn, commonRotation, DynamicObjectsGroup.transform);
                }
                else // Placing middle objects:
                {
                    Instantiate<GameObject>(fillerBuoy, objectSpawn, commonRotation, DynamicObjectsGroup.transform);
                }
                placementPosition += averagedSpacing;
            }

            theBoxCollider.transform.localRotation = Quaternion.FromToRotation(this.transform.right, endPoint.position - this.transform.position);
            theBoxCollider.size = new Vector3((endPoint.position - this.transform.position).magnitude + boxColliderAdditionalLength, boxColliderYHeight, boxColliderZHeight);
            theBoxCollider.transform.localPosition = Vector3.Lerp(Vector3.zero, endPoint.localPosition, 0.5f);
        }
    }

    //private void RandomizeObjectPositions()
    //{
    //    foreach
    //}

    #endif
}
