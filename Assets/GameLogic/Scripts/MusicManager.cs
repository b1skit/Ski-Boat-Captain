using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {
    [Header("Music:")]
    public AudioClip music01_intro;
    public AudioClip music01_loop;

    private AudioSource theMusicSource;

    public bool IsPlaying
    {
        get { return theMusicSource.isPlaying; }
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
        DontDestroyOnLoad(this.gameObject);
    }


    // Use this for initialization
    void Start ()
    {
        theMusicSource = this.GetComponent<AudioSource>();

        if (GameManager.Instance.enableMusic)
        {
            PlayMusic();
        }
	}

    public void PlayMusic()
    {
        //AudioSource.PlayClipAtPoint(music01_intro, Vector3.zero);
        theMusicSource.clip = music01_intro;
        theMusicSource.loop = false;
        theMusicSource.Play();
        Invoke("PlayMainMusicLoop", music01_intro.length);
    }


    private void PlayMainMusicLoop()
    {
        theMusicSource.clip = music01_loop;
        theMusicSource.loop = true;
        theMusicSource.Play();
    }


    public void StopMusic()
    {
        theMusicSource.Stop();
    }

	
	//// Update is called once per frame
	//void Update () {
		
	//}
}
