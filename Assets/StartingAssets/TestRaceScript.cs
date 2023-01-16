using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRaceScript : MonoBehaviour
{
    float avg = 0f;
    int times = 0;

    float total_time = 0f;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Race Started");

        total_time = 0f;
    }

    private void Update()
    {
        total_time += Time.deltaTime;
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log(total_time + " is the finishing time.");

        times++;
        avg = (avg + total_time) / times;
        Debug.Log(avg + " is the current average of " + times + " runs.");

        other.transform.position = Vector3.up * 3f;
    }
}
