using UnityEngine;
using System.Collections;

public enum BuffType
{
    None,
    SpreadShot,
    LaserShot,
    Shield
}

public class BuffPickup : MonoBehaviour
{
    [Header("Movement")]
    public float hoverDuration = 0.5f;      // hover di tempat sebelum gerak
    public float moveSpeed = 2f;            // kecepatan gerak ke kiri
    public float lifeTime = 8f;             // hilang setelah 8 detik

    private bool isMoving = false;

    // ===================== LIFECYCLE =====================

    void Start()
    {
        Destroy(gameObject, lifeTime);
        StartCoroutine(HoverThenMove());
    }

    IEnumerator HoverThenMove()
    {
        // Hover di tempat sebentar
        yield return new WaitForSeconds(hoverDuration);
        isMoving = true;
    }

    void Update()
    {
        if (!isMoving) return;
        transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
    }

    // ===================== PICKUP =====================

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerBuffSystem buffSystem = other.GetComponent<PlayerBuffSystem>();
        if (buffSystem == null) return;

        BuffType buff = GetRandomBuff();
        buffSystem.ActivateBuff(buff);
        Debug.Log("Player pickup buff: " + buff);

        Destroy(gameObject);
    }

    // Gacha 33% tiap buff
    BuffType GetRandomBuff()
    {
        int rand = Random.Range(0, 3);
        switch (rand)
        {
            case 0: return BuffType.SpreadShot;
            case 1: return BuffType.LaserShot;
            case 2: return BuffType.Shield;
            default: return BuffType.SpreadShot;
        }
    }
}
