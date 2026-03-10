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
    public int maxLevel = 5;               

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
                // Stage 5 tidak pakai timer untuk trigger outro
                // Boss fight yang trigger outro via BossDefeated()
                if (currentLevelIndex < maxLevel && timer <= 0f)
                    StartOutro();
                break;
        }
    }

    // ===================== GAME FLOW =====================

    public void StartGame()
    {
        currentLevelIndex = 1;
        lastLevelIndex = 1;
        totalEnemyDestroyed = 0;
        totalEnemyEscaped = 0;
        breachMeter = 0;
        bossDefeated = false;

        // Ke Opening Cutscene dulu
        LoadCutscene("OpeningCutscene");
    }

    void LoadCutscene(string sceneName)
    {
        currentState = GameState.Cutscene;
        SceneManager.LoadScene(sceneName);
    }

    // Dipanggil dari CutsceneManager setelah cutscene selesai
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

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        StartCoroutine(RunLevelIntro());
    }

    IEnumerator RunLevelIntro()
    {
        // Tunggu 2 frame supaya semua GameObject fully initialized
        yield return null;
        yield return null;

        ResetLevelCounter();
        allWavesCompleted = false;
        currentState = GameState.Intro;
        timer = introDuration;

        Debug.Log("RunLevelIntro START - Level " + currentLevelIndex);

        // 1. Tampilkan level title
        LevelIntroUI introUI = FindAnyObjectByType<LevelIntroUI>();
        Debug.Log("LevelIntroUI found: " + (introUI != null));

        if (introUI != null)
            yield return StartCoroutine(introUI.PlayIntro("Level " + currentLevelIndex));

        Debug.Log("Title intro selesai, spawn player...");

        // 2. Spawn player lalu intro animation
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
    }

    void StartOutro()
    {
        currentState = GameState.Outro;

        // Stop spawner
        EnemySpawner spawner = FindAnyObjectByType<EnemySpawner>();
        if (spawner != null)
            spawner.StopSpawner();

        StartCoroutine(RunOutro());
    }

    IEnumerator RunOutro()
    {
        // Tunggu sampai semua enemy mati/lolos
        while (FindAnyObjectByType<EnemyBase>() != null)
            yield return new WaitForSeconds(0.5f);

        // Outro animation player
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
            LoadLevel(currentLevelIndex);
        }
        else
        {
            // Stage 5 selesai → trigger ending cutscene
            TriggerEndingCutscene();
        }
    }

    // ===================== BOSS FIGHT =====================

    // Dipanggil oleh BossController saat boss mati
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
        if (breachMeter == 0)
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

        LoadLevel(currentLevelIndex);
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

    // ===================== WAVE STATUS =====================

    public void OnAllWavesCompleted()
    {
        allWavesCompleted = true;
        Debug.Log($"Level {currentLevelIndex} - Semua wave selesai! Menunggu timer...");

        // Stage 5 — setelah wave selesai langsung trigger boss fight
        if (currentLevelIndex == maxLevel)
        {
            Debug.Log("Stage 5 waves selesai - Boss Fight dimulai!");
            currentState = GameState.BossFight;
            // BossController akan di-activate dari sini atau via event
            //BossController boss = FindAnyObjectByType<BossController>();
            //if (boss != null)
            //    boss.StartBossFight();
        }
    }

    // ===================== PLAYER =====================

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
    public float GetTimer() => timer;
    public int GetBreachMeter() => breachMeter;
    public bool IsBossFight() => currentState == GameState.BossFight;
}