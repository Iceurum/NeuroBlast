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

    public void Launch(Vector2 dir, float projectileSpeed, int bulletDamage, Collider2D playerCollider)
{
    direction = dir.normalized;
    speed = projectileSpeed;
    damage = bulletDamage;

    rb.linearVelocity = direction * speed;

    // Ignore collision tanpa perlu FindObjectOfType
    Collider2D bulletCollider = GetComponent<Collider2D>();
    if (playerCollider != null && bulletCollider != null)
        Physics2D.IgnoreCollision(bulletCollider, playerCollider);

    Destroy(gameObject, lifeTime);
}

    void OnTriggerEnter2D(Collider2D other)
    {
        // Jangan destroy jika mengenai player
        if (other.GetComponent<PlayerController>() != null)
            return;

        if (other.CompareTag("Enemy"))
        {
            // other.GetComponent<Enemy>().TakeDamage(damage);
        }

        Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Jangan destroy jika mengenai player
        if (collision.gameObject.GetComponent<PlayerController>() != null)
            return;

        Destroy(gameObject);
    }
}
