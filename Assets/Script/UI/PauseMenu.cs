using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [Header("UI")]
    public GameObject pauseMenuPanel;       

    private bool isPaused = false;

    void Start()
    {
        
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            TogglePause();
    }

    public void TogglePause()
    {
        if (isPaused)
            Resume();
        else
            Pause();
    }

    public void Pause()
    {
        isPaused = true;

        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(true);
    }

    public void Resume()
    {
        isPaused = false;
        Time.timeScale = 1f;         

        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);
    }

    public void OnResumeButton()
    {
        Resume();
    }

    public void OnMainMenuButton()
    {
        Time.timeScale = 1f;              
        SceneManager.LoadScene("MainMenu");
    }

    void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}
