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

public enum EndingType
{
    None,
    PerfectClear,       // Breach Meter = 0
    PartialSuccess,     // Breach Meter 1 - breachLimit     
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Player")]
    public GameObject playerPrefab;
    private PlayerController currentPlayer;

    [Header("Stage Timing")]
    public float introDuration = 5f;
    public float stageDuration = 120f;
    public float outroDuration = 5f;

    private float timer;
    private GameState currentState;

    [Header("Level Settings")]
    public int currentLevelIndex = 1;
    public int maxLevel = 4;

    [Header("Enemy Counter (Global)")]
    public int totalEnemyDestroyed;
    public int totalEnemyEscaped;

    [Header("Enemy Counter (Per Level)")]
    public int levelEnemyDestroyed;
    public int levelEnemyEscaped;

    [Header("Wave Status")]
    public bool allWavesCompleted = false;

    [Header("Breach Meter")]
    public int breachMeter = 0;             // nilai breach meter saat ini       

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

    public void StartGame()
    {
        currentLevelIndex = 1;
        totalEnemyDestroyed = 0;
        totalEnemyEscaped = 0;
        breachMeter = 0;

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

        SpawnPlayer();
        StartIntro();
    }

    void StartIntro()
    {
        ResetLevelCounter();
        allWavesCompleted = false;
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
        Debug.Log($"Breach Meter: {breachMeter}");

        if (currentLevelIndex < maxLevel)
        {
            currentLevelIndex++;
            LoadLevel(currentLevelIndex);
        }
        else
        {
            // Semua level selesai — tentukan ending
            TriggerEnding();
        }
    }

    void TriggerEnding()
    {
        EndingType ending = GetEndingType();

        Debug.Log($"GAME COMPLETED - Ending: {ending} | Breach Meter: {breachMeter}");

        switch (ending)
        {
            case EndingType.PerfectClear:
                SceneManager.LoadScene("Ending_PerfectClear");
                break;

            case EndingType.PartialSuccess:
                SceneManager.LoadScene("Ending_PartialSuccess");
                break;

        }
    }

    public EndingType GetEndingType()
    {
        if (breachMeter == 0)
            return EndingType.PerfectClear;
        else
            return EndingType.PartialSuccess;
    }

    public void AddBreachMeter(int value)
    {
        breachMeter += value;
        Debug.Log($"Breach Meter: {breachMeter}");
    }

    public void OnAllWavesCompleted()
    {
        allWavesCompleted = true;
        Debug.Log($"Level {currentLevelIndex} - Semua wave selesai! Menunggu timer...");
    }

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
        Debug.Log("Game Over - Player Died");
        currentState = GameState.Finished;
        SceneManager.LoadScene("Ending_GameOver");
    }

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

    public GameState CurrentState => currentState;
    public float GetTimer() => timer;
    public int GetBreachMeter() => breachMeter;
}