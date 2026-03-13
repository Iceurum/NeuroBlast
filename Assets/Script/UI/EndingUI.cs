using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class EndingUI : MonoBehaviour
{
    [Header("UI")]
    public Image backgroundImage;           // image background
    public Image textboxImage;              // image kotak teks
    public TextMeshProUGUI endingText;      // text ending
    public CanvasGroup buttonsPanel;        // panel button MainMenu

    [Header("Timing")]
    public float imageTextFadeIn = 1f;      // durasi fade in semua elemen
    public float buttonDelay = 5f;          // detik sebelum button muncul
    public float fadeDuration = 1f;         // durasi fade in button

    // ===================== LIFECYCLE =====================

    void Start()
    {
        if (buttonsPanel != null)
        {
            buttonsPanel.alpha = 0f;
            buttonsPanel.interactable = false;
            buttonsPanel.blocksRaycasts = false;
        }

        StartCoroutine(PlayEnding());
    }

    // ===================== ENDING SEQUENCE =====================

    IEnumerator PlayEnding()
    {
        yield return StartCoroutine(FadeInAll());
        yield return new WaitForSeconds(buttonDelay);
        yield return StartCoroutine(FadeInButtons());
    }

    IEnumerator FadeInAll()
    {
        // Set alpha awal ke 0
        SetAlpha(backgroundImage, 0f);
        SetAlpha(textboxImage, 0f);
        SetTextAlpha(endingText, 0f);

        float elapsed = 0f;
        while (elapsed < imageTextFadeIn)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsed / imageTextFadeIn);

            SetAlpha(backgroundImage, alpha);
            SetAlpha(textboxImage, alpha);
            SetTextAlpha(endingText, alpha);

            yield return null;
        }

        SetAlpha(backgroundImage, 1f);
        SetAlpha(textboxImage, 1f);
        SetTextAlpha(endingText, 1f);
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

    // ===================== HELPERS =====================

    void SetAlpha(Image image, float alpha)
    {
        if (image == null) return;
        Color c = image.color;
        image.color = new Color(c.r, c.g, c.b, alpha);
    }

    void SetTextAlpha(TextMeshProUGUI text, float alpha)
    {
        if (text == null) return;
        Color c = text.color;
        text.color = new Color(c.r, c.g, c.b, alpha);
    }

    // ===================== BUTTON =====================

    public void OnMainMenuButton()
    {
        Debug.Log("MainMenu button diklik!");
        if (GameManager.Instance != null)
            GameManager.Instance.GoToMainMenu();
        else
            SceneManager.LoadScene("MainMenu");
    }
}