﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkierBehavior : MonoBehaviour {

	//// Use this for initialization
	//void Start () {
		
	//}
	


	//// Update is called once per frame
	//void Update () {
		
	//}

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Collectable" && SceneManager.instance.IsPlaying)
        {
            SceneManager.instance.FailLevel();

            Destroy(this.GetComponentsInChildren<MeshFilter>()[0]); // TEMP HACK: Destroy the visible mesh (only), so the rope can still be connected to the gameObject
        }
    }
}
