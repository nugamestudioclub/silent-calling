using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingMenu : MonoBehaviour
{

    private bool coroutineRunning = false;

    [SerializeField]
    private GameObject button;

    [SerializeField]
    private float rateOfBlinking = 0.8f;
    // Update is called once per frame
    void Update()
    {
        if(!coroutineRunning) {
            StartCoroutine(MyCoroutine());
        }
    }

    IEnumerator MyCoroutine() {
        coroutineRunning = true;
        button.SetActive(!button.activeSelf);
        yield return new WaitForSeconds(rateOfBlinking);


        coroutineRunning = false;
    }
}
