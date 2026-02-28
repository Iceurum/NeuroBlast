using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float lifeTime = 3f;
    public int damage = 10;

    private Vector2 direction;
    private float speed;

    public void Launch(Vector2 dir, float spd)
    {
        direction = dir.normalized;
        speed = spd;

        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //PlayerHealth player = other.GetComponent<PlayerHealth>();
//
        //if (player != null)
        //{
        //    player.TakeDamage(damage);
        //    Destroy(gameObject);
        //}
    }
}
