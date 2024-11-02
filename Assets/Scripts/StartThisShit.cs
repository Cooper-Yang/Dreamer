using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Add this directive to use SceneManager

public class StartThisShit : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Check for any input
        if (Input.anyKeyDown)
        {
            // Switch to the next scene
            LoadNextScene();
        }
    }

    void LoadNextScene()
    {
        // Get the current scene index
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        // Load the next scene
        SceneManager.LoadScene(currentSceneIndex + 1);
    }
}