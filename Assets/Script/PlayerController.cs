using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public InputAction MoveAction;
    public float speed = 6f;
    Vector2 moveDirection = new Vector2(1, 0);

    [Header("Combat")]
    public InputAction ShootAction;
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

    private Rigidbody2D rb;


    private Vector2 moveInput;
    private Vector2 lastMoveDirection = Vector2.right;

    private void Start()
    {
        MoveAction.Enable();
        ShootAction.Enable();

        rb = GetComponent<Rigidbody2D>();

        rb.gravityScale = 0f; // penting untuk free movement
        currentHealth = maxHealth;
    }

    private void Update()
    {
        moveInput = MoveAction.ReadValue<Vector2>();

        if (!Mathf.Approximately(moveInput.x, 0.0f) || !Mathf.Approximately(moveInput.y, 0.0f))
        {
            moveDirection.Set(moveInput.x, moveInput.y);
            moveDirection.Normalize();
        }

        HandleShoot();
        HandleRegen();
    }

    private void FixedUpdate()
    {
        Vector2 position = rb.position + moveInput * speed * Time.fixedDeltaTime;
        rb.MovePosition(position);
        ClampPosition();
    }

    void HandleShoot()
    {
        if (ShootAction.WasPressedThisFrame())
        {
            Vector2 shootDirection = lastMoveDirection;

            GameObject projectile = Instantiate(
                projectilePrefab,
                transform.position,
                Quaternion.identity
            );

           // projectile.GetComponent<Projectile>()
           //     .Launch(shootDirection, 500f, bulletDamage);

        }
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
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);


        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        Debug.Log("Player Dead");
        GameManager.Instance.PlayerDied();
        Destroy(gameObject);

    }
}