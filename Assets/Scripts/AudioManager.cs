using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AudioType
{
    Ambience,
    BGM,
    SFX
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource ambienceSource;
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    [Header("Audio Clips")]
    public AudioClip[] ambienceClips;
    public AudioClip[] bgmClips;
    public AudioClip[] sfxClips;

    private Dictionary<AudioType, AudioSource> audioSources;
    private Dictionary<AudioType, AudioClip[]> audioClips;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // Initialize dictionaries
        audioSources = new Dictionary<AudioType, AudioSource>
        {
            { AudioType.Ambience, ambienceSource },
            { AudioType.BGM, bgmSource },
            { AudioType.SFX, sfxSource }
        };

        audioClips = new Dictionary<AudioType, AudioClip[]>
        {
            { AudioType.Ambience, ambienceClips },
            { AudioType.BGM, bgmClips },
            { AudioType.SFX, sfxClips }
        };

        // Preload audio clips
        PreloadAudioClips();

        // Play ambience from the start
        PlayAmbienceFromStart();
    }

    void PreloadAudioClips()
    {
        // Access each audio clip to ensure they are loaded into memory
        foreach (var clip in ambienceClips)
        {
            if (clip.loadState != AudioDataLoadState.Loaded)
            {
                clip.LoadAudioData();
            }
        }

        foreach (var clip in bgmClips)
        {
            if (clip.loadState != AudioDataLoadState.Loaded)
            {
                clip.LoadAudioData();
            }
        }

        foreach (var clip in sfxClips)
        {
            if (clip.loadState != AudioDataLoadState.Loaded)
            {
                clip.LoadAudioData();
            }
        }
    }

    public void PlayAudio(AudioType type, int clipIndex, bool loop = false)
    {
        if (audioSources.ContainsKey(type) && audioClips.ContainsKey(type))
        {
            AudioSource source = audioSources[type];
            AudioClip[] clips = audioClips[type];

            if (clipIndex >= 0 && clipIndex < clips.Length)
            {
                source.clip = clips[clipIndex];
                source.loop = loop;
                source.Play();
            }
            else
            {
                Debug.LogWarning("Clip index out of range for audio type: " + type);
            }
        }
        else
        {
            Debug.LogWarning("Audio type not found: " + type);
        }
    }

    public void StopAudio(AudioType type)
    {
        if (audioSources.ContainsKey(type))
        {
            AudioSource source = audioSources[type];
            source.Stop();
        }
        else
        {
            Debug.LogWarning("Audio type not found: " + type);
        }
    }

    public void PauseAudio(AudioType type)
    {
        if (audioSources.ContainsKey(type))
        {
            AudioSource source = audioSources[type];
            source.Pause();
        }
        else
        {
            Debug.LogWarning("Audio type not found: " + type);
        }
    }

    public void ResumeAudio(AudioType type)
    {
        if (audioSources.ContainsKey(type))
        {
            AudioSource source = audioSources[type];
            source.UnPause();
        }
        else
        {
            Debug.LogWarning("Audio type not found: " + type);
        }
    }

    public void PlayAmbienceFromStart()
    {
        PlayAudio(AudioType.Ambience, 0, true);
    }

    public void PlayRandomSFX(int startIndex, int endIndex, float minPitch = 0.9f, float maxPitch = 1.1f)
    {
        if (sfxClips.Length > 0)
        {
            // Ensure the start and end indices are within the bounds of the sfxClips array
            startIndex = Mathf.Clamp(startIndex, 0, sfxClips.Length - 1);
            endIndex = Mathf.Clamp(endIndex, 0, sfxClips.Length - 1);
    
            // Ensure the start index is less than or equal to the end index
            if (startIndex <= endIndex)
            {
                int randomIndex = Random.Range(startIndex, endIndex + 1);
                sfxSource.clip = sfxClips[randomIndex];
                sfxSource.pitch = Random.Range(minPitch, maxPitch);
                sfxSource.Play();
            }
            else
            {
                Debug.LogWarning("Start index must be less than or equal to end index.");
            }
        }
        else
        {
            Debug.LogWarning("No SFX clips available to play.");
        }
    }
}