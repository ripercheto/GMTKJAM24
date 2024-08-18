using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private void Update()
    {
        if (PlayerInput.PressingAction)
        {
            SceneManager.LoadSceneAsync(1);
            enabled = false;
        }
    }

    public void PlayGame()
    {
        SceneManager.LoadSceneAsync(1);
    }
}