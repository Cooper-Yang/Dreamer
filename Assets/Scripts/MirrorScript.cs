using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class MirrorScript : MonoBehaviour
{
    private Transform mainCamera;
    private Vector3 offset;
    
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = GameObject.Find("Main Camera").GetComponent<Transform>();
        offset = mainCamera.eulerAngles - transform.eulerAngles; 
        
    }

    // Update is called once per frame
    void Update()
    {
        quaternion rotation = Quaternion.Euler(mainCamera.eulerAngles - offset); //- offset *-1f
        gameObject.transform.rotation = rotation;
    }
    
}
