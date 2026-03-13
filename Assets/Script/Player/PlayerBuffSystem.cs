using UnityEngine;
using System.Collections;

public class PlayerBuffSystem : MonoBehaviour
{
    [Header("Buff Duration")]
    public float buffDuration = 5f;

    [Header("Spread Shot")]
    public GameObject spreadProjectilePrefab;
    public int spreadCount = 3;
    public float spreadAngle = 20f;
    public float spreadSpeed = 15f;

    [Header("Laser Shot")]
    public GameObject laserBeamPrefab;          // prefab dengan LaserBeam.cs + Collider + SpriteRenderer
    public Vector2 laserOffset = new Vector2(3f, 0f);

    [Header("Shield")]
    public GameObject shieldPrefab;             // prefab shield yang jadi child player

    // State
    public BuffType currentBuff = BuffType.None;
    private Coroutine buffCoroutine;

    // Active objects
    private GameObject activeLaser;
    private GameObject activeShield;

    private PlayerController player;

    // ===================== LIFECYCLE =====================

    void Start()
    {
        player = GetComponent<PlayerController>();
    }

    // ===================== ACTIVATE BUFF =====================

    public void ActivateBuff(BuffType buff)
    {
        // Replace buff lama
        if (buffCoroutine != null)
            StopCoroutine(buffCoroutine);

        ClearBuff();

        currentBuff = buff;
        Debug.Log("Buff aktif: " + buff);

        buffCoroutine = StartCoroutine(BuffTimer(buff));
    }

    IEnumerator BuffTimer(BuffType buff)
    {
        // Aktifkan buff
        switch (buff)
        {
            case BuffType.LaserShot:
                ActivateLaser();
                break;

            case BuffType.Shield:
                ActivateShield();
                break;
        }

        yield return new WaitForSeconds(buffDuration);
        ClearBuff();
    }

    // ===================== SHOOT WITH BUFF =====================

    // Dipanggil dari PlayerController.HandleShoot()
    public void ShootWithBuff(Vector2 origin, int baseDamage)
    {
        switch (currentBuff)
        {
            case BuffType.SpreadShot:
                FireSpread(origin, baseDamage);
                break;

            case BuffType.LaserShot:
                // Laser sudah aktif sebagai GameObject — tidak perlu tembak manual
                break;

            default:
                FireSingle(origin, Vector2.right, baseDamage);
                break;
        }
    }

    // ===================== FIRE MODES =====================

    void FireSingle(Vector2 origin, Vector2 direction, int damage)
    {
        if (player == null || player.projectilePrefab == null) return;

        GameObject obj = Instantiate(player.projectilePrefab, origin, Quaternion.identity);
        obj.GetComponent<Projectile>()?.Launch(direction, 15f, damage);
    }

    void FireSpread(Vector2 origin, int damage)
    {
        if (player == null || player.projectilePrefab == null) return;

        float angleStep = spreadCount > 1 ? spreadAngle / (spreadCount - 1) : 0f;
        float startAngle = -spreadAngle / 2f;

        for (int i = 0; i < spreadCount; i++)
        {
            float angle = startAngle + angleStep * i;
            Vector2 dir = RotateVector(Vector2.right, angle);

            GameObject obj = Instantiate(player.projectilePrefab, origin, Quaternion.identity);
            obj.GetComponent<Projectile>()?.Launch(dir, spreadSpeed, damage);
        }
    }

    // ===================== LASER =====================

    void ActivateLaser()
    {
        if (laserBeamPrefab == null)
        {
            Debug.LogWarning("laserBeamPrefab belum di-assign!");
            return;
        }

        // Spawn laser sebagai child player supaya ikut gerak
        activeLaser = Instantiate(laserBeamPrefab, transform);
        activeLaser.transform.localPosition = laserOffset;
    }

    // ===================== SHIELD =====================

    void ActivateShield()
{
    if (shieldPrefab == null)
    {
        Debug.LogWarning("shieldPrefab belum di-assign!");
        return;
    }

    activeShield = Instantiate(shieldPrefab, transform.position, Quaternion.identity);
    activeShield.transform.SetParent(transform);
    activeShield.transform.localPosition = Vector3.zero;
}

    // Shield menyerap damage
    public bool AbsorbDamage()
    {
        if (currentBuff == BuffType.Shield && activeShield != null)
        {
            Debug.Log("Shield menyerap damage!");
            return true;
        }
        return false;
    }

    // ===================== CLEAR BUFF =====================

    void ClearBuff()
    {
        currentBuff = BuffType.None;

        if (activeLaser != null)
        {
            Destroy(activeLaser);
            activeLaser = null;
        }

        if (activeShield != null)
        {
            Destroy(activeShield);
            activeShield = null;
        }
    }

    // ===================== HELPER =====================

    Vector2 RotateVector(Vector2 v, float degrees)
    {
        float radians = degrees * Mathf.Deg2Rad;
        float sin = Mathf.Sin(radians);
        float cos = Mathf.Cos(radians);
        return new Vector2(cos * v.x - sin * v.y, sin * v.x + cos * v.y);
    }

    public bool HasBuff() => currentBuff != BuffType.None;
    public BuffType GetCurrentBuff() => currentBuff;
}
