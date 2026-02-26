using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage = 20;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Launch(Vector2 direction, float speed)
    {
        rb.linearVelocity = direction.normalized * speed;
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        // PlayerHealth player = other.GetComponent<PlayerHealth>();
        // if (player != null)
        // {
        //     player.TakeDamage(damage);
        //     Destroy(gameObject);
        // }
    }
}
