using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 15f;
    public float lifeTime = 3f;

    private int damage;
    private Vector2 direction;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Launch(Vector2 dir, float projectileSpeed, int bulletDamage)
    {
        direction = dir.normalized;
        speed = projectileSpeed;
        damage = bulletDamage;

        rb.linearVelocity = direction * speed;

        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter2D(Collider2D other)
{
    Debug.Log($"Bullet tag: {tag} | Kena: {other.gameObject.name} tag: {other.tag}");

    if (CompareTag("PlayerBullet") && other.CompareTag("Enemy"))
    {
        other.GetComponent<EnemyBase>()?.TakeDamage(damage);
        Destroy(gameObject);
        return;
    }

    if (CompareTag("EnemyBullet") && other.CompareTag("Player"))
    {
        other.GetComponent<PlayerController>()?.TakeDamage(damage);
        Destroy(gameObject);
        return;
    }
}

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Bullet player mengenai enemy
        if (CompareTag("PlayerBullet") && collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<EnemyBase>()?.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        // Bullet enemy mengenai player
        if (CompareTag("EnemyBullet") && collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerController>()?.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }
    }

    public int GetDamage() => damage;
}