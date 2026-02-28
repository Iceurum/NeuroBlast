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

    [Header("Stage Timing")]
    public float introDuration = 5f;
    public float stageDuration = 100f;
    public float outroDuration = 5f;

    private float timer;
    private GameState currentState;

    [Header("Global Counter")]
    public int totalEnemyDestroyed;
    public int totalEnemyEscaped;

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

    void Start()
    {
        currentState = GameState.MainMenu;
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
                if (timer <= 0) StartPlaying();
                break;

            case GameState.Playing:
                if (timer <= 0) StartOutro();
                break;

            case GameState.Outro:
                if (timer <= 0) FinishStage();
                break;
        }
    }

    // ===== FLOW CONTROL =====

    public void StartGame()
    {
        totalEnemyDestroyed = 0;
        totalEnemyEscaped = 0;

        SceneManager.LoadScene("Level1");
        StartIntro();
    }

    void StartIntro()
    {
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
        Debug.Log("Stage Finished");
    }

    // ===== COUNTER =====

    public void AddEnemyDestroyed()
    {
        totalEnemyDestroyed++;
    }

    public void AddEnemyEscaped()
    {
        totalEnemyEscaped++;
    }

    public GameState CurrentState => currentState;
}