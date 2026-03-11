using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    [Header("Sprite Animation (8 Frame)")]
    public Image animationImage;        
    public Sprite[] frames;             
    public float fps = 12f;              
    public bool loopAnimation = true;    

    [Header("Buttons Panel")]
    public CanvasGroup buttonsPanel;
    public float buttonDelay = 7f;
    public float fadeDuration = 1f;

    [Header("Confirm Popup")]
    public GameObject confirmPopupPanel;
    public TextMeshProUGUI confirmText;

    void Start()
    {
        if (buttonsPanel != null)
        {
            buttonsPanel.alpha = 0f;
            buttonsPanel.interactable = false;
            buttonsPanel.blocksRaycasts = false;
        }

        if (confirmPopupPanel != null)
            confirmPopupPanel.SetActive(false);

        if (confirmText != null)
            confirmText.text = "Kembali ke Main Menu?\nSemua progres tidak disimpan\ndan kamu harus mengulang dari awal.";

        // Mulai animasi sprite
        if (animationImage != null && frames != null && frames.Length > 0)
            StartCoroutine(PlaySpriteAnimation());

        StartCoroutine(ShowButtonsAfterDelay());
    }

    IEnumerator PlaySpriteAnimation()
    {
        float interval = 1f / fps; // jeda antar frame
        int currentFrame = 0;

        while (true)
        {
            animationImage.sprite = frames[currentFrame];
            currentFrame++;

            // Kalau sudah frame terakhir
            if (currentFrame >= frames.Length)
            {
                if (loopAnimation)
                    currentFrame = 0;  // ulangi dari awal
                else
                    yield break;       // berhenti di frame terakhir
            }

            yield return new WaitForSeconds(interval);
        }
    }

    IEnumerator ShowButtonsAfterDelay()
    {
        yield return new WaitForSeconds(buttonDelay);
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