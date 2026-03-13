using UnityEngine;
using System.Collections;

public class BossController : MonoBehaviour
{
    [Header("Stats")]
    public int maxHP = 2000;
    public int contactDamage = 30;
    private int currentHP;
    private bool isDead = false;

    [Header("Movement")]
    public float moveSpeed = 2f;
    public float floatAmplitude = 1f;       // seberapa jauh naik turun
    public float floatFrequency = 1f;       // seberapa cepat naik turun
    public float introSpeed = 5f;           // kecepatan masuk dari kanan
    public Vector2 battlePosition = new Vector2(5f, 0f); // posisi boss saat attack

    [Header("Spread Shot")]
    public GameObject projectilePrefab;
    public int spreadCount = 5;
    public float spreadAngle = 30f;
    public int spreadDamage = 20;
    public float spreadSpeed = 8f;
    public float spreadCooldown = 3f;

    [Header("Homing Attack")]
    public GameObject homingProjectilePrefab;
    public int homingCount = 7;
    public int homingDamage = 10;
    public float homingSpeed = 3f;
    public float homingTurnSpeed = 2f;
    public float homingCooldown = 5f;
    public float homingDelayBetween = 1f; 

    [Header("Charge Attack")]
    public float chargeSpeed = 20f;
    public int chargeDamage = 30;
    public float chargeCooldown = 8f;
    public float chargeWindup = 1f;        

    [Header("Death")]
    public GameObject explosionEffectPrefab;
    public float explosionDuration = 2f;
    public float whiteFlashDuration = 1f;

    private enum BossState { Inactive, Intro, Battle, Attacking, Dead }
    private BossState currentState = BossState.Inactive;

    private int currentAttackIndex = 0;     // 0 = Spread, 1 = Homing, 2 = Charge
    private float startY;
    private PlayerController player;
    private Rigidbody2D rb;
    private bool isStarted = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.gravityScale = 0f;
    }

    void Start()
    {
        currentHP = maxHP;
        startY = battlePosition.y;
    }

    void Update()
    {
        if (!isStarted) return;

        if (currentState == BossState.Battle)
            HandleFloating();
    }

    public void StartBossFight()
    {
        isStarted = true;
        player = FindAnyObjectByType<PlayerController>();

        // Spawn dari kanan layar
        transform.position = new Vector2(20f, battlePosition.y);
        currentState = BossState.Intro;

        StartCoroutine(BossIntro());
    }

    IEnumerator BossIntro()
    {
        // Bergerak ke battle position
        while (Vector2.Distance(transform.position, battlePosition) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                battlePosition,
                introSpeed * Time.deltaTime
            );
            yield return null;
        }

        transform.position = battlePosition;
        startY = transform.position.y;
        currentState = BossState.Battle;

        // Mulai attack loop
        StartCoroutine(AttackLoop());
    }

    void HandleFloating()
    {
        float newY = startY + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = new Vector2(transform.position.x, newY);
    }

    IEnumerator AttackLoop()
    {
        while (!isDead)
        {
            yield return new WaitForSeconds(1.5f);  // jeda antar attack

            if (isDead) break;

            currentState = BossState.Attacking;

            switch (currentAttackIndex)
            {
                case 0:
                    yield return StartCoroutine(SpreadShot());
                    break;
                case 1:
                    yield return StartCoroutine(HomingAttack());
                    break;
                case 2:
                    yield return StartCoroutine(ChargeAttack());
                    break;
            }

            // Lanjut ke attack berikutnya
            currentAttackIndex = (currentAttackIndex + 1) % 3;
            currentState = BossState.Battle;
        }
    }

    IEnumerator SpreadShot()
    {
        // Berhenti sebentar (telegraph)
        yield return new WaitForSeconds(0.5f);

        if (isDead) yield break;

        // Hitung arah ke player
        Vector2 dirToPlayer = Vector2.left;
        if (player != null)
            dirToPlayer = (player.transform.position - transform.position).normalized;

        // Tembak spread
        float angleStep = spreadAngle / (spreadCount - 1);
        float startAngle = -spreadAngle / 2f;

        for (int i = 0; i < spreadCount; i++)
        {
            float angle = startAngle + angleStep * i;
            Vector2 dir = RotateVector(dirToPlayer, angle);
            FireProjectile(projectilePrefab, dir, spreadSpeed, spreadDamage);
        }

        yield return new WaitForSeconds(spreadCooldown);
    }

    IEnumerator HomingAttack()
    {
        yield return new WaitForSeconds(homingDelayBetween);

        if (isDead) yield break;

        // Tembak homing projectile satu per satu dari posisi boss
        for (int i = 0; i < homingCount; i++)
        {
            if (isDead) yield break;

            FireHomingProjectile(transform.position);
            yield return new WaitForSeconds(0.3f);
        }

        yield return new WaitForSeconds(homingCooldown);
    }

    IEnumerator ChargeAttack()
    {
        yield return new WaitForSeconds(chargeWindup);

        if (isDead) yield break;

        // Charge ke kiri selama 0.5 detik
        float chargeTime = 0.5f;
        float elapsed = 0f;

        while (elapsed < chargeTime)
        {
            transform.position += Vector3.left * chargeSpeed * Time.deltaTime;
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Kembali ke battle position
        while (Vector2.Distance(transform.position, battlePosition) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                battlePosition,
                introSpeed * Time.deltaTime
            );
            yield return null;
        }

        transform.position = battlePosition;
        yield return new WaitForSeconds(chargeCooldown);
    }

    void FireProjectile(GameObject prefab, Vector2 direction, float speed, int damage)
    {
        if (prefab == null) return;

        GameObject obj = Instantiate(prefab, transform.position, Quaternion.identity);
        Projectile projectile = obj.GetComponent<Projectile>();
        if (projectile != null)
            projectile.Launch(direction, speed, damage);
    }

    void FireHomingProjectile(Vector2 spawnPos)
    {
        if (homingProjectilePrefab == null)
        {
            // Fallback pakai projectile biasa kalau homing prefab tidak ada
            FireProjectile(projectilePrefab, Vector2.down, homingSpeed, homingDamage);
            return;
        }

        GameObject obj = Instantiate(homingProjectilePrefab, spawnPos, Quaternion.identity);
        HomingProjectile homing = obj.GetComponent<HomingProjectile>();
        if (homing != null)
            homing.Launch(homingSpeed, homingDamage, homingTurnSpeed);
    }

    Vector2 RotateVector(Vector2 v, float degrees)
    {
        float radians = degrees * Mathf.Deg2Rad;
        float sin = Mathf.Sin(radians);
        float cos = Mathf.Cos(radians);
        return new Vector2(cos * v.x - sin * v.y, sin * v.x + cos * v.y);
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHP -= amount;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);

        Debug.Log($"Boss HP: {currentHP}/{maxHP}");

        if (currentHP <= 0)
            StartCoroutine(BossDeath());
    }

    IEnumerator BossDeath()
    {
        isDead = true;
        currentState = BossState.Dead;

        StopAllCoroutines();
        StartCoroutine(BossDeathSequence());
        yield break;
    }

    IEnumerator BossDeathSequence()
    {
        // 1. Boss berhenti bergerak
        if (rb != null) rb.linearVelocity = Vector2.zero;

        // 2. Spawn efek ledakan
        if (explosionEffectPrefab != null)
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);

        yield return new WaitForSeconds(explosionDuration);

        // 3. White flash
        yield return StartCoroutine(WhiteFlash());

        // 4. Kasih tahu GameManager boss mati
        if (GameManager.Instance != null)
            GameManager.Instance.BossDefeated();

        Destroy(gameObject);
    }

    IEnumerator WhiteFlash()
    {
        GameObject flashObj = new GameObject("WhiteFlash");
        Canvas canvas = FindAnyObjectByType<Canvas>();
        if (canvas != null)
            flashObj.transform.SetParent(canvas.transform, false);

        UnityEngine.UI.Image flashImage = flashObj.AddComponent<UnityEngine.UI.Image>();
        flashImage.color = new Color(1f, 1f, 1f, 0f);
        flashImage.rectTransform.anchorMin = Vector2.zero;
        flashImage.rectTransform.anchorMax = Vector2.one;
        flashImage.rectTransform.offsetMin = Vector2.zero;
        flashImage.rectTransform.offsetMax = Vector2.zero;

        // Fade in putih
        float elapsed = 0f;
        while (elapsed < whiteFlashDuration)
        {
            elapsed += Time.deltaTime;
            flashImage.color = new Color(1f, 1f, 1f, Mathf.Lerp(0f, 1f, elapsed / whiteFlashDuration));
            yield return null;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>()?.TakeDamage(contactDamage);
        }

        if (other.CompareTag("PlayerBullet"))
        {
             Projectile projectile = other.GetComponent<Projectile>();
            if (projectile != null)
                TakeDamage(projectile.GetDamage());
    
            Destroy(other.gameObject); 
        }
    }

    public int GetCurrentHP() => currentHP;
    public int GetMaxHP() => maxHP;
}