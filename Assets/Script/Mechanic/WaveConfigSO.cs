using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class EnemySpawnEntry
{
    public GameObject enemyPrefab;
    public int count = 3;
    public float delayBetweenSpawns = 0.5f;
    public SpawnOrigin spawnOrigin = SpawnOrigin.RightEdge;
    public Transform spawnPoint;               
}

[System.Serializable]
public class WaveData
{
    public string waveName = "Wave 1";
    public List<EnemySpawnEntry> enemies = new List<EnemySpawnEntry>();
    public float delayBeforeWave = 2f;
    public float delayAfterWave = 3f;
}

public enum SpawnOrigin
{
    RightEdge,
    FixedPoint
}

[CreateAssetMenu(fileName = "WaveConfig_Level1", menuName = "Game/Wave Config")]
public class WaveConfigSO : ScriptableObject
{
    [Header("Level Info")]
    public string levelName = "Level 1";

    [Header("Waves")]
    public List<WaveData> waves = new List<WaveData>();
}
