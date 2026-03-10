using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [Header("UI")]
    public Slider healthSlider;            
    public Image fillImage;                 

    [Header("Color")]
    public Color highHealthColor = Color.green;
    public Color midHealthColor = Color.yellow;
    public Color lowHealthColor = Color.red;

    private PlayerController player;

    void Start()
{
    player = FindAnyObjectByType<PlayerController>();

    if (healthSlider != null)
    {
        healthSlider.minValue = 0;
        healthSlider.maxValue = 100;
        healthSlider.value = 100;
        healthSlider.gameObject.SetActive(false);
    }
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
        int max = player.GetMaxHealth();

        healthSlider.value = current;


        if (fillImage != null)
        {
            float ratio = (float)current / max;

            if (ratio > 0.6f)
                fillImage.color = highHealthColor;
            else if (ratio > 0.3f)
                fillImage.color = midHealthColor;
            else
                fillImage.color = lowHealthColor;
        }
    }
}
