using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public InputAction MoveAction;
    public float speed = 6f;
    private Vector2 moveDirection = new Vector2(1, 0);
    private Vector2 moveInput;

    [Header("Combat")]
    public GameObject projectilePrefab;
    public int bulletDamage = 25;

    [Header("Health")]
    public int maxHealth = 100;
    public float regenRate = 5f;
    private int currentHealth;

    [Header("Boundary")]
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;

    [Header("Intro/Outro")]
    public float introSpeed = 20f;
    public float outroSpeed = 20f;

    private Rigidbody2D rb;
    private bool inputEnabled = false;
    private bool isOutro = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
    }

    private void Start()
    {
        MoveAction.Enable();
        currentHealth = maxHealth;
        inputEnabled = false;
    }

    private void Update()
    {
        if (!inputEnabled) return;

        moveInput = MoveAction.ReadValue<Vector2>();

        if (!Mathf.Approximately(moveInput.x, 0.0f) || !Mathf.Approximately(moveInput.y, 0.0f))
        {
            moveDirection.Set(moveInput.x, moveInput.y);
            moveDirection.Normalize();
        }

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
            HandleShoot();

        HandleRegen();
    }

    private void FixedUpdate()
    {
        if (!inputEnabled || isOutro) return;

        Vector2 position = rb.position + moveInput * speed * Time.fixedDeltaTime;
        rb.MovePosition(position);
        ClampPosition();
    }

    public IEnumerator PlayIntroAnimation(Vector2 spawnTarget)
    {
        Debug.Log("PlayIntroAnimation START - target: " + spawnTarget);

        // Mulai dari offscreen kiri
        transform.position = new Vector2(spawnTarget.x - 15f, spawnTarget.y);

        while (Vector2.Distance(transform.position, spawnTarget) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                spawnTarget,
                introSpeed * Time.deltaTime
            );
            yield return null;
        }

        transform.position = spawnTarget;
        inputEnabled = true;
        Debug.Log("PlayIntroAnimation SELESAI - input aktif!");
    }


    public IEnumerator PlayOutroAnimation()
    {
        isOutro = true;
        inputEnabled = false;

        Vector2 centerTarget = new Vector2(transform.position.x, 0f);
        while (Vector2.Distance(transform.position, centerTarget) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                centerTarget,
                outroSpeed * Time.deltaTime
            );
            yield return null;
        }

        // Maju ke kanan sampai hilang dari layar
        Vector3 exitTarget = new Vector3(20f, 0f, 0f);
        while (transform.position.x < exitTarget.x)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                exitTarget,
                outroSpeed * Time.deltaTime
            );
            yield return null;
        }

        if (GameManager.Instance != null)
            GameManager.Instance.OnOutroAnimationComplete();
    }

    void HandleShoot()
    {
        if (projectilePrefab == null)
        {
            Debug.LogError("projectilePrefab KOSONG! Assign di Inspector!");
            return;
        }

        Vector2 shootDirection = Vector2.right;

        GameObject projectileObject = Instantiate(
            projectilePrefab,
            rb.position + Vector2.up * 0.5f,
            Quaternion.identity
        );

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        if (projectile == null)
        {
            Debug.LogError("Komponen Projectile tidak ada di prefab!");
            return;
        }

        projectile.Launch(shootDirection, 15f, bulletDamage);
    }

    void ClampPosition()
    {
        if (maxX > minX && maxY > minY)
        {
            Vector3 pos = transform.position;
            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            pos.y = Mathf.Clamp(pos.y, minY, maxY);
            transform.position = pos;
        }
    }

    void HandleRegen()
    {
        if (currentHealth < maxHealth)
        {
            currentHealth += Mathf.RoundToInt(regenRate * Time.deltaTime);
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        }
    }

    public void TakeDamage(int amount)
    {
        if (!inputEnabled) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        Debug.Log("Player Dead");
        if (GameManager.Instance != null)
            GameManager.Instance.PlayerDied();
        Destroy(gameObject);
    }

    public int GetCurrentHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;
    public bool IsInputEnabled() => inputEnabled;
}