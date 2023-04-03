using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    int i = 0;

    public void Print()
    {
        Debug.Log(i);

        i += 1;
    }
}
