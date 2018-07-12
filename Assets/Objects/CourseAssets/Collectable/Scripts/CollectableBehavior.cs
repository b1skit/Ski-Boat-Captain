using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableBehavior : MonoBehaviour {

    [Tooltip("The number of points to award for collecting this object")]
    public int pointValue = 100;

    [Tooltip("The euler angles to rotate this object each frame")]
    public Vector3 rotation = new Vector3(100.0f, 0.0f, 0.0f);

    [Tooltip("The sound clip to play when this objeect is collected")]
    private AudioSource pickupSound;

    private void Start()
    {
        pickupSound = this.gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update () {
        this.transform.Rotate(rotation * Time.deltaTime);
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "Skier")
        {
            pickupSound.Play();
            GameManager.Instance.AddPoints(pointValue);

            // TEMP HACK: Sound playback is cancelled when an object is destroyed. So I destroy the mesh, then destroy the object once the sound is finished. Is there a simpler way to handle this?
            Destroy(this.gameObject.GetComponent<MeshRenderer>());
            Destroy(this.gameObject, pickupSound.clip.length); 
        }
    }
}
