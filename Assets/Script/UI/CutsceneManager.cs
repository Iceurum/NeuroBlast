using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.InputSystem;
using TMPro;

public class CutsceneManager : MonoBehaviour
{
    [Header("UI References")]
    public Image backgroundImage;           // Background gambar cutscene
    public Image fadePanel;                 // Panel untuk fade (hitam/putih)

    [Header("Dialogue Box")]
    public GameObject dialogueBox;          // Panel kotak dialog bawah
    public TextMeshProUGUI speakerNameText; // Nama karakter (misal: "Dr. Reeves")
    public GameObject speakerNamePanel;     // Panel background nama speaker
    public TextMeshProUGUI dialogueBodyText;// Teks isi dialog

    [Header("Slides")]
    public Sprite[] slides;                 // Picture2 - Picture6 (isi di Inspector)

    [Header("Timing")]
    public float fadeDuration = 0.5f;
    public float typewriterSpeed = 0.03f;   // Kecepatan typewriter per karakter
    public float autoAdvanceDelay = 3.5f;   // Auto lanjut setelah dialog selesai jika tidak diklik

    [Header("Settings")]
    public string cutsceneName = "OpeningCutscene";

    // ===================== DATA =====================

    private struct SlideData
    {
        public string speaker;
        public string dialogue;
        public SlideData(string s, string d) { speaker = s; dialogue = d; }
    }

    private SlideData[] slideData = new SlideData[]
    {
        new SlideData("Doctor", "Tumor ini berkembang sangat cepat… dalam 3 minggu tekanan akan fatal."),
        new SlideData("Doctor", "Operasi terlalu beresiko. Kita butuh solusi lain."),
        new SlideData("Ilmuwan",    "Kami akan mengirim unit NK-67 untuk menghancurkannya dari dalam."),
        new SlideData("Ilmuwan",    "Robot ini masih eksperimental. Namun, tidak ada cara lain."),
        new SlideData("",           ""),
    };

    private bool playerInput = false;



    void Start()
    {
        dialogueBox.SetActive(false);
        SetFadeColor(Color.black, 1f);
        StartCoroutine(PlayCutscene());
    }

    void Update()
    {
    if (Mouse.current.leftButton.wasPressedThisFrame ||
        Keyboard.current.spaceKey.wasPressedThisFrame ||
        Keyboard.current.enterKey.wasPressedThisFrame)
        playerInput = true;
    }



    IEnumerator PlayCutscene()
    {
        for (int i = 0; i < slides.Length; i++)
        {
            backgroundImage.sprite = slides[i];

            if (i == slides.Length - 1)
            {
                yield return StartCoroutine(Fade(1f, 0f, fadeDuration, Color.black));
                yield return new WaitForSeconds(2f);
                yield return StartCoroutine(Fade(0f, 1f, fadeDuration, Color.white));
                break;
            }

            // Fade in slide
            yield return StartCoroutine(Fade(1f, 0f, fadeDuration, Color.black));

            // Tampilkan dialogue jika ada
            if (!string.IsNullOrEmpty(slideData[i].dialogue))
            {
                yield return StartCoroutine(ShowDialogue(slideData[i].speaker, slideData[i].dialogue));
                yield return new WaitForSeconds(0.3f);
            }
            else
            {
                yield return new WaitForSeconds(2.5f);
            }

            // Sembunyikan dialogue box lalu fade out ke slide berikutnya
            yield return StartCoroutine(HideDialogueBox());
            yield return StartCoroutine(Fade(0f, 1f, fadeDuration, Color.black));
        }

        Advance();
    }

    // ===================== DIALOGUE BOX =====================

    IEnumerator ShowDialogue(string speaker, string dialogue)
    {
        // Setup konten
        speakerNameText.text = speaker;
        dialogueBodyText.text = "";
        speakerNamePanel.SetActive(!string.IsNullOrEmpty(speaker));

        // Ambil atau tambah CanvasGroup untuk animasi alpha
        CanvasGroup cg = dialogueBox.GetComponent<CanvasGroup>();
        if (cg == null) cg = dialogueBox.AddComponent<CanvasGroup>();

        RectTransform rt = dialogueBox.GetComponent<RectTransform>();
        float slideOffset = 50f;
        Vector2 shownPos = rt.anchoredPosition;
        Vector2 hiddenPos = shownPos - new Vector2(0, slideOffset);

        // Slide up + fade in
        cg.alpha = 0f;
        rt.anchoredPosition = hiddenPos;
        dialogueBox.SetActive(true);

        float elapsed = 0f;
        float inDuration = 0.35f;
        while (elapsed < inDuration)
        {
            elapsed += Time.deltaTime;
            float t = EaseOut(elapsed / inDuration);
            cg.alpha = Mathf.Lerp(0f, 1f, t);
            rt.anchoredPosition = Vector2.Lerp(hiddenPos, shownPos, t);
            yield return null;
        }
        cg.alpha = 1f;
        rt.anchoredPosition = shownPos;

        // Typewriter
        playerInput = false;


        for (int c = 0; c < dialogue.Length; c++)
        {
            // Jika player klik saat typewriter berjalan = langsung tampilkan semua
            if (playerInput)
            {
                dialogueBodyText.text = dialogue;
                playerInput = false;
                break;
            }
            dialogueBodyText.text += dialogue[c];
            yield return new WaitForSeconds(typewriterSpeed);
        }

        // Tunggu input player atau auto-advance
        playerInput = false;
        float waitTimer = 0f;
        while (waitTimer < autoAdvanceDelay && !playerInput)
        {
            waitTimer += Time.deltaTime;
            yield return null;
        }
        playerInput = false;
    }

    IEnumerator HideDialogueBox()
    {
        CanvasGroup cg = dialogueBox.GetComponent<CanvasGroup>();
        if (cg == null) { dialogueBox.SetActive(false); yield break; }

        float elapsed = 0f;
        float outDuration = 0.25f;
        float startAlpha = cg.alpha;

        while (elapsed < outDuration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(startAlpha, 0f, elapsed / outDuration);
            yield return null;
        }

        cg.alpha = 0f;
        dialogueBox.SetActive(false);
    }

    // ===================== FADE =====================

    IEnumerator Fade(float from, float to, float duration, Color color)
    {
        fadePanel.color = new Color(color.r, color.g, color.b, from);
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            fadePanel.color = new Color(color.r, color.g, color.b, Mathf.Lerp(from, to, elapsed / duration));
            yield return null;
        }
        fadePanel.color = new Color(color.r, color.g, color.b, to);
    }

    void SetFadeColor(Color color, float alpha) =>
        fadePanel.color = new Color(color.r, color.g, color.b, alpha);

    float EaseOut(float t) => 1f - Mathf.Pow(1f - Mathf.Clamp01(t), 3f);

    // ===================== ADVANCE =====================

    void Advance()
    {
        StopAllCoroutines();

        if (GameManager.Instance != null)
            GameManager.Instance.OnCutsceneFinished(cutsceneName);
        else
        {
            Debug.LogWarning("GameManager tidak ditemukan!");
            UnityEngine.SceneManagement.SceneManager.LoadScene("Level1");
        }
    }
}