using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{   
    // switches scene to main menu
    public void ToTitleScreen() {
        SceneManager.LoadSceneAsync("Main Menu");
    }

    // assumes game is already paused via changing time scale to 0
    // changes it back to 1
    public void Unpause() {
        Time.timeScale = 1;
        // loading generic scene for now after unpausing
        SceneManager.LoadSceneAsync("SampleScene");
    }
}
