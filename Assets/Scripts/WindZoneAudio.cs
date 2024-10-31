using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class WindZoneAudio : MonoBehaviour
{
    public AudioClip windClip; // The wind sound effect
    public float fadeDuration = 1.0f; // Duration for fading in and out the wind sound
    public float rumbleIntensity = 0.5f; // Intensity of the gamepad rumble

    private AudioSource windSource;
    private Coroutine fadeCoroutine;

    void Start()
    {
        // Create an AudioSource for the wind sound
        windSource = gameObject.AddComponent<AudioSource>();
        windSource.clip = windClip;
        windSource.loop = true;
        windSource.playOnAwake = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Stop any ongoing fade coroutine
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }

            // Start fading in the wind sound
            fadeCoroutine = StartCoroutine(FadeInWindSound());

            // Start gamepad rumble
            StartGamepadRumble();
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Ensure the wind sound is playing while the player is in the wind zone
            if (!windSource.isPlaying)
            {
                windSource.Play();
            }

            // Maintain gamepad rumble
            MaintainGamepadRumble();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Stop any ongoing fade coroutine
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }

            // Start fading out the wind sound
            fadeCoroutine = StartCoroutine(FadeOutWindSound());

            // Stop gamepad rumble
            StopGamepadRumble();
        }
    }

    IEnumerator FadeInWindSound()
    {
        windSource.volume = 0;
        windSource.Play();

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            windSource.volume = Mathf.Lerp(0, 1.0f, t / fadeDuration);
            yield return null;
        }

        windSource.volume = 1.0f; // Ensure volume is set to max after fading in
    }

    IEnumerator FadeOutWindSound()
    {
        float startVolume = windSource.volume;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            windSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
            yield return null;
        }

        windSource.Stop();
        windSource.volume = startVolume; // Reset volume for next time
    }

    void StartGamepadRumble()
    {
        if (Gamepad.current != null)
        {
            Gamepad.current.SetMotorSpeeds(1, 1);
        }
    }

    void MaintainGamepadRumble()
    {
        if (Gamepad.current != null)
        {
            Gamepad.current.SetMotorSpeeds(rumbleIntensity, rumbleIntensity);
        }
    }

    void StopGamepadRumble()
    {
        if (Gamepad.current != null)
        {
            Gamepad.current.SetMotorSpeeds(0, 0);
        }
    }
}