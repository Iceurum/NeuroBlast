using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("BGM")]
    public AudioClip menuBGM;
    public AudioClip gameBGM;

    [Header("SFX")]
    public AudioClip buttonSFX;
    public AudioClip gameOverSFX; 
    public AudioClip bulletSFX;

    [Header("Settings")]
    [Range(0f, 1f)]
    public float bgmVolume = 0.5f;
    [Range(0f, 1f)]
    public float sfxVolume = 1f;

    private AudioSource bgmSource;
    private AudioSource sfxSource;
    private AudioClip currentClip;

    // ===================== LIFECYCLE =====================

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // BGM source
            bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.loop = true;
            bgmSource.volume = bgmVolume;

            // SFX source
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.volume = sfxVolume;
            
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        PlayBGMForScene(SceneManager.GetActiveScene().name);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // ===================== SCENE LOADED =====================

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayBGMForScene(scene.name);
    }

    void PlayBGMForScene(string sceneName)
    {
        switch (sceneName)
        {
            case "MainMenu":
            case "OpeningCutscene":
                PlayBGM(menuBGM);
                break;

            case "Level1":
            case "Level2":
            case "LevelBoss":
                PlayBGM(gameBGM);
                break;

            case "Ending_GameOver":
                StopBGM();
                PlaySFX(gameOverSFX);
                break;
            case "Ending_PerfectClear":
            case "Ending_PartialSuccess":
                StopBGM();
                break;
        }
    }

    // ===================== BGM =====================

    public void PlayBGM(AudioClip clip)
    {
        if (clip == null) return;
        if (currentClip == clip) return;

        currentClip = clip;
        bgmSource.clip = clip;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        currentClip = null;
        bgmSource.Stop();
    }

    // ===================== SFX =====================

    public void PlayButtonSFX()
    {
        if (buttonSFX == null) return;
        sfxSource.PlayOneShot(buttonSFX, sfxVolume);
    }
    public void PlayBulletSFX()
    {
        PlaySFX(bulletSFX);
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip, sfxVolume);
    }

    // ===================== VOLUME =====================

    public void SetBGMVolume(float val)
    {
        bgmVolume = Mathf.Clamp01(val);
        bgmSource.volume = bgmVolume;
    }

    public void SetSFXVolume(float val)
    {
        sfxVolume = Mathf.Clamp01(val);
        sfxSource.volume = sfxVolume;
    }
}