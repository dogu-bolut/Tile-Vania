using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Animations;

public class LevelExit : MonoBehaviour
{
    [SerializeField] float LevelLoadDelay = 0.5f;
    [SerializeField] float LevelExitSlowMode = 0.6f;    
   void OnTriggerEnter2D(Collider2D other)
    {
        StartCoroutine(LoadNextLevel());
    }
    IEnumerator LoadNextLevel()
    {
        Time.timeScale = LevelExitSlowMode;
        yield return new WaitForSecondsRealtime(LevelLoadDelay);
        Time.timeScale = 1f;

        var currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex + 1);
    }
}
