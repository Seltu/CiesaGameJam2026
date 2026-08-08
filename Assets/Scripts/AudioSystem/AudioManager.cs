using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource introAudioSource;
    public AudioSource loopAudioSource;
    public AudioSource sfxAudioSource;

    public List<AudioClip> stageThemeIntros;
    public List<AudioClip> stageThemeLoops;
    public AudioClip gameOverTheme;
    public AudioClip stageClearTheme;
    public AudioClip trainingTheme;
    public AudioClip introTheme;

    public static AudioManager instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlayOneShot(AudioClip clip)
    {
        sfxAudioSource.PlayOneShot(clip);
    }

    public void PlayOneShotTheme(AudioClip clip)
    {
        Stop();

        introAudioSource.clip = clip;
        introAudioSource.Play();
    }

    public void PlayAndLoop(AudioClip intro, AudioClip loop)
    {
        Stop();

        introAudioSource.clip = intro;
        loopAudioSource.clip = loop;

        // Play an intro Clip followed by a loop
        double introDuration = (double)introAudioSource.clip.samples / introAudioSource.clip.frequency;
        double startTime = AudioSettings.dspTime + 0.05;
        introAudioSource.PlayScheduled(startTime);
        loopAudioSource.PlayScheduled(startTime + introDuration);
    }

    public void Stop()
    {
        introAudioSource.Stop();
        loopAudioSource.Stop();
    }
}
