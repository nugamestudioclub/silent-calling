using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A test script for a moving platform. All moving platforms must have the MovingObject Tag.
/// NOTE THAT THIS SCRIPT IS FRAMERATE DEPENDANT. THE PLATFORM WILL MOVE AT DIFFERENT SPEEDS
/// ON DIFFERENT MACHINES.
/// </summary>
public class TestingPlatformScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.right * Mathf.Sin(Time.realtimeSinceStartup) * 0.09f;
    }
}
