using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    [Header("Stats")]
    public int maxHP = 50;
    public int contactDamage = 10;
    protected int currentHP;

    [Header("Movement")]
    public float moveSpeed = 2f;

    [Header("Breach")]
    public int breachValue = 5;             // nilai yang ditambahkan ke Breach Meter jika lolos

    [Header("Item Drop")]
    public GameObject itemDropPrefab;
    [Range(0f, 1f)]
    public float dropChance = 0.3f;

    [Header("Boundary")]
    public float leftBoundary = -12f;

    protected bool isDead = false;

    // ===================== LIFECYCLE =====================

    protected virtual void Start()
    {
        currentHP = maxHP;
    }

    protected virtual void Update()
    {
        HandleMovement();
        CheckOutOfBounds();
    }

    // ===================== ABSTRACT =====================

    protected abstract void HandleMovement();

    // ===================== COMBAT =====================

    public virtual void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHP -= amount;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);

        if (currentHP <= 0)
            Die();
    }

    protected virtual void Die()
    {
        if (isDead) return;
        isDead = true;

        TryDropItem();

        if (GameManager.Instance != null)
            GameManager.Instance.AddEnemyDestroyed();
        else
            Debug.LogWarning("GameManager.Instance null!");

        Destroy(gameObject);
    }

    protected virtual void OnEscaped()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddBreachMeter(breachValue);
            GameManager.Instance.AddEnemyEscaped();
        }
        else
            Debug.LogWarning("GameManager.Instance null!");

        Destroy(gameObject);
    }

    // Dipanggil oleh Boundary — destroy tanpa trigger Die() atau OnEscaped()
    // karena Boundary sudah handle sendiri
    public void ForceDestroy()
    {
        isDead = true;
        Destroy(gameObject);
    }

    // ===================== COLLISION =====================

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
                player.TakeDamage(contactDamage);
        }

        if (other.CompareTag("PlayerBullet"))
        {
            Projectile projectile = other.GetComponent<Projectile>();
            if (projectile != null)
                TakeDamage(projectile.GetDamage());
        }
    }

    // ===================== BOUNDARY =====================

    protected virtual void CheckOutOfBounds()
    {
        if (transform.position.x < leftBoundary)
            OnEscaped();
    }

    // ===================== ITEM DROP =====================

    protected void TryDropItem()
    {
        if (itemDropPrefab == null) return;

        if (Random.value <= dropChance)
            Instantiate(itemDropPrefab, transform.position, Quaternion.identity);
    }

    // ===================== ACCESSOR =====================

    public int GetCurrentHP() => currentHP;
    public bool IsDead() => isDead;
}