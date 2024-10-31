using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindArea : MonoBehaviour
{
    public Vector3 windDirection = new Vector3(1, 0, 0); // Direction of the wind
    public float windStrength = 10f; // Strength of the wind
    public float constantWindStrength = 5000f; // Strength of the wind
    // Start is called before the first frame update
    void Start()
    {
        // Ensure the BoxCollider is set as a trigger
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        if (boxCollider != null)
        {
            boxCollider.isTrigger = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Called when another collider enters the trigger
    void OnTriggerEnter(Collider other)
    {
        // Check if the other collider has a Rigidbody
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Apply wind force
            rb.AddForce(windDirection.normalized * windStrength, ForceMode.Impulse);
        }
    }

    // Called when another collider stays within the trigger
    void OnTriggerStay(Collider other)
    {
        // Check if the other collider has a Rigidbody
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Apply wind force continuously
            rb.AddForce(windDirection.normalized * constantWindStrength * Time.deltaTime, ForceMode.Force);
        }
    }
}