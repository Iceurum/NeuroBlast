using UnityEngine;

public class HomingProjectile : MonoBehaviour
{
    private float speed;
    private int damage;
    private float turnSpeed;
    private PlayerController target;
    private bool launched = false;

    public float lifeTime = 5f;

    void Start()
    {
        target = FindAnyObjectByType<PlayerController>();
        Destroy(gameObject, lifeTime);
    }

    public void Launch(float projectileSpeed, int projectileDamage, float projectileTurnSpeed)
    {
        speed = projectileSpeed;
        damage = projectileDamage;
        turnSpeed = projectileTurnSpeed;
        launched = true;
    }

    void Update()
    {
        if (!launched) return;

        // Kejar player
        if (target != null)
        {
            Vector2 dirToTarget = ((Vector2)target.transform.position - (Vector2)transform.position).normalized;
            Vector2 currentDir = (Vector2)transform.up;

            // Rotate perlahan ke arah player
            Vector2 newDir = Vector2.MoveTowards(currentDir, dirToTarget, turnSpeed * Time.deltaTime);
            transform.up = newDir;
        }

        // Gerak maju
        transform.Translate(Vector2.up * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>()?.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
