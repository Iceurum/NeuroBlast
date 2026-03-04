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
       
        //if (other.CompareTag("Enemy"))
        //{
        //    other.GetComponent<Enemy>().TakeDamage(damage);
        //}

        Destroy(gameObject);
    }
    void OnCollisionEnter2D(Collision2D collision)
        {
            Destroy(gameObject);
        }
}
