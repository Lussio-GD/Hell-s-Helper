using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance;

    public string bootScene = "BootScene";
    public string mainMenuScene = "MainMenu";
    public string mainGameScene = "MainGame";
    public string creditsScene = "CreditsScene";

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
        if (SceneManager.GetActiveScene().name == bootScene)
        {
            LoadMainMenu();
        }
    }

    public void LoadBootScene()
    {
        SceneManager.LoadScene(bootScene);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
    }

    public void LoadMainGame()
    {
        SceneManager.LoadScene(mainGameScene);
    }

    public void LoadCreditsScene()
    {
        SceneManager.LoadScene(creditsScene);
    }

    public void ReloadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}