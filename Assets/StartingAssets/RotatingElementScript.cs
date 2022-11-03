using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingElementScript : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(Rotate());
    }

    IEnumerator Rotate()
    {
        while (true)
        {
            transform.Rotate(Vector2.up, Space.World);

            yield return new WaitForEndOfFrame();
        }
    }
}
