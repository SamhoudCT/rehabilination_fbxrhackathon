using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public static AudioController Instance;

    //Inspector variables
    [SerializeField] private AudioClip soundThrow;
    [SerializeField] private AudioClip soundVictory;
    [SerializeField] private AudioClip soundBucket;
    [SerializeField] private AudioClip soundGrab;
    
    //Private variables
    private AudioSource source;
    private bool playingVictory;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    public void PlayGrabSound()
    {
        if (playingVictory) return;
        
        source.clip = soundGrab;
        source.loop = false;
        source.Play();
    }

    public void PlayThrowSound()
    {
        if (playingVictory) return;
        
        source.clip = soundThrow;
        source.loop = false;
        source.Play();
    }

    public void PlayVictorySound()
    {
        source.clip = soundVictory;
        source.loop = false;
        source.Play();
        playingVictory = true;
        Observable.Timer(TimeSpan.FromSeconds(4.5f)).Subscribe(x => playingVictory = false);
    }

    public void PlayBucketSound()
    {
        if (playingVictory) return;
        
        source.clip = soundBucket;
        source.loop = false;
        source.Play();
    }
}
