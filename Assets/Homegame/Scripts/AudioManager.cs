using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioListener playerListener;

    public AudioSource musicSource1;
    public AudioSource musicSource2;
    public AudioSource footSource;
    public AudioSource ActionSource1;
    public AudioSource ActionSource2;
    public AudioSource ActionSource3;

    public AudioClip[] footclip;
    public AudioClip musicClip1;
    public AudioClip musicClip2;
    public AudioClip gameOverClip;
    public AudioClip alertClip;
    public AudioClip wristBandClip;

    public GameObject playerObject;

    // Start is called before the first frame update
    void Start()
    {
        playerListener = playerObject.AddComponent<AudioListener>();

        musicSource1 = playerObject.AddComponent<AudioSource>();
        musicSource2 = playerObject.AddComponent<AudioSource>();
        footSource = playerObject.AddComponent<AudioSource>();
        ActionSource1 = playerObject.AddComponent<AudioSource>();
        ActionSource2 = playerObject.AddComponent<AudioSource>();
        ActionSource3 = playerObject.AddComponent<AudioSource>();

        musicSource1.clip = musicClip1;
        musicSource1.loop = true;
        musicSource1.Play();

        musicSource2.clip = musicClip2;
        musicSource2.loop = true;
    }

    // Update is called once per frame
    void Update()
    {
        /*if (Input.GetKey("h")) {
            fadeOutIn(musicSource1, musicSource2);
        }
        if (Input.GetKeyDown("j"))
        {

        }*/
    }

    public void fadeOutIn(AudioSource fadeOutSource, AudioSource fadeInSource) {
        if (fadeInSource.isPlaying == false) {
            fadeInSource.Play();
        }
        fadeOutSource.volume -= 0.2f;
        fadeInSource.volume =+ 0.2f;

        if (fadeOutSource.volume <= 0f) {
            fadeOutSource.Stop();
        }
        if (fadeInSource.volume >= 1f) { 
        
        }
    }
}
