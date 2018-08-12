using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierAudio : MonoBehaviour {

    public AudioSource collisionSound;


    public float volumeMultiplier = 0.25f;


    private void OnCollisionEnter(Collision collision)
    {
        GameObject collidingObject = collision.gameObject;
        Rigidbody collidingRb;
        float rbVelocityMagnitude = 0.0f;

        if (collidingObject.CompareTag("Player"))
        {
            collidingRb = collidingObject.GetComponentInParent<Rigidbody>();
            rbVelocityMagnitude = collidingRb.velocity.magnitude;
        }
        else if (collidingObject.CompareTag("Skier"))
        {
            collidingRb = collidingObject.GetComponent<Rigidbody>();
            rbVelocityMagnitude = collidingRb.velocity.magnitude;
        }

        collisionSound.volume = rbVelocityMagnitude * volumeMultiplier;
        collisionSound.Play();
    }
}
