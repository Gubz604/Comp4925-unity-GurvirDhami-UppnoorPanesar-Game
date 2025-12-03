using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [Header("Music Clips")]
    [SerializeField] private AudioClip menuMusic; 
    [SerializeField] private AudioClip gameMusic; 

    [Header("Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float volume = 0.5f;

    private AudioSource _audioSource;

    private void Awake()
    {
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
        if (scene.name == "Home_Screen" || scene.name == "Login_Scene")
        {
            PlayMenuMusic(preserveIfAlreadyPlaying: true);
        }
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
            return;
        }

        _audioSource.clip = menuMusic;
        _audioSource.time = 0f;
        _audioSource.Play();
    }

    private void PlayGameMusic()
    {
        if (gameMusic == null) return;

        _audioSource.clip = gameMusic;
        _audioSource.time = 0f;
        _audioSource.Play();
    }
}
