using UnityEngine;
using UnityEngine.Video;
using System.Collections;

public class CutsceneManager : MonoBehaviour
{
    [Header("Timer")]
    public float cutsceneDuration = 10f;

    [Header("Video (Opsional - bisa dikosongkan)")]
    public VideoPlayer videoPlayer;

    [Header("Settings")]
    public string cutsceneName = "OpeningCutscene";

    // ===================== LIFECYCLE =====================

    void Start()
    {
        // Play video kalau ada
        if (videoPlayer != null)
        {
            videoPlayer.Play();
            videoPlayer.loopPointReached += OnVideoFinished;
        }

        StartCoroutine(AutoAdvance());
    }

    // ===================== AUTO ADVANCE =====================

    IEnumerator AutoAdvance()
    {
        yield return new WaitForSeconds(cutsceneDuration);
        Advance();
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        Advance();
    }

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
