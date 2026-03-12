using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LaserBeam : MonoBehaviour
{
    [Header("Laser Settings")]
    public int damagePerTick = 20;
    public float tickInterval = 1f;         // delay damage per detik
    public float duration = 5f;             // durasi laser aktif

    private List<EnemyBase> enemiesInLaser = new List<EnemyBase>();
    private List<BossController> bossInLaser = new List<BossController>();

    // ===================== LIFECYCLE =====================

    void Start()
    {
        StartCoroutine(LaserTick());
        Destroy(gameObject, duration);
    }

    IEnumerator LaserTick()
    {
        while (true)
        {
            yield return new WaitForSeconds(tickInterval);

            // Damage semua enemy dalam laser
            for (int i = enemiesInLaser.Count - 1; i >= 0; i--)
            {
                if (enemiesInLaser[i] != null)
                    enemiesInLaser[i].TakeDamage(damagePerTick);
                else
                    enemiesInLaser.RemoveAt(i);
            }

            // Damage boss kalau ada
            for (int i = bossInLaser.Count - 1; i >= 0; i--)
            {
                if (bossInLaser[i] != null)
                    bossInLaser[i].TakeDamage(damagePerTick);
                else
                    bossInLaser.RemoveAt(i);
            }
        }
    }

    // ===================== COLLISION =====================

    void OnTriggerEnter2D(Collider2D other)
    {
        EnemyBase enemy = other.GetComponent<EnemyBase>();
        if (enemy != null && !enemiesInLaser.Contains(enemy))
            enemiesInLaser.Add(enemy);

        BossController boss = other.GetComponent<BossController>();
        if (boss != null && !bossInLaser.Contains(boss))
            bossInLaser.Add(boss);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        EnemyBase enemy = other.GetComponent<EnemyBase>();
        if (enemy != null)
            enemiesInLaser.Remove(enemy);

        BossController boss = other.GetComponent<BossController>();
        if (boss != null)
            bossInLaser.Remove(boss);
    }
}
