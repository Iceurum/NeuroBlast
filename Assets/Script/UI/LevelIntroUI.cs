using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class LevelIntroUI : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI levelTitleText;  
    public float showDuration = 1.5f;      
    public float fadeDuration = 1f;         

    public IEnumerator PlayIntro(string levelName)
    {
        if (levelTitleText == null) yield break;

        // Set text dan tampilkan
        
        levelTitleText.gameObject.SetActive(true);

        // Fade in
        yield return StartCoroutine(FadeText(0f, 1f, 0.5f));

        // Diam sebentar
        yield return new WaitForSeconds(showDuration);

        // Fade out
        yield return StartCoroutine(FadeText(1f, 0f, fadeDuration));

        levelTitleText.gameObject.SetActive(false);
    }

    IEnumerator FadeText(float from, float to, float duration)
    {
        float elapsed = 0f;
        Color color = levelTitleText.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(from, to, elapsed / duration);
            levelTitleText.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        levelTitleText.color = new Color(color.r, color.g, color.b, to);
    }
}
