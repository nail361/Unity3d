using UnityEngine;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    public AudioSource efxSource;
    public AudioSource musicSource;
    public static SoundManager instance = null;
    public float lowPitchRange = .95f;
    public float highPitchRange = 1.05f;

    [SerializeField]
    private List<AudioClip> soundsCollection;
    
    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public void ChangeMusicVolume(float vol)
    {
        musicSource.volume = vol;
    }

    public void PlayMusic(string music_name, float delay = 0, float vol = 1.0f)
    {
        foreach (AudioClip music in soundsCollection)
        {
            if (music != null && music.name == music_name)
            {
                PlayMusic(music, delay, vol);
                break;
            }
        }
    }

    public void PlayMusic(AudioClip music = null, float delay = 0, float vol = 1.0f)
    {
        if (!GameManager.instance.music_on) return;

        if (music == null) musicSource.Stop();
        else {
            musicSource.Stop();
            musicSource.clip = music;
            musicSource.volume = vol;
            musicSource.PlayDelayed(delay);
        }
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void PauseMusic()
    {
        musicSource.Pause();
    }

    public void ResumeMusic()
    {
        musicSource.UnPause();
    }

    public void PlaySound(string sound_name, bool pitch = false, float delay = 0)
    {
        foreach (AudioClip sound in soundsCollection)
        {
            if (sound != null && sound.name == sound_name)
            {
                PlaySound(sound, pitch, delay);
                break;
            }
        }
    }

    public void PlaySound(AudioClip clip, bool pitch = false, float delay = 0)
    {
        if (!GameManager.instance.sound_on) return;

        efxSource.clip = clip;

        if (pitch)
        {
            float randomPitch = Random.Range(lowPitchRange, highPitchRange);
            efxSource.pitch = randomPitch;
        }
        efxSource.PlayDelayed(delay);
    }

    public void RandomizeSfx(params AudioClip[] clips)
    {
        if (!GameManager.instance.sound_on) return;

        int randomIndex = Random.Range(0, clips.Length);
        PlaySound(clips[randomIndex], true);
    }

}