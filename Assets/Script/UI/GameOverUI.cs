using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    [Header("Video (Opsional)")]
    public VideoPlayer videoPlayer;

    [Header("Buttons Panel")]
    public CanvasGroup buttonsPanel;        // CanvasGroup untuk fade in
    public float buttonDelay = 7f;          // delay sebelum button muncul
    public float fadeDuration = 1f;         // durasi fade in button

    [Header("Confirm Popup")]
    public GameObject confirmPopupPanel;
    public TextMeshProUGUI confirmText;

    // ===================== LIFECYCLE =====================

    void Start()
    {
        // Sembunyikan button panel dulu
        if (buttonsPanel != null)
        {
            buttonsPanel.alpha = 0f;
            buttonsPanel.interactable = false;
            buttonsPanel.blocksRaycasts = false;
        }

        // Sembunyikan confirm popup
        if (confirmPopupPanel != null)
            confirmPopupPanel.SetActive(false);

        // Set confirm text
        if (confirmText != null)
            confirmText.text = "Kembali ke Main Menu?\nSemua progres tidak disimpan\ndan kamu harus mengulang dari awal.";

        // Play video kalau ada
        if (videoPlayer != null)
            videoPlayer.Play();

        // Mulai timer untuk munculkan button
        StartCoroutine(ShowButtonsAfterDelay());
    }

    // ===================== SHOW BUTTONS =====================

    IEnumerator ShowButtonsAfterDelay()
    {
        // Tunggu delay
        yield return new WaitForSeconds(buttonDelay);

        // Fade in button panel
        yield return StartCoroutine(FadeInButtons());
    }

    IEnumerator FadeInButtons()
    {
        if (buttonsPanel == null) yield break;

        buttonsPanel.interactable = true;
        buttonsPanel.blocksRaycasts = true;

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            buttonsPanel.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            yield return null;
        }

        buttonsPanel.alpha = 1f;
    }

    public void OnRetryButton()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.RetryFromLevel(GameManager.Instance.lastLevelIndex);
        else
            SceneManager.LoadScene("Level1");
    }

    public void OnMainMenuButton()
    {
        if (confirmPopupPanel != null)
            confirmPopupPanel.SetActive(true);
    }

    public void OnConfirmMainMenu()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.GoToMainMenu();
        else
            SceneManager.LoadScene("MainMenu");
    }

    public void OnCancelMainMenu()
    {
        if (confirmPopupPanel != null)
            confirmPopupPanel.SetActive(false);
    }
}