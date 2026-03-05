using UnityEngine;

public class NeuroShooter : EnemyBase
{
    [Header("Combat")]
    public GameObject projectilePrefab;
    public float fireRate = 2f;
    public float fireDelay = 0.5f;
    public int bulletDamage = 15;
    public float bulletSpeed = 8f;
    public float spreadAngle = 20f;         // sudut spread antar peluru

    [Header("Zigzag Movement")]
    public float zigzagAmplitude = 2f;      // seberapa jauh naik/turun
    public float zigzagFrequency = 2f;      // seberapa cepat zigzag

    private float fireTimer;
    private bool isTelegraphing = false;
    private float startY;

    // ===================== LIFECYCLE =====================

    protected override void Start()
    {
        base.Start();
        startY = transform.position.y;
        fireTimer = fireDelay + 1.5f;       // delay awal sebelum tembak pertama
    }

    protected override void Update()
    {
        base.Update();
        HandleShooting();
    }

    // ===================== MOVEMENT =====================

    protected override void HandleMovement()
    {
        // Gerak horizontal ke kiri
        float newX = transform.position.x - moveSpeed * Time.deltaTime;

        // Zigzag vertikal menggunakan sin wave
        float newY = startY + Mathf.Sin(Time.time * zigzagFrequency) * zigzagAmplitude;

        transform.position = new Vector2(newX, newY);
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

            Invoke(nameof(ShootSpread), fireDelay);
        }
    }

    void ShootSpread()
    {
        if (isDead) return;
        isTelegraphing = false;

        if (projectilePrefab == null)
        {
            Debug.LogWarning("NeuroShooter: projectilePrefab belum di-assign!");
            return;
        }

        // Tembak 3 peluru: lurus, atas, bawah
        FireBullet(Vector2.left);
        FireBullet(Quaternion.Euler(0, 0, spreadAngle) * Vector2.left);
        FireBullet(Quaternion.Euler(0, 0, -spreadAngle) * Vector2.left);
    }

    void FireBullet(Vector2 direction)
    {
        GameObject projectileObj = Instantiate(
            projectilePrefab,
            transform.position,
            Quaternion.identity
        );

        Projectile projectile = projectileObj.GetComponent<Projectile>();
        if (projectile != null)
            projectile.Launch(direction, bulletSpeed, bulletDamage);
    }
}