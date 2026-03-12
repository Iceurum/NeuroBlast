using UnityEngine;
using System.Collections;

public class TumorBuffA : EnemyBase
{
    [Header("Tumor Buff A - Bullet Drop")]
    public GameObject projectilePrefab;
    public int bulletDamage = 10;
    public float shootCooldown = 3f;
    public float bulletSpeed = 6f;

    [Header("Drop")]
    public GameObject buffPickupPrefab;     // assign BuffPickup prefab di Inspector

    private bool canShoot = false;
    private float shootTimer = 0f;

    // ===================== LIFECYCLE =====================

    void Start()
    {
        StartCoroutine(ShootDelay());
    }

    IEnumerator ShootDelay()
    {
        yield return new WaitForSeconds(2f);    // delay awal sebelum bisa tembak
        canShoot = true;
    }

    // ===================== MOVEMENT =====================

    protected override void HandleMovement()
    {
        // Bergerak lambat ke kiri
        transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
    }

    // ===================== SHOOT =====================

    void Update()
    {
        CheckOutOfBounds();

        if (!canShoot) return;

        shootTimer += Time.deltaTime;
        if (shootTimer >= shootCooldown)
        {
            shootTimer = 0f;
            Shoot();
        }
    }

    void Shoot()
    {
        if (projectilePrefab == null) return;

        GameObject obj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Projectile projectile = obj.GetComponent<Projectile>();
        if (projectile != null)
            projectile.Launch(Vector2.left, bulletSpeed, bulletDamage);
    }

    // ===================== DEATH OVERRIDE =====================

    protected override void Die()
    {
        // Drop buff pickup
        if (buffPickupPrefab != null)
            Instantiate(buffPickupPrefab, transform.position, Quaternion.identity);

        base.Die();
    }
}
