using UnityEngine;

public class Enemy1 : EnemyBase
{
    [Header("Movement")]
    public float moveSpeed = 3f;
    public float amplitude = 1.5f;
    public float frequency = 2f;

    [Header("Shooting")]
    public GameObject projectilePrefab;
    public float shootInterval = 2f;
    public float projectileSpeed = 6f;

    private float shootTimer;
    private float startY;
    private float randomOffset;

    private Vector2 moveDirection = Vector2.left;

    protected override void Start()
    {
        base.Start();
        startY = transform.position.y;
        randomOffset = Random.Range(0f, 10f);
        shootTimer = shootInterval;
    }

    void Update()
    {
        Move();
        Shoot();
    }

    void Move()
    {
        float newX = transform.position.x - moveSpeed * Time.deltaTime;
        float newY = startY + Mathf.Sin((Time.time + randomOffset) * frequency) * amplitude;
        transform.position = new Vector2(newX, newY);
    }

    void Shoot()
    {
        shootTimer -= Time.deltaTime;
        if (shootTimer <= 0f)
        {
            FireProjectile();
            shootTimer = shootInterval;
        }
    }

    void FireProjectile()
    {
        if (projectilePrefab == null) return;

        GameObject proj = Instantiate(
            projectilePrefab,
            (Vector2)transform.position + moveDirection * 0.6f,
            Quaternion.identity
        );

        EnemyProjectile projectile = proj.GetComponent<EnemyProjectile>();
        if (projectile != null)
        {
            projectile.Launch(moveDirection, projectileSpeed);
        }
    }
}