using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndGameTrigger : MonoBehaviour
{
    public Image fadeImage; // Reference to the UI Image for fading
    public float fadeDuration = 2.0f; // Duration of the fade effect

    private bool isFading = false;

    void Start()
    {
        if (fadeImage != null)
        {
            // Ensure the image is transparent at the start
            fadeImage.color = new Color(1, 1, 1, 0);
        }
    }

    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isFading)
        {
            // Stop any shaking effect
            StopShaking();

            // Start the fade to white
            StartCoroutine(FadeToWhite());
        }
    }

    IEnumerator FadeToWhite()
    {
        isFading = true;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            fadeImage.color = new Color(1, 1, 1, alpha);
            yield return null;
        }

        // Ensure the image is fully white
        fadeImage.color = new Color(1, 1, 1, 1);

        // Load the next scene
        LoadNextScene();
    }

    void StopShaking()
    {
        // Implement logic to stop any shaking effect here
        // For example, if you have a camera shake script, disable it
    }

    void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex + 1);
    }
}