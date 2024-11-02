using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Add this directive to use SceneManager

public class EndGameToStart : MonoBehaviour
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
            // Switch to scene 0
            SceneManager.LoadScene(0);
        }
    }
}