using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierAudio : MonoBehaviour {

    public AudioSource collisionSound;

    public AudioSource rubbingSound;


    public float collisionVolumeMultiplier = 0.25f;
    public float collsionPitchMultiplier = 0.15f;

    public float rubbingVolumeMultiplier = 0.25f;
    public float rubbingPitchMultiplier = 0.15f;

    private Rigidbody collidingRb;

    private void Update()
    {
        if (rubbingSound.isPlaying)
        {
            rubbingSound.volume = collidingRb.velocity.magnitude * rubbingVolumeMultiplier;
            rubbingSound.pitch = collidingRb.velocity.magnitude * rubbingPitchMultiplier;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        float rbVelocityMagnitude = 0.0f;

        if (collision.gameObject.CompareTag("Player"))
        {
            collidingRb = collision.gameObject.GetComponentInParent<Rigidbody>();
            rbVelocityMagnitude = collidingRb.velocity.magnitude;
        }
        else if (collision.gameObject.CompareTag("Skier"))
        {
            collidingRb = collision.gameObject.GetComponent<Rigidbody>();
            rbVelocityMagnitude = collidingRb.velocity.magnitude;
        }

        collisionSound.volume = rbVelocityMagnitude * collisionVolumeMultiplier;
        collisionSound.pitch = rbVelocityMagnitude * collsionPitchMultiplier;
        if (!collisionSound.isPlaying)
        {
            collisionSound.Play();
        }

        if (!rubbingSound.isPlaying)
        {
            rubbingSound.volume = rbVelocityMagnitude * rubbingVolumeMultiplier;
            rubbingSound.pitch = rbVelocityMagnitude * rubbingPitchMultiplier;
            rubbingSound.Play();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        rubbingSound.Stop();
    }
}
