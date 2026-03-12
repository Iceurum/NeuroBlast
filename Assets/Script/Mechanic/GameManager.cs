using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public enum GameState
{
    MainMenu,
    Cutscene,
    Intro,
    Playing,
    Outro,
    BossFight,
    Finished
}

public enum EndingType
{
    None,
    PerfectClear,
    PartialSuccess,
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
    public int lastLevelIndex = 1;
    public int maxLevel = 3;                // ← 3 level: 1-2 biasa, 3 boss

    [Header("Enemy Counter (Global)")]
    public int totalEnemyDestroyed;
    public int totalEnemyEscaped;

    [Header("Enemy Counter (Per Level)")]
    public int levelEnemyDestroyed;
    public int levelEnemyEscaped;

    [Header("Wave Status")]
    public bool allWavesCompleted = false;
    public bool bossDefeated = false;

    [Header("Breach Meter")]
    public int breachMeter = 0;

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
            currentState == GameState.Playing)
        {
            timer -= Time.deltaTime;
        }

        switch (currentState)
        {
            case GameState.Intro:
                if (timer <= 0f) StartPlaying();
                break;

            case GameState.Playing:
                // Level 3 (boss) tidak pakai timer — boss yang trigger outro
                if (currentLevelIndex < maxLevel && timer <= 0f)
                    StartOutro();
                break;
        }
    }

    public void StartGame()
    {
        currentLevelIndex = 1;
        lastLevelIndex = 1;
        totalEnemyDestroyed = 0;
        totalEnemyEscaped = 0;
        breachMeter = 0;
        bossDefeated = false;

        LoadCutscene("OpeningCutscene");
    }

    void LoadCutscene(string sceneName)
    {
        currentState = GameState.Cutscene;
        SceneManager.LoadScene(sceneName);
    }

    public void OnCutsceneFinished(string cutsceneName)
    {
        switch (cutsceneName)
        {
            case "OpeningCutscene":
                LoadLevel(currentLevelIndex);
                break;

            case "Ending_PerfectClear":
            case "Ending_PartialSuccess":
                SceneManager.LoadScene("MainMenu");
                break;
        }
    }

    void LoadLevel(int levelIndex)
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene("Level" + levelIndex);
    }

    void LoadBossLevel()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene("LevelBoss");
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        StartCoroutine(RunLevelIntro());
    }

    IEnumerator RunLevelIntro()
    {
        yield return null;
        yield return null;

        ResetLevelCounter();
        allWavesCompleted = false;
        currentState = GameState.Intro;
        timer = introDuration;

        Debug.Log("RunLevelIntro START - Level " + currentLevelIndex);

        // Tampilkan level title
        LevelIntroUI introUI = FindAnyObjectByType<LevelIntroUI>();
        Debug.Log("LevelIntroUI found: " + (introUI != null));

        if (introUI != null)
        {
            string titleText = currentLevelIndex == maxLevel
                ? "Boss Stage"                          // Level 3 tampilkan "Boss Stage"
                : "Level " + currentLevelIndex;
            yield return StartCoroutine(introUI.PlayIntro(titleText));
        }

        Debug.Log("Title intro selesai, spawn player...");

        SpawnPlayer();

        if (currentPlayer != null)
        {
            Transform spawnPoint = GameObject.FindWithTag("PlayerSpawn")?.transform;
            Debug.Log("SpawnPoint found: " + (spawnPoint != null));

            if (spawnPoint != null)
                yield return StartCoroutine(currentPlayer.PlayIntroAnimation(spawnPoint.position));
        }
        else
        {
            Debug.LogError("currentPlayer NULL setelah SpawnPlayer!");
        }

        Debug.Log("Player intro selesai, mulai gameplay!");
        StartPlaying();
    }

    void StartPlaying()
    {
        currentState = GameState.Playing;
        timer = stageDuration;

        if (currentLevelIndex == maxLevel)
        {
            Debug.Log("Level 3 - Boss Fight dimulai!");
            StartCoroutine(StartBossFightDelay());
        }
    }

    IEnumerator StartBossFightDelay()
    {
        yield return new WaitForSeconds(1f);  
        currentState = GameState.BossFight;

        BossController boss = FindAnyObjectByType<BossController>();
        if (boss != null)
            boss.StartBossFight();
        else
            Debug.LogWarning("BossController tidak ditemukan di scene Level3!");
    }

    void StartOutro()
    {
        currentState = GameState.Outro;

        EnemySpawner spawner = FindAnyObjectByType<EnemySpawner>();
        if (spawner != null)
            spawner.StopSpawner();

        StartCoroutine(RunOutro());
    }

    IEnumerator RunOutro()
    {
        while (FindAnyObjectByType<EnemyBase>() != null)
            yield return new WaitForSeconds(0.5f);

        if (currentPlayer != null)
            yield return StartCoroutine(currentPlayer.PlayOutroAnimation());
        else
            OnOutroAnimationComplete();
    }

    public void OnOutroAnimationComplete()
    {
        FinishStage();
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
            lastLevelIndex = currentLevelIndex;

            // Setelah Level 2 → load LevelBoss langsung
            if (currentLevelIndex == maxLevel)
                LoadBossLevel();
            else
                LoadLevel(currentLevelIndex);
        }
        else
        {
            TriggerEndingCutscene();
        }
    }

    // ===================== BOSS FIGHT =====================

    public void BossDefeated()
    {
        bossDefeated = true;
        Debug.Log("Boss defeated!");
        StartOutro();
    }

    // ===================== ENDING =====================

    void TriggerEndingCutscene()
    {
        EndingType ending = GetEndingType();
        Debug.Log($"GAME COMPLETED - Ending: {ending} | Breach Meter: {breachMeter}");

        switch (ending)
        {
            case EndingType.PerfectClear:
                LoadCutscene("Ending_PerfectClear");
                break;

            case EndingType.PartialSuccess:
                LoadCutscene("Ending_PartialSuccess");
                break;
        }
    }

    public EndingType GetEndingType()
    {
        if (breachMeter < 10)
            return EndingType.PerfectClear;
        else
            return EndingType.PartialSuccess;
    }

    // ===================== GAME OVER =====================

    public void PlayerDied()
    {
        Debug.Log($"Game Over - Player Died at Level {currentLevelIndex}");
        lastLevelIndex = currentLevelIndex;
        currentState = GameState.Finished;
        SceneManager.LoadScene("Ending_GameOver");
    }

    public void RetryFromLevel(int levelIndex)
    {
        currentLevelIndex = levelIndex;
        breachMeter = 0;
        totalEnemyDestroyed = 0;
        totalEnemyEscaped = 0;
        bossDefeated = false;

        if (levelIndex == maxLevel)
            LoadBossLevel();
        else
            LoadLevel(levelIndex);
    }

    public void GoToMainMenu()
    {
        currentLevelIndex = 1;
        lastLevelIndex = 1;
        breachMeter = 0;
        totalEnemyDestroyed = 0;
        totalEnemyEscaped = 0;
        bossDefeated = false;

        SceneManager.LoadScene("MainMenu");
    }

    // ===================== BREACH METER =====================

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

        Vector2 offscreenLeft = new Vector2(spawnPoint.position.x - 15f, spawnPoint.position.y);
        GameObject playerObj = Instantiate(playerPrefab, offscreenLeft, Quaternion.identity);
        currentPlayer = playerObj.GetComponent<PlayerController>();
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
    public bool IsBossFight() => currentState == GameState.BossFight;
}