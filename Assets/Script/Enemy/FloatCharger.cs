using UnityEngine;

public class FloatCharger : EnemyBase
{
    [Header("Float Charger Settings")]
    public float floatDuration = 1.5f;      // waktu mengambang sebelum dash
    public float dashSpeed = 18f;           // kecepatan dash
    public int breachMeterValue = 5;        // nilai breach meter jika lolos

    private enum State { Floating, Dashing }
    private State currentState = State.Floating;

    private float floatTimer;
    private Vector2 dashDirection;
    private Vector2 targetPosition;

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
        // Panggil CheckOutOfBounds manual, skip HandleMovement dari base
        CheckOutOfBounds();

        switch (currentState)
        {
            case State.Floating:
                HandleFloating();
                break;

            case State.Dashing:
                // Movement ditangani rb.velocity, tidak perlu update manual
                break;
        }
    }

    // ===================== MOVEMENT =====================

    protected override void HandleMovement()
    {
        // Dikosongkan — logic movement ditangani di Update via state machine
    }

    void HandleFloating()
    {
        // Gerak lambat ke kiri sambil mengambang
        transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);

        floatTimer -= Time.deltaTime;

        if (floatTimer <= 0f)
        {
            LockAndDash();
        }
    }

    void LockAndDash()
    {
        // Lock posisi player saat ini
        PlayerController player = FindAnyObjectByType<PlayerController>();
        if (player != null)
        {
            dashDirection = (player.transform.position - transform.position).normalized;
        }
        else
        {
            // Fallback jika player tidak ditemukan
            dashDirection = Vector2.left;
        }

        currentState = State.Dashing;
        rb.linearVelocity = dashDirection * dashSpeed;
    }

    // ===================== BOUNDARY =====================

    protected override void CheckOutOfBounds()
    {
        if (transform.position.x < leftBoundary)
            OnEscaped();
    }

    protected override void OnEscaped()
    {
        // Tambah breach meter
        // GameManager.Instance.AddBreachMeter(breachMeterValue);
        GameManager.Instance.AddEnemyEscaped();

        Destroy(gameObject);
    }

    // ===================== COLLISION =====================

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
    }
}
