using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MusicPlayer : MonoBehaviour
{
    public bool playBase;
    public AudioClip baseMusic;
    private AudioSource baseTrack;

    public bool playCombat;
    public AudioClip combatMusic;
    private AudioSource combatTrack;

    public bool playSpeed;
    public AudioClip speedMusic;
    private AudioSource speedTrack;

    void Awake()
    {
        baseTrack = gameObject.AddComponent<AudioSource>();
        combatTrack = gameObject.AddComponent<AudioSource>();
        speedTrack = gameObject.AddComponent<AudioSource>();

        baseTrack.volume = 0.0f;
        combatTrack.volume = 0.0f;
        speedTrack.volume = 0.0f;

        baseTrack.clip = baseMusic;
        combatTrack.clip = combatMusic;
        speedTrack.clip = speedMusic;

        baseTrack.Play();
        combatTrack.Play();
        speedTrack.Play();
    }

    private void Start()
    {
        playBase = true;
        baseTrack.DOFade(0.33f, 1.5f);
    }


    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            playBase = !playBase;

            if (playBase)
            {
                baseTrack.DOFade(0.33f, 1.5f);
            }
            else
            {
                baseTrack.DOFade(0.0f, 1.5f);
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            playCombat = !playCombat;

            if (playCombat)
            {
                combatTrack.DOFade(0.4f, 1.5f);
            }
            else
            {
                combatTrack.DOFade(0.0f, 1.5f);
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            playSpeed = !playSpeed;

            if (playSpeed)
            {
                speedTrack.DOFade(0.4f, 1.5f);
            }
            else
            {
                speedTrack.DOFade(0.0f, 1.5f);
            }
        }

    }
}
