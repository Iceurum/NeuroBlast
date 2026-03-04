using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    MainMenu,
    Intro,
    Playing,
    Outro,
    Finished
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Player")]
    public GameObject playerPrefab;
    private PlayerController currentPlayer;

    [Header("Stage Timing")]
    public float introDuration = 5f;
    public float stageDuration = 100f;
    public float outroDuration = 5f;

    private float timer;
    private GameState currentState;

    [Header("Level Settings")]
    public int currentLevelIndex = 1;
    public int maxLevel = 3;

    [Header("Enemy Counter (Global)")]
    public int totalEnemyDestroyed;
    public int totalEnemyEscaped;

    [Header("Enemy Counter (Per Level)")]
    public int levelEnemyDestroyed;
    public int levelEnemyEscaped;

    // ===================== LIFECYCLE =====================

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (currentState == GameState.Intro ||
            currentState == GameState.Playing ||
            currentState == GameState.Outro)
        {
            timer -= Time.deltaTime;
        }

        switch (currentState)
        {
            case GameState.Intro:
                if (timer <= 0f) StartPlaying();
                break;

            case GameState.Playing:
                if (timer <= 0f) StartOutro();
                break;

            case GameState.Outro:
                if (timer <= 0f) FinishStage();
                break;
        }
    }

    // ===================== GAME FLOW =====================

    public void StartGame()
    {
        currentLevelIndex = 1;

        totalEnemyDestroyed = 0;
        totalEnemyEscaped = 0;

        LoadLevel(currentLevelIndex);
    }

    void LoadLevel(int levelIndex)
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene("Level" + levelIndex);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        SpawnPlayer();     // 🔥 Spawn otomatis tiap load level
        StartIntro();
    }

    void StartIntro()
    {
        ResetLevelCounter();
        currentState = GameState.Intro;
        timer = introDuration;
    }

    void StartPlaying()
    {
        currentState = GameState.Playing;
        timer = stageDuration;
    }

    void StartOutro()
    {
        currentState = GameState.Outro;
        timer = outroDuration;
    }

    void FinishStage()
    {
        currentState = GameState.Finished;

        Debug.Log($"Level {currentLevelIndex} finished");
        Debug.Log($"Destroyed: {levelEnemyDestroyed}, Escaped: {levelEnemyEscaped}");

        if (currentLevelIndex < maxLevel)
        {
            currentLevelIndex++;
            LoadLevel(currentLevelIndex);
        }
        else
        {
            Debug.Log("GAME COMPLETED");
        }
    }

    // ===================== PLAYER SPAWN =====================

    void SpawnPlayer()
    {
        Transform spawnPoint = GameObject.FindWithTag("PlayerSpawn")?.transform;

        if (spawnPoint == null)
        {
            Debug.LogError("No PlayerSpawn found in scene!");
            return;
        }

        GameObject playerObj = Instantiate(playerPrefab, spawnPoint.position, Quaternion.identity);
        currentPlayer = playerObj.GetComponent<PlayerController>();
    }

    public void PlayerDied()
    {
        Debug.Log("Game Over");
        currentState = GameState.Finished;

        // Bisa tambahkan load game over scene di sini
    }

    // ===================== COUNTER =====================

    void ResetLevelCounter()
    {
        levelEnemyDestroyed = 0;
        levelEnemyEscaped = 0;
    }

    public void AddEnemyDestroyed()
    {
        totalEnemyDestroyed++;
        levelEnemyDestroyed++;
    }

    public void AddEnemyEscaped()
    {
        totalEnemyEscaped++;
        levelEnemyEscaped++;
    }

    // ===================== ACCESSOR =====================

    public GameState CurrentState => currentState;
}