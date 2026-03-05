using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Wave Configs (index = level - 1)")]
    public WaveConfigSO[] levelConfigs;         // slot 0 = Level1, slot 1 = Level2, dst

    [Header("Spawn Area (Right Edge)")]
    public float spawnX = 12f;
    public float spawnMinY = -4f;
    public float spawnMaxY = 4f;

    [Header("State (Read Only)")]
    public int currentWaveIndex = 0;
    public bool isSpawning = false;

    private WaveConfigSO activeConfig;

    // ===================== LIFECYCLE =====================

    void Start()
    {
        // Ambil level saat ini dari GameManager
        int levelIndex = GameManager.Instance != null
            ? GameManager.Instance.currentLevelIndex - 1  // convert 1-based ke 0-based
            : 0;

        if (levelConfigs == null || levelConfigs.Length == 0)
        {
            Debug.LogError("EnemySpawner: levelConfigs kosong! Assign WaveConfig di Inspector.");
            return;
        }

        // Clamp supaya tidak out of range
        levelIndex = Mathf.Clamp(levelIndex, 0, levelConfigs.Length - 1);
        activeConfig = levelConfigs[levelIndex];

        if (activeConfig == null)
        {
            Debug.LogError($"EnemySpawner: WaveConfig untuk level {levelIndex + 1} belum di-assign!");
            return;
        }

        Debug.Log($"EnemySpawner: Memuat {activeConfig.levelName}");
        StartCoroutine(RunWaves());
    }

    // ===================== WAVE RUNNER =====================

    IEnumerator RunWaves()
    {
        for (int i = 0; i < activeConfig.waves.Count; i++)
        {
            currentWaveIndex = i;
            WaveData wave = activeConfig.waves[i];

            Debug.Log($"[{activeConfig.levelName}] {wave.waveName} dimulai dalam {wave.delayBeforeWave}s");

            yield return new WaitForSeconds(wave.delayBeforeWave);

            isSpawning = true;
            foreach (EnemySpawnEntry entry in wave.enemies)
                yield return StartCoroutine(SpawnGroup(entry));
            isSpawning = false;

            Debug.Log($"[{activeConfig.levelName}] {wave.waveName} selesai. Jeda {wave.delayAfterWave}s");

            yield return new WaitForSeconds(wave.delayAfterWave);
        }

        Debug.Log($"[{activeConfig.levelName}] Semua wave selesai!");
    }

    // ===================== SPAWN GROUP =====================

    IEnumerator SpawnGroup(EnemySpawnEntry entry)
    {
        if (entry.enemyPrefab == null)
        {
            Debug.LogWarning("EnemySpawner: enemyPrefab kosong di salah satu entry!");
            yield break;
        }

        for (int i = 0; i < entry.count; i++)
        {
            Vector2 spawnPos = GetSpawnPosition(entry);
            Instantiate(entry.enemyPrefab, spawnPos, Quaternion.identity);
            yield return new WaitForSeconds(entry.delayBetweenSpawns);
        }
    }

    // ===================== SPAWN POSITION =====================

    Vector2 GetSpawnPosition(EnemySpawnEntry entry)
    {
        if (entry.spawnOrigin == SpawnOrigin.FixedPoint && entry.spawnPoint != null)
            return entry.spawnPoint.position;

        float randomY = Random.Range(spawnMinY, spawnMaxY);
        return new Vector2(spawnX, randomY);
    }

    // ===================== PUBLIC =====================

    public void StopSpawner()
    {
        StopAllCoroutines();
        isSpawning = false;
    }

    public void RestartSpawner()
    {
        StopAllCoroutines();
        currentWaveIndex = 0;
        isSpawning = false;
        StartCoroutine(RunWaves());
    }
}
