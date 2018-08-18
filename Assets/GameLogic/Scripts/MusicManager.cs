using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {
    [Header("Music:")]
    //public AudioClip music01_intro;
    //public AudioClip music01_loop;

    public AudioSource introMusicSource;
    public AudioSource loopMusicSource;


    //public AudioClip sourceClip;
    //private AudioSource audio1;
    //private AudioSource audio2;
    //private AudioClip cutClip1;
    //private AudioClip cutClip2;
    //private float overlap = 0.2F;
    //private int len1 = 0;
    //private int len2 = 0;

    public bool IsPlaying()
    {
        return introMusicSource.isPlaying || loopMusicSource.isPlaying;
    }

    private static MusicManager _instance = null;
    public static MusicManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<MusicManager>();
                if (_instance == null)
                {
                    GameObject theGameObject = new GameObject("theMusicManager");
                    _instance = theGameObject.AddComponent<MusicManager>();
                }
            }
            return _instance;
        }
    }


    private void Awake()
    {
        MusicManager existingMusicManager = FindObjectOfType<MusicManager>();
        if (existingMusicManager != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            DontDestroyOnLoad(this.gameObject);
        }
    }


    // Use this for initialization
    void Start ()
    {
        introMusicSource.loop = false;
        loopMusicSource.loop = true;

        if (GameManager.Instance.enableMusic)
        {
            PlayMusic();
        }


        //GameObject child;
        //child = new GameObject("Player1");
        //child.transform.parent = gameObject.transform;
        //audio1 = child.AddComponent<AudioSource>();

        //child = new GameObject("Player2");
        //child.transform.parent = gameObject.transform;
        //audio2 = child.AddComponent<AudioSource>();

        //int overlapSamples;

        //len1 = sourceClip.samples / 2;
        //len2 = sourceClip.samples - len1;

        //overlapSamples = (int)(overlap * sourceClip.frequency);

        //cutClip1 = AudioClip.Create("cut1", len1 + overlapSamples, sourceClip.channels, sourceClip.frequency, false);
        //cutClip2 = AudioClip.Create("cut2", len2 + overlapSamples, sourceClip.channels, sourceClip.frequency, false);

        //float[] smp1 = new float[(len1 + overlapSamples) * sourceClip.channels];
        //float[] smp2 = new float[(len2 + overlapSamples) * sourceClip.channels];

        //sourceClip.GetData(smp1, 0);
        //sourceClip.GetData(smp2, len1 - overlapSamples);

        //cutClip1.SetData(smp1, 0);
        //cutClip2.SetData(smp2, 0);



        //if (GameManager.Instance.enableMusic)
        //{
        //    PlayMusic();
        //}

    }

    public void PlayMusic()
    {
        if (!this.IsPlaying())
        {
            introMusicSource.Play();
            introMusicSource.SetScheduledEndTime(AudioSettings.dspTime + introMusicSource.clip.length);

            loopMusicSource.PlayScheduled(AudioSettings.dspTime + introMusicSource.clip.length);
            loopMusicSource.SetScheduledEndTime(AudioSettings.dspTime + introMusicSource.clip.length + loopMusicSource.clip.length);
        }


        //https://docs.unity3d.com/ScriptReference/AudioSource.PlayScheduled.html
        //https://docs.unity3d.com/ScriptReference/AudioSource.SetScheduledEndTime.html

        //audio1.clip = cutClip1;
        //audio2.clip = cutClip2;

        //double t0 = AudioSettings.dspTime + 3.0F;
        //double clipTime1 = len1;

        //clipTime1 /= cutClip1.frequency;

        //audio1.PlayScheduled(t0);
        //audio1.SetScheduledEndTime(t0 + clipTime1);

        //Debug.Log("t0 = " + t0 + ", clipTime1 = " + clipTime1 + ", cutClip1.frequency = " + cutClip1.frequency);
        //Debug.Log("cutClip2.frequency = " + cutClip2.frequency + ", samplerate = " + AudioSettings.outputSampleRate);

        //audio2.PlayScheduled(t0 + clipTime1);
        //audio2.time = overlap;
    }


    //private void PlayMainMusicLoop()
    //{
    //    // ?
    //    Invoke("PlayMainMusicLoop", music01_loop.length);
    //}


    public void StopMusic()
    {
        introMusicSource.Stop();
        loopMusicSource.Stop();
    }


    //// Update is called once per frame
    //void Update () {

    //}
}
