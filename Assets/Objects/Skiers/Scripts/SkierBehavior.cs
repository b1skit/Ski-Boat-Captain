using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkierBehavior : MonoBehaviour {

    public GameObject ropeObject; // TO DO: Move this reference to PlayerPositionHistoryControl.cs ?

    //private ConfigurableJoint skiRopeJoint;

    //// Use this for initialization
    //void Start()
    //{
    //    skiRopeJoint = this.GetComponent<ConfigurableJoint>();
    //}


    //// Update is called once per frame
    //void Update()
    //{
    //    if (skiRopeJoint == null && SceneManager.Instance.IsPlaying)
    //    {
    //        Destroy(ropeObject);
    //        SceneManager.Instance.FailLevel();
    //    }
    //}


    //private void OnCollisionEnter(Collision collision)
    //{
    // TO DO: Skier collision impact sound effects 
    //}
}
