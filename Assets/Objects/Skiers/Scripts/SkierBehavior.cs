using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkierBehavior : MonoBehaviour {

    public GameObject ropeObject;

    private ConfigurableJoint skiRopeJoint;

    // Use this for initialization
    void Start()
    {
        skiRopeJoint = this.GetComponent<ConfigurableJoint>();
    }



    // Update is called once per frame
    void Update()
    {
        if (skiRopeJoint == null && SceneManager.instance.IsPlaying)
        {
            Destroy(ropeObject);
            SceneManager.instance.FailLevel();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //if (collision.gameObject.tag != "Collectable" && SceneManager.instance.IsPlaying)
        //{
        //    SceneManager.instance.FailLevel();

        //    Destroy(this.GetComponentsInChildren<MeshFilter>()[0]); // TEMP HACK: Destroy the visible mesh (only), so the rope can still be connected to the gameObject
        //}
    }
}
