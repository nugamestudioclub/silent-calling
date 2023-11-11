using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void Play()
    {
        SceneManager.LoadSceneAsync("SampleScene");
    }

    public void Quit() 
    {
        Application.Quit();
        Debug.Log("Player has quit game");
    }

    public void LoadOptions()
    {
        SceneManager.LoadSceneAsync("Options Menu");
    }
}
