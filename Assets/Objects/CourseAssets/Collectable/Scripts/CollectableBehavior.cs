using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableBehavior : MonoBehaviour {

    public int pointValue = 100;

	// Use this for initialization
	void Start () {
		//this.gameObject.GetComponent<Rigidbody>().
	}
	
	// Update is called once per frame
	void Update () {
        this.transform.Rotate(this.transform.rotation.x + (100 * Time.deltaTime), this.transform.rotation.y, this.transform.rotation.z);
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "Skier")
        {
            GameManager.Instance.AddPoints(pointValue);
            Destroy(this.gameObject);
        }
    }
}
