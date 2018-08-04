using System;
using UnityEngine;
using Random = UnityEngine.Random;

// This is a highly modified version of the CarAudio control script provided in the Unity standard assets project
[RequireComponent(typeof (PlayerControl))]
public class BoatAudio : MonoBehaviour
{
	// the engine clips should all be a steady pitch, not rising or falling.

	// The clips should be:
	// lowAccelClip : The engine at low revs, with throttle open (i.e. begining acceleration at very low speed)
	// highAccelClip : Thenengine at high revs, with throttle open (i.e. accelerating, but almost at max speed)
	// lowDecelClip : The engine at low revs, with throttle at minimum (i.e. idling or engine-braking at very low speed)
	// highDecelClip : Thenengine at high revs, with throttle at minimum (i.e. engine-braking at very high speed)

	// For proper crossfading, the clips pitches should all match, with an octave offset between low and high.


	public AudioClip lowAccelClip;                                              // Audio clip for low acceleration
	public AudioClip lowDecelClip;                                              // Audio clip for low deceleration
	public AudioClip highAccelClip;                                             // Audio clip for high acceleration
	public AudioClip highDecelClip;                                             // Audio clip for high deceleration
    public AudioClip waterWakeClip;

	[Tooltip("A constant factor used to alter the pitch of audio clips")]
	public float pitchMultiplier = 1f;

    [Tooltip("The lowest possible pitch for the low sounds")]
    public float lowPitchMin = 1f;

    [Tooltip("The highest possible pitch for the low sounds")]
	public float lowPitchMax = 6f;

	[Tooltip("A constant factor used to alter the pitch of high sounds. Is multiplied with pitchMultipier")]
	public float highPitchMultiplier = 0.25f;

	private AudioSource m_LowAccel; // Source for the low acceleration sounds
	private AudioSource m_LowDecel; // Source for the low deceleration sounds
	private AudioSource m_HighAccel; // Source for the high acceleration sounds
	private AudioSource m_HighDecel; // Source for the high deceleration sounds
    private AudioSource waterWake;
	private bool m_StartedSound; // flag for knowing if we have started sounds
	private PlayerControl thePlayerControl; // Reference to player ship
    private float engineRevs;

    [Tooltip("The number of seconds it takes for the revs to match the user's input")]
    public float engineRevResponseTime = 0.4f;

    [Tooltip("Strength of the pitch fluctuation caused by the bobbing of the ship in the water")]
    public float bobPitchFactor = 0.8f;

    private void Start()
    {
        StartSound();
        engineRevs = 0f;
    }

    // Update is called once per frame
    private void Update()
	{
        // Leaving this here incase I need it...
        //// Stop sound, if required
        //if (m_StartedSound && false) 
        //{
        //    StopSound();
        //}

        AdjustRevs(); 

        if (m_StartedSound)
		{
            // The pitch is interpolated between the min and max values, according to the revs
            float pitch = ULerp(lowPitchMin, lowPitchMax, engineRevs); 

            // clamp to minimum pitch (note, not clamped to max for high revs)
            pitch = Mathf.Min(lowPitchMax, pitch);

            // Adjust the pitches based on the multipliers;
            //      pitch is ULerp'd between low and high, depending on "revs"
            //      pitchMultiplier is a constant supplied by the user
            //      highPitchMultiplier is yet another constant supplied by the user

            m_LowAccel.pitch = (pitch * pitchMultiplier) + (thePlayerControl.viewMeshTransform.localRotation.y * bobPitchFactor);
            m_LowDecel.pitch = (pitch * pitchMultiplier) + (thePlayerControl.viewMeshTransform.localRotation.y * bobPitchFactor);
            m_HighAccel.pitch = (pitch * highPitchMultiplier * pitchMultiplier) + (thePlayerControl.viewMeshTransform.localRotation.y * bobPitchFactor);
            m_HighDecel.pitch = (pitch * highPitchMultiplier * pitchMultiplier) + (thePlayerControl.viewMeshTransform.localRotation.y * bobPitchFactor);

            // get values for fading the sounds based on the acceleration
            float accFade = Mathf.Abs(thePlayerControl.VerticalInput); // Grab the user's input
            float decFade = 1 - accFade;

            // get the high fade value based on the cars revs
            //float highFade = Mathf.InverseLerp(0.2f, 0.8f, thePlayerControl.VerticalInput); // TEMP HACK! Need to create a "revs" function that slowly moves between [0,1] in response to user input, instead of immediate throttle changes
            float highFade = Mathf.InverseLerp(0.2f, 0.8f, engineRevs); 
            float lowFade = 1 - highFade;

            // adjust the values to be more realistic
            highFade = 1 - ((1 - highFade) * (1 - highFade));
            lowFade = 1 - ((1 - lowFade) * (1 - lowFade));
            accFade = 1 - ((1 - accFade) * (1 - accFade));
            decFade = 1 - ((1 - decFade) * (1 - decFade));

            // adjust the source volumes based on the fade values
            m_LowAccel.volume = lowFade * accFade;
            m_LowDecel.volume = lowFade * decFade;
            m_HighAccel.volume = highFade * accFade;
            m_HighDecel.volume = highFade * decFade;

            waterWake.volume = Mathf.Clamp(thePlayerControl.viewMeshTransform.localRotation.y + (highFade * accFade), 0.2f, 1); // ???
        }
    }


    private void AdjustRevs()
    {
        float currentVerticalInput = thePlayerControl.VerticalInput;

        if (engineRevs < currentVerticalInput)
        {
            engineRevs += engineRevResponseTime * Time.deltaTime;

            if (engineRevs > currentVerticalInput) // Clamp the value
                engineRevs = currentVerticalInput;
        }
        else if (engineRevs > currentVerticalInput)
        {
            engineRevs -= engineRevResponseTime * Time.deltaTime;

            if (engineRevs < currentVerticalInput) // Clamp the value
                engineRevs = currentVerticalInput;
        }
    }


    private void StartSound()
    {
        thePlayerControl = GetComponent<PlayerControl>();

        // setup the audio sources
        m_HighAccel = SetUpEngineAudioSource(highAccelClip);
        m_LowAccel = SetUpEngineAudioSource(lowAccelClip);
        m_LowDecel = SetUpEngineAudioSource(lowDecelClip);
        m_HighDecel = SetUpEngineAudioSource(highDecelClip);
        waterWake = SetUpEngineAudioSource(waterWakeClip);

        // flag that we have started the sounds playing
        m_StartedSound = true;
    }


    private void StopSound() // THIS GETS CALLED IN UPDATE IF THE CAR IS OUT OF AUDIBLE RANGE
    {
        //Destroy all audio sources on this object:
        foreach (var source in GetComponents<AudioSource>())
        {
            Destroy(source);
        }
        m_StartedSound = false;
    }


    // sets up and adds new audio source to the gane object
    private AudioSource SetUpEngineAudioSource(AudioClip clip)
	{
		// create the new audio source component on the game object and set up its properties
		AudioSource source = gameObject.AddComponent<AudioSource>();
		source.clip = clip;
		source.volume = 0;
		source.loop = true;

		// start the clip from a random point
		source.time = Random.Range(0f, clip.length);
		source.Play();

		return source;
	}


	// unclamped versions of Lerp and Inverse Lerp, to allow value to exceed the from-to range
	private static float ULerp(float from, float to, float value)
	{
		return (1.0f - value) * from + value * to;
	}
}
