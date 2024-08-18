using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToMainMenu : MonoBehaviour
{
    private float nextSceneTime;

    void Start()
    {
        nextSceneTime = Time.time + 2;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time < nextSceneTime)
        {
            return;
        }
        if (PlayerInput.PressingAction)
        {
            SceneManager.LoadScene(0);
        }
    }
}