using UnityEngine;
using UnityEngine.InputSystem;

public class UIController : MonoBehaviour
{
    [SerializeField] GameObject pauseScreen;
    bool isPaused = false;
    public void Pause()
    {
        if (isPaused)
        {
            Time.timeScale = 1f;
            pauseScreen.SetActive(false);
            isPaused = false;
        }
        else
        {
            Time.timeScale = 0f;
            pauseScreen.SetActive(true);
            isPaused = true;
        }
    }
    public void Quit(InputAction.CallbackContext context)
    {
        if (context.performed)
            Application.Quit();
    }
    public void Resume()
    {
        Time.timeScale = 1f;
        pauseScreen.SetActive(false);
        isPaused = false;
    }
    public void Quit()
    {
        Application.Quit();
    }
}
