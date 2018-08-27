using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SkierAudio : MonoBehaviour {

    [Tooltip("The sound of the skier's wake as they move through the water")]
    public AudioClip waterWakeClip;
    
    [Tooltip("The skier velocity at which the wake sound will be played at full volume")]
    public float maxVolumeVelocity = 15.0f;

    private AudioSource wakeAudioSource;
    private Rigidbody theSkierRigidBody;


	// Use this for initialization
	void Start () {
        theSkierRigidBody = this.GetComponent<Rigidbody>();

        // Setup the skier's wake sound:
        // If we add another audiosource, break this out into an init function!
        wakeAudioSource = this.gameObject.AddComponent<AudioSource>();
        wakeAudioSource.clip = waterWakeClip;
        wakeAudioSource.volume = 0;
        wakeAudioSource.loop = true;

        wakeAudioSource.time = Random.Range(0.0f, waterWakeClip.length);
        wakeAudioSource.Play();
	}


	// Update is called once per frame
	void Update () {
        wakeAudioSource.volume = Mathf.Clamp01(theSkierRigidBody.velocity.magnitude / maxVolumeVelocity);
	}
}
