using UnityEngine;

public class Enemy2 : MonoBehaviour
{

    public Transform player;

    public int maxHealth = 100;
    private int currentHealth;

    public float moveSpeed = 6f;
    public float hoverDuration = 1.5f;
    public float verticalAdjustSpeed = 5f;

    private float hoverTimer;
    private bool isCharging = false;
    private float lockedY;

    void Start()
    {
        currentHealth = maxHealth;
        hoverTimer = hoverDuration;
    }

    void Update()
    {
        if (!isCharging)
        {
            HoverPhase();
        }
        else
        {
            ChargePhase();
        }
    }

    void HoverPhase()
    {
        hoverTimer -= Time.deltaTime;

        if (hoverTimer <= 0f)
        {
            LockPlayerPosition();
            isCharging = true;
        }
    }

    void LockPlayerPosition()
    {
        if (player != null)
        {
            lockedY = player.position.y;
        }
        else
        {
            lockedY = transform.position.y;
        }
    }

    void ChargePhase()
    {
        // Gerak ke kiri
        float newX = transform.position.x - moveSpeed * Time.deltaTime;

        // Menuju posisi Y yang sudah di-lock
        float newY = Mathf.MoveTowards(
            transform.position.y,
            lockedY,
            verticalAdjustSpeed * Time.deltaTime
        );

        transform.position = new Vector3(newX, newY, transform.position.z);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        Destroy(gameObject);
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}