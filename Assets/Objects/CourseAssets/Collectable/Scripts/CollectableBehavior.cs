using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectableBehavior : MonoBehaviour {

    [Tooltip("Points text popup prefab")]
    public GameObject pointsPopupText;

    [Tooltip("How long the points points should remain visible, in seconds. NOTE: *MUST* be longer than the pickup sound effect length")]
    public float pointsPopupStayTime = 3.0f;

    [Tooltip("The transform of this object's viewmesh. Used to apply rotation animation")]
    public Transform viewMeshTransform;

    private GameObject pointsPopup;

    [Tooltip("The number of points to award for collecting this object")]
    public int pointValue = 100;

    [Tooltip("The euler angles to rotate this object each frame")]
    public Vector3 rotation = new Vector3(100.0f, 0.0f, 0.0f);

    private AudioSource pickupSound;

    private Canvas worldSpaceCanvas;
    private Camera mainCamera;

    private void Start()
    {
        pickupSound = this.gameObject.GetComponent<AudioSource>();

        worldSpaceCanvas = this.GetComponentInChildren<Canvas>();

        Camera[] theCameras = Resources.FindObjectsOfTypeAll<Camera>();
        foreach (Camera current in theCameras)
        {
            if (current.gameObject.scene == UnityEngine.SceneManagement.SceneManager.GetActiveScene())
            {
                mainCamera = current;
            }
        }
    }

    // Update is called once per frame
    void Update () {
        viewMeshTransform.Rotate(rotation * Time.deltaTime);

        if (pointsPopup)
        {
            pointsPopup.transform.rotation = mainCamera.transform.rotation;
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Skier"))
        {           
            pointsPopup = Instantiate<GameObject>(pointsPopupText, worldSpaceCanvas.transform);
            pointsPopup.GetComponent<Text>().text = pointValue.ToString();
            
            pickupSound.Play();

            GameManager.Instance.AddPoints(pointValue);
            
            // HACK: Sound playback is cancelled when an object is destroyed. So we destroy the mesh, then destroy the object once the sound is finished. Is there a simpler way to handle this?
            Destroy(this.gameObject.GetComponentInChildren<MeshRenderer>());
            Destroy(this.gameObject, pointsPopupStayTime);
        }
    }
}
