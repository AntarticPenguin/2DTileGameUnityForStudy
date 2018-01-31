using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour {

    //Singleton

    static SoundPlayer _instance;
    public static SoundPlayer Instance
    {
        get
        {
            if(null == _instance)
            {
                _instance = FindObjectOfType<SoundPlayer>();
                if(null == _instance)
                {
                    GameObject gameobj = new GameObject();
                    gameobj.name = "SoundPlayer";
                    _instance = gameobj.AddComponent<SoundPlayer>();
                    DontDestroyOnLoad(gameobj);
                }
            }
            return _instance;
        }
    }


    //Unity Functions

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    void Awake()
    {
        _audioSource = gameObject.AddComponent<AudioSource>();
    }


    //SoundPlayer

    AudioSource _audioSource;

    public void playEffect(string soundName)
    {
        string filePath = "Sounds/Effects/" + soundName;
        AudioClip clip = Resources.Load<AudioClip>(filePath);

        if(null != clip)
        {
            _audioSource.clip = clip;
            _audioSource.Play();
        }
    }
}
