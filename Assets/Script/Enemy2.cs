using UnityEngine;

public class Enemy2 : EnemyBase
{
    public Transform player;

    public float moveSpeed = 6f;
    public float hoverDuration = 1.5f;
    public float verticalAdjustSpeed = 5f;

    public float shakeDuration = 0.4f;
    public float shakeMagnitude = 0.08f;

    private float hoverTimer;
    private float shakeTimer;

    private bool isShaking = false;
    private bool isCharging = false;

    private float lockedY;
    private Vector2 originalPosition;

    protected override void Start()
    {
        base.Start();
        hoverTimer = hoverDuration;

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (!isCharging)
            HandlePreCharge();
        else
            ChargePhase();
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
        lockedY = player != null ? player.position.y : transform.position.y;
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
}