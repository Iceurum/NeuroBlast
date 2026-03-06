using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [Header("UI")]
    public GameObject pauseMenuPanel;       // assign Panel pause di Inspector

    private bool isPaused = false;

    // ===================== LIFECYCLE =====================

    void Start()
    {
        // Pastikan pause menu tersembunyi saat mulai
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);
    }

    void Update()
    {
        // Trigger pause via keyboard Escape
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            TogglePause();
    }

    // ===================== PAUSE LOGIC =====================

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
        Time.timeScale = 0f;                // freeze semua gameplay

        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(true);
    }

    public void Resume()
    {
        isPaused = false;
        Time.timeScale = 1f;                // resume gameplay

        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);
    }

    // ===================== BUTTON ACTIONS =====================

    public void OnResumeButton()
    {
        Resume();
    }

    public void OnMainMenuButton()
    {
        Time.timeScale = 1f;                // reset timeScale sebelum pindah scene
        SceneManager.LoadScene("MainMenu");
    }

    // ===================== CLEANUP =====================

    void OnDestroy()
    {
        // Pastikan timeScale reset kalau scene di-destroy
        Time.timeScale = 1f;
    }
}
