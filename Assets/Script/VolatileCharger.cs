using UnityEngine;

public class VolatileCharger : EnemyBase
{
    [Header("Dash Settings")]
    public float floatDuration = 1.5f;
    public float dashSpeed = 18f;

    [Header("Explosion Settings")]
    public int explosionDamage = 25;
    public float explosionRadius = 2f;
    public float explosionDelay = 0.3f;
    public GameObject explosionEffectPrefab;    // optional: efek visual ledakan

    private enum State { Floating, Dashing }
    private State currentState = State.Floating;

    private float floatTimer;
    private Vector2 dashDirection;
    private Rigidbody2D rb;

    // ===================== LIFECYCLE =====================

    protected override void Start()
    {
        base.Start();

        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;

        floatTimer = floatDuration;
    }

    protected override void Update()
    {
        CheckOutOfBounds();

        switch (currentState)
        {
            case State.Floating:
                HandleFloating();
                break;

            case State.Dashing:
                // Movement ditangani rb.linearVelocity
                break;
        }
    }

    // ===================== MOVEMENT =====================

    protected override void HandleMovement()
    {
        // Dikosongkan — ditangani state machine di Update
    }

    void HandleFloating()
    {
        transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);

        floatTimer -= Time.deltaTime;

        if (floatTimer <= 0f)
            LockAndDash();
    }

    void LockAndDash()
    {
        PlayerController player = FindAnyObjectByType<PlayerController>();
        if (player != null)
            dashDirection = (player.transform.position - transform.position).normalized;
        else
            dashDirection = Vector2.left;

        currentState = State.Dashing;
        rb.linearVelocity = dashDirection * dashSpeed;
    }

    // ===================== DEATH & EXPLOSION =====================

    protected override void Die()
    {
        if (isDead) return;
        isDead = true;

        // Stop movement
        rb.linearVelocity = Vector2.zero;

        // Explosion delay sebelum aktif
        Invoke(nameof(Explode), explosionDelay);
    }

    void Explode()
    {
        // Spawn efek visual jika ada
        if (explosionEffectPrefab != null)
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);

        // Deteksi player dalam radius ledakan
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Player"))
                hit.GetComponent<PlayerController>()?.TakeDamage(explosionDamage);
        }

        // Pakai TryDropItem dari base class langsung
        TryDropItem();
        GameManager.Instance.AddEnemyDestroyed();

        Destroy(gameObject);
    }

    // ===================== BOUNDARY =====================

    protected override void CheckOutOfBounds()
    {
        if (transform.position.x < leftBoundary)
            OnEscaped();
    }
}