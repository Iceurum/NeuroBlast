using UnityEngine;

public class Enemy2 : MonoBehaviour
{
    public Transform player;

    //health
    public int maxHealth = 80;
    private int currentHealth;

    //movement
    public float moveSpeed = 6f;
    public float hoverDuration = 1.5f;
    public float verticalAdjustSpeed = 5f;

    //shake
    public float shakeDuration = 0.4f;
    public float shakeMagnitude = 0.08f;

    private float hoverTimer;
    private float shakeTimer;

    private bool isShaking = false;
    private bool isCharging = false;

    private float lockedY;
    private Vector2 originalPosition;

    void Start()
    {
        currentHealth = maxHealth;
        hoverTimer = hoverDuration;
    }

    void Update()
    {
        if (!isCharging)
        {
            HandlePreCharge();
        }
        else
        {
            ChargePhase();
        }
    }

    void HandlePreCharge()
    {
        if (hoverTimer > 0f && !isShaking)
        {
            hoverTimer -= Time.deltaTime;
            return;
        }

        if (!isShaking)
        {
            isShaking = true;
            shakeTimer = shakeDuration;
            originalPosition = transform.position;
        }

        if (shakeTimer > 0f)
        {
            shakeTimer -= Time.deltaTime;

            float offsetX = Random.Range(-1f, 1f) * shakeMagnitude;
            float offsetY = Random.Range(-1f, 1f) * shakeMagnitude;

            transform.position = originalPosition + new Vector2(offsetX, offsetY);
            return;
        }

        transform.position = originalPosition;
        LockPlayerPosition();
        isCharging = true;
    }

    void LockPlayerPosition()
    {
        if (player != null)
            lockedY = player.position.y;
        else
            lockedY = transform.position.y;
    }

    void ChargePhase()
    {
        float newX = transform.position.x - moveSpeed * Time.deltaTime;

        float newY = Mathf.MoveTowards(
            transform.position.y,
            lockedY,
            verticalAdjustSpeed * Time.deltaTime
        );

        transform.position = new Vector2(newX, newY);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
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