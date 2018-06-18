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
		
	}

    private void OnCollisionEnter(Collision collision) // USE A TRIGGER INSTEAD?????
    {
        if (collision.gameObject.tag == "Player")
        {
            GameManager.Instance.AddPoints(pointValue);
            Destroy(this.gameObject);
        }
        
    }
}
