using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
