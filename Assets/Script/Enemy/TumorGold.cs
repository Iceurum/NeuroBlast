using UnityEngine;

public class TumorGold : EnemyBase
{
    [Header("Tumor Gold - Power Up Drop")]
    public float zigzagAmplitude = 2f;
    public float zigzagFrequency = 1.5f;

    [Header("Drop")]
    public GameObject buffPickupPrefab;     // assign BuffPickup prefab di Inspector

    private float startY;
    private bool initialized = false;

    // ===================== LIFECYCLE =====================

    protected override void Start()
    {
        base.Start();
        startY = transform.position.y;
        initialized = true;
    }

    protected override void Update()
    {
        HandleMovement();
        CheckOutOfBounds();
    }

    // ===================== MOVEMENT =====================

    protected override void HandleMovement()
    {
        if (!initialized) return;

        float newX = transform.position.x - moveSpeed * Time.deltaTime;
        float newY = startY + Mathf.Sin(Time.time * zigzagFrequency) * zigzagAmplitude;
        transform.position = new Vector2(newX, newY);
    }

    // ===================== DEATH OVERRIDE =====================

    protected override void Die()
    {
        if (buffPickupPrefab != null)
            Instantiate(buffPickupPrefab, transform.position, Quaternion.identity);

        base.Die();
    }
}
