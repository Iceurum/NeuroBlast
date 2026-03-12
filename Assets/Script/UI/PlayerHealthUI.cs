using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [Header("UI")]
    public Slider healthSlider;
    public Image fillImage;             // Fill Area > Fill (Image component)
    public Image backgroundImage;       // Background image di Slider (opsional)

    [Header("Fill Sprites")]
    public Sprite fullSprite;           // full.png  — HP > 50%
    public Sprite halfSprite;           // half.png  — HP <= 50%

    [Header("Background Sprite")]
    public Sprite barSprite;            // bar.png   — assign ke backgroundImage

    private PlayerController player;
    private float lastRatio = -1f;      

    void Start()
    {
        player = FindAnyObjectByType<PlayerController>();

        // Pasang bar.png ke background
        if (backgroundImage != null && barSprite != null)
            backgroundImage.sprite = barSprite;

        if (healthSlider != null)
        {
            healthSlider.minValue = 0;
            healthSlider.maxValue = 100;
            healthSlider.value = 100;
            healthSlider.gameObject.SetActive(false);
        }
        SetFillSprite(1f);
    }

    void Update()
    {
        if (player == null)
        {
            player = FindAnyObjectByType<PlayerController>();

            if (player != null && healthSlider != null)
            {
                healthSlider.maxValue = player.GetMaxHealth();
                healthSlider.gameObject.SetActive(true);
            }
            return;
        }

        UpdateHealthBar();
    }

    void UpdateHealthBar()
    {
        if (healthSlider == null) return;

        int current = player.GetCurrentHealth();
        int max     = player.GetMaxHealth();

        healthSlider.value = current;

        float ratio = (float)current / max;

        if (Mathf.Abs(ratio - lastRatio) > 0.001f)
        {
            SetFillSprite(ratio);
            lastRatio = ratio;
        }
    }

    void SetFillSprite(float ratio)
    {
        if (fillImage == null) return;

        if (ratio > 0.5f)
        {
            if (fullSprite != null) fillImage.sprite = fullSprite;
        }
        else
        {
            if (halfSprite != null) fillImage.sprite = halfSprite;
        }
    }
}