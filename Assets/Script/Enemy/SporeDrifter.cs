using UnityEngine;

public class SporeDrifter : EnemyBase
{
    [Header("Combat")]
    public GameObject projectilePrefab;
    public float fireRate = 2f;          // menembak setiap 2 detik
    public float fireDelay = 0.5f;       // telegraph delay sebelum tembak
    public int bulletDamage = 10;
    public float bulletSpeed = 8f;

    private float fireTimer;
    private bool isTelegraphing = false;

    // ===================== LIFECYCLE =====================

    protected override void Start()
    {
        base.Start();
        // Mulai timer dengan delay awal supaya tidak langsung tembak saat spawn
        fireTimer = fireDelay + 1f;
    }

    protected override void Update()
    {
        base.Update();
        HandleShooting();
    }

    // ===================== MOVEMENT =====================

    protected override void HandleMovement()
    {
        // Bergerak horizontal kanan ke kiri
        transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
    }

    // ===================== SHOOTING =====================

    void HandleShooting()
    {
        if (isTelegraphing) return;

        fireTimer -= Time.deltaTime;

        if (fireTimer <= 0f)
        {
            isTelegraphing = true;
            fireTimer = fireRate;

            // Telegraph delay sebelum tembak
            Invoke(nameof(Shoot), fireDelay);
        }
    }

    void Shoot()
    {
        if (isDead) return;
        isTelegraphing = false;

        if (projectilePrefab == null)
        {
            Debug.LogWarning("SporeDrifter: projectilePrefab belum di-assign!");
            return;
        }

        // Spawn dan tembak lurus ke kiri
        GameObject projectileObj = Instantiate(
            projectilePrefab,
            transform.position,
            Quaternion.identity
        );

        Projectile projectile = projectileObj.GetComponent<Projectile>();
        if (projectile != null)
            projectile.Launch(Vector2.left, bulletSpeed, bulletDamage);
    }
}
