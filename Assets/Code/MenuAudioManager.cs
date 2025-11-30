using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuAudioManager : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioSource musicSource;
    public AudioClip buttonHoverSound;
    public AudioClip buttonClickSound;
    public AudioClip backgroundMusic;

    private static MenuAudioManager instance;
    private bool musicStarted = false;

    void Awake()
    {
        // Singleton pattern with proper scene management
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        InitializeAudioSources();
    }

    void OnDestroy()
    {
        // Clean up event
        if (instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Scene loaded: {scene.name}");

        // Play music only in menu scenes
        if (scene.name == "MainMenu" || scene.name == "CreditsScene")
        {
            PlayBackgroundMusic();
        }
        else
        {
            StopMenuMusic();
        }
    }

    void InitializeAudioSources()
    {
        // Set up music audio source
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
        }

        // Set up SFX audio source
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 0f; // 2D sound
            audioSource.volume = 0.8f;
        }

        // Configure music source
        musicSource.loop = true;
        musicSource.volume = 0.6f;
        musicSource.spatialBlend = 0f; // 2D music
    }

    void PlayBackgroundMusic()
    {
        if (musicSource != null && backgroundMusic != null)
        {
            if (!musicSource.isPlaying)
            {
                musicSource.clip = backgroundMusic;
                musicSource.Play();
                musicStarted = true;
                Debug.Log("Background music started!");
            }
        }
        else
        {
            Debug.LogWarning("Music source or background music clip is null!");
        }
    }

    public void OnButtonHover()
    {
        if (audioSource != null && buttonHoverSound != null)
        {
            audioSource.PlayOneShot(buttonHoverSound);
            Debug.Log("Hover sound played!");
        }
    }

    public void OnButtonClick()
    {
        if (audioSource != null && buttonClickSound != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
            Debug.Log("Click sound played!");
        }
    }

    public void StopMenuMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.Stop();
            Debug.Log("Menu music stopped!");
        }
    }

    public void PlayMenuMusic()
    {
        PlayBackgroundMusic();
    }

    // Static method to easily access the audio manager from any script
    public static MenuAudioManager GetInstance()
    {
        return instance;
    }

    // Public method to check if audio manager is working
    public bool IsAudioWorking()
    {
        return audioSource != null && musicSource != null;
    }
}