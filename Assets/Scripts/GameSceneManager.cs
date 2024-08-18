using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    private static GameSceneManager sceneManager;
    private static bool gameEnded;
    public int victoryScene, defeatScene;

    private void Awake()
    {
        sceneManager = this;
    }

    public static void Victory()
    {
        if (gameEnded)
        {
            return;
        }
        gameEnded = true;
        sceneManager.StartCoroutine(DelayedEnd(sceneManager.victoryScene));
    }

    public static void Defeat()
    {
        if (gameEnded)
        {
            return;
        }
        gameEnded = true;
        sceneManager.StartCoroutine(DelayedEnd(sceneManager.defeatScene));
    }

    private static IEnumerator DelayedEnd(int scene)
    {
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(scene);
    }
}