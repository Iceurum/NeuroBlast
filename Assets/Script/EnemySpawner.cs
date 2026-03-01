using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    public GameObject enemy1Prefab;

    [Header("Spawn Area")]
    public float spawnX = 10f;
    public float minY = -4f;
    public float maxY = 4f;

    [Header("Timing")]
    public float stageDuration = 100f;
    public float introDuration = 5f;
    public float outroDuration = 5f;

    [Header("Wave Settings")]
    public float spawnDelay = 0.5f; 

    private float stageTimer;
    private int currentPhase = -1;

    void Start()
    {
        stageTimer = stageDuration;
    }

    void Update()
    {
        if (GameManager.Instance.CurrentState != GameState.Playing)
            return;

        stageTimer -= Time.deltaTime;

        float elapsed = stageDuration - stageTimer;

        if (elapsed < introDuration ||
            elapsed > stageDuration - outroDuration)
            return;

        float gameplayTime = elapsed - introDuration;
        int phase = Mathf.FloorToInt(gameplayTime / 15f);

        if (phase != currentPhase)
        {
            currentPhase = phase;
            StartPhase(currentPhase);
        }
    }

    void StartPhase(int phase)
    {
        switch (phase)
        {
            case 0:
                StartCoroutine(SpawnWave(3, enemy1Prefab));
                break;

            case 3:
                StartCoroutine(SpawnWave(4, enemy1Prefab));
                break;

            case 2:
                StartCoroutine(SpawnWave(5, enemy1Prefab));
                break;

            case 1:
                StartCoroutine(SpawnMiddleLine(5, enemy1Prefab,0.3f));
                break;

            default:
                StartCoroutine(SpawnWave(6, enemy1Prefab));
                break;
        }
    }

    IEnumerator SpawnWave(int count, GameObject prefab)
    {
        for (int i = 0; i < count; i++)
        {
            float randomY = Random.Range(minY, maxY);
            Vector3 spawnPos = new Vector3(spawnX, randomY, 0f);

            Instantiate(prefab, spawnPos, Quaternion.identity);

            yield return new WaitForSeconds(spawnDelay);
        }
    }
    IEnumerator SpawnMiddleLine(int count,GameObject Prefab,float spawnDelay)
    {
        float middleY = 0f;

        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPos = new Vector3(spawnX, middleY, 0f);
            Instantiate(Prefab, spawnPos, Quaternion.identity);

            yield return new WaitForSeconds(spawnDelay);
        }
    }
}