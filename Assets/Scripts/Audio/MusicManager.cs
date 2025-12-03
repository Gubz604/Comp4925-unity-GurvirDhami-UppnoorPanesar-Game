using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [Header("Music Clips")]
    [SerializeField] private AudioClip menuMusic;  // Home + Login
    [SerializeField] private AudioClip gameMusic;  // Game scene

    [Header("Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float volume = 0.5f;

    private AudioSource _audioSource;

    private void Awake()
    {
        // Singleton that survives scene loads
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.loop = true;
        _audioSource.volume = volume;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Home + Login share the same music, and it should NOT restart
        if (scene.name == "Home_Screen" || scene.name == "Login_Scene")
        {
            PlayMenuMusic(preserveIfAlreadyPlaying: true);
        }
        // Game_Scene music should restart every time the scene loads
        else if (scene.name == "Game_Scene")
        {
            PlayGameMusic();
        }
    }

    private void PlayMenuMusic(bool preserveIfAlreadyPlaying)
    {
        if (menuMusic == null) return;

        if (preserveIfAlreadyPlaying &&
            _audioSource.clip == menuMusic &&
            _audioSource.isPlaying)
        {
            // Already playing menu music → do nothing
            return;
        }

        _audioSource.clip = menuMusic;
        _audioSource.time = 0f;
        _audioSource.Play();
    }

    private void PlayGameMusic()
    {
        if (gameMusic == null) return;

        // Always restart at beginning when game starts / restarts
        _audioSource.clip = gameMusic;
        _audioSource.time = 0f;
        _audioSource.Play();
    }
}
