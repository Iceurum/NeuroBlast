using UnityEngine;

public class TumorBuffB : EnemyBase
{
    [Header("Tumor Buff B - Power Up Drop")]
    public float zigzagAmplitude = 2f;      // seberapa jauh zigzag
    public float zigzagFrequency = 1.5f;    // seberapa cepat zigzag

    [Header("Drop")]
    public GameObject buffPickupPrefab;     // assign BuffPickup prefab di Inspector

    private float startY;
    private bool initialized = false;

    // ===================== LIFECYCLE =====================

    void Start()
    {
        startY = transform.position.y;
        initialized = true;
    }

    void Update()
    {
        CheckOutOfBounds();
    }

    // ===================== MOVEMENT =====================

    protected override void HandleMovement()
    {
        if (!initialized) return;

        // Bergerak ke kiri + zigzag vertikal
        float newX = transform.position.x - moveSpeed * Time.deltaTime;
        float newY = startY + Mathf.Sin(Time.time * zigzagFrequency) * zigzagAmplitude;

        transform.position = new Vector2(newX, newY);
    }

    // ===================== DEATH OVERRIDE =====================

    protected override void Die()
    {
        // Drop random power-up pickup
        if (buffPickupPrefab != null)
            Instantiate(buffPickupPrefab, transform.position, Quaternion.identity);

        base.Die();
    }
}
