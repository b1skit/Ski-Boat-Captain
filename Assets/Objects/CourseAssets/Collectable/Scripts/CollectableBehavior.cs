using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectableBehavior : MonoBehaviour {

    [Tooltip("Points text popup prefab")]
    public GameObject pointsPopupText;

    [Tooltip("How long the points popup should be displayed")]
    public float pointUIPopupTime = 3.0f;

    private GameObject pointsPopup;
    private Vector3 pointsLocation;

    protected Canvas mainCanvas;
    protected RectTransform mainCanvasRectTransform;

    [Tooltip("The number of points to award for collecting this object")]
    public int pointValue = 100;

    [Tooltip("The euler angles to rotate this object each frame")]
    public Vector3 rotation = new Vector3(100.0f, 0.0f, 0.0f);

    [Tooltip("The sound clip to play when this objeect is collected. NOTE: *CANNOT* be longer than pointUIPopupTime")]
    private AudioSource pickupSound;

    private void Start()
    {
        pickupSound = this.gameObject.GetComponent<AudioSource>();

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

    // Update is called once per frame
    void Update () {
        this.transform.Rotate(rotation * Time.deltaTime);

        if (pointsPopup)
        {
            //Destroy(pointsPopup);


            Vector2 screenPointsLocation = RectTransformUtility.WorldToScreenPoint(mainCanvas.worldCamera, pointsLocation);

            if (RectTransformUtility.RectangleContainsScreenPoint(mainCanvasRectTransform, screenPointsLocation, mainCanvas.worldCamera))
            {
                Vector2 hoverPoint = new Vector2();
                RectTransformUtility.ScreenPointToLocalPointInRectangle(mainCanvasRectTransform, screenPointsLocation, mainCanvas.worldCamera, out hoverPoint);
                //pointsPopup = Instantiate<GameObject>(pointsPopupText, mainCanvas.transform);
                pointsPopup.GetComponent<RectTransform>().anchoredPosition = hoverPoint;
                Debug.Log(hoverPoint);

                
            }
            else
            {
                Destroy(pointsPopup);

                Debug.Log("Destroyed out of bounds collection text");
            }
                
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Skier"))
        {
            pickupSound.Play();

            pointsLocation = this.transform.position;
            
            Vector2 screenPointsLocation = RectTransformUtility.WorldToScreenPoint(mainCanvas.worldCamera, pointsLocation);

            Vector2 hoverPoint = new Vector2();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(mainCanvasRectTransform, screenPointsLocation, mainCanvas.worldCamera, out hoverPoint);

            pointsPopup = Instantiate<GameObject>(pointsPopupText, mainCanvas.transform);
            pointsPopup.GetComponent<RectTransform>().anchoredPosition = hoverPoint;

            pointsPopup.GetComponent<Text>().text = pointValue.ToString();

            //Invoke("RemovePointsPopup", pointUIPopupTime);
            Destroy(pointsPopup, pointUIPopupTime);

            GameManager.Instance.AddPoints(pointValue);
            
            // TEMP HACK: Sound playback is cancelled when an object is destroyed. So I destroy the mesh, then destroy the object once the sound is finished. Is there a simpler way to handle this?
            Destroy(this.gameObject.GetComponentInChildren<MeshRenderer>());
            Destroy(this.gameObject, pickupSound.clip.length); 
        }
    }

    //private void RemovePointsPopup()
    //{
    //    Destroy(pointsPopup);
    //}
}
