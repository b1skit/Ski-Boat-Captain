using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableBehavior : MonoBehaviour {

    public int pointValue = 100;
    public Vector3 rotation = new Vector3(100.0f, 0.0f, 0.0f);

	// Update is called once per frame
	void Update () {
        this.transform.Rotate(rotation * Time.deltaTime);
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
