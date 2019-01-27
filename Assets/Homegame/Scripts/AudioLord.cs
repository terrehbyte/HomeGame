using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioLord : MonoBehaviour
{

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

    public float transitionTime;
    private int songPlaying;
    private float time;
    private bool isSwapping;
    // Start is called before the first frame update
    void Start()
    {

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

        if (Input.GetKeyDown(KeyCode.O))
        {
            SwapSong();
        }


        if (isSwapping)
        {
            if (songPlaying == 1)
            {
                musicSource1.volume -= 1 / transitionTime * Time.deltaTime;
                musicSource2.volume += 1 / transitionTime * Time.deltaTime;

                if (musicSource1.volume <= 0.0f)
                {
                    isSwapping = false;
                    musicSource1.Pause();
                }
            }
            else
            {
                musicSource2.volume -= 1 / transitionTime * Time.deltaTime;
                musicSource1.volume += 1 / transitionTime * Time.deltaTime;

                if (musicSource1.volume <= 0.0f)
                {
                    isSwapping = false;
                    musicSource2.Pause();
                }
            }
        }


    }

    public void SwapSong()
    {
        time = 0.0f;
        isSwapping = true;

        if (musicSource1.isPlaying)
        {
            songPlaying = 1;

            musicSource2.volume = 0.0f;
            musicSource2.Play();
        }
        else
        {
            songPlaying = 2;

            musicSource1.volume = 0.0f;
            musicSource1.Play();
        }
    }

    public void BlinkCaller()
    {
        ActionSource1.PlayOneShot(wristBandClip);

    }

    public void FootCaller()
    {
        footSource.PlayOneShot(footclip[Random.Range(0, 6)]);
    }



}
