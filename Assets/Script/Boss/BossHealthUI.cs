using UnityEngine;
using UnityEngine.UI;

public class BossHealthUI : MonoBehaviour
{
    [Header("UI")]
    public Slider healthSlider;
    public Image fillImage;

    [Header("Color")]
    public Color highHealthColor = Color.red;
    public Color midHealthColor = new Color(1f, 0.5f, 0f);
    public Color lowHealthColor = Color.yellow;

    private BossController boss;

    void Start()
    {
        boss = FindAnyObjectByType<BossController>();

        if (healthSlider != null)
        {
            healthSlider.minValue = 0;
            healthSlider.maxValue = 2000;
            healthSlider.value = 2000;
            healthSlider.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (boss == null)
        {
            boss = FindAnyObjectByType<BossController>();
            if (boss != null && healthSlider != null)
            {
                healthSlider.maxValue = boss.GetMaxHP();
                healthSlider.gameObject.SetActive(true);
            }
            return;
        }

        UpdateHealthBar();
    }

    void UpdateHealthBar()
    {
        if (healthSlider == null) return;

        int current = boss.GetCurrentHP();
        int max = boss.GetMaxHP();

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
