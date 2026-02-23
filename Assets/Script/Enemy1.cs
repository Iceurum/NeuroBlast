using UnityEngine;

public class Enemy1 : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //health
    public int maxHealth = 100;
    private int currentHealth;
    //movemet
    public float moveSpeed = 3f;
    public float amplitude = 1.5f;
    public float frequency = 2f; 
    private Vector2 moveDirection = Vector2.left;
    private float startY;
    
    void Start()
    {
        currentHealth = maxHealth;
        startY = transform.position.y;
    }

    void Update()
    {
        // Hitung posisi baru
        float newX = transform.position.x - moveSpeed * Time.deltaTime;
        float newY = startY + Mathf.Sin(Time.time * frequency) * amplitude;

        transform.position = new Vector3(newX, newY, transform.position.z);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    //void OnTriggerEnter2D(Collider2D other)
		//{
			// player = other.gameObject.GetComponent<>();

			//if (player != null)
			//{
				//player.ChangeHealth(-10);
			//}
	//	}
}
