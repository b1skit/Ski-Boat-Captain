using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrindRailAudio : MonoBehaviour {

    public AudioSource railHitClip;

    public AudioSource grindingClip;

    public float railHitVolumeFactor = 1.0f;
    public float railHitPitchFactor = 1.0f;

    public float grindingVolumeFactor = 1.0f;
    public float grindingPitchFactor = 1.0f;

    private Rigidbody otherRb;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Skier"))
        {
            otherRb = other.gameObject.GetComponent<Rigidbody>();
            railHitClip.volume = otherRb.velocity.magnitude * railHitVolumeFactor;
            railHitClip.pitch = otherRb.velocity.magnitude * railHitPitchFactor;
            railHitClip.Play();

            grindingClip.volume = otherRb.velocity.magnitude * grindingVolumeFactor;
            grindingClip.pitch = otherRb.velocity.magnitude * grindingPitchFactor;
            grindingClip.Play();
        }
            
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Skier"))
        {
            grindingClip.volume = otherRb.velocity.magnitude * grindingVolumeFactor;
            grindingClip.pitch = otherRb.velocity.magnitude * grindingPitchFactor;
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        grindingClip.Stop();
    }
}
