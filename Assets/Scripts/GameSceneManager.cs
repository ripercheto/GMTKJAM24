using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    public int victoryScene, defeatScene;
    private static GameSceneManager sceneManager;

    private void Awake()
    {
        sceneManager = this;
    }

    public static void Victory()
    {
        SceneManager.LoadScene(sceneManager.victoryScene);
    }

    public static void Defeat()
    {
        SceneManager.LoadScene(sceneManager.defeatScene);
    }
}