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
            transform.Rotate(Vector3.up * 0.25f);

            yield return new WaitForEndOfFrame();
        }
    }
}
