using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI References")]
    public GameObject winPanel;
    public GameObject losePanel;
    public Slider progressSlider;
    public Text progressText;

    [Header("Game Settings")]
    public string mainMenuScene = "MainMenu";

    private int totalEnemies;
    private int enemiesKilled;
    private bool gameEnded = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        InitializeGame();
    }

    void InitializeGame()
    {
        gameEnded = false;
        FindAllEnemies();
        UpdateUI();

        if (winPanel != null) winPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);

        Time.timeScale = 1f;

        Debug.Log($"GameManager initialized. Total enemies: {totalEnemies}");
    }

    void FindAllEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        totalEnemies = enemies.Length;
        enemiesKilled = 0;

        Debug.Log($"Total enemies found: {totalEnemies}");

        
        foreach (GameObject enemy in enemies)
        {
            Debug.Log($"Enemy found: {enemy.name}");
        }
    }

    public void EnemyKilled()
    {
        if (gameEnded) return;

        enemiesKilled++;
        Debug.Log($"Enemy killed! {enemiesKilled}/{totalEnemies}");

        UpdateUI();

        
        if (totalEnemies > 0 && enemiesKilled >= totalEnemies)
        {
            Debug.Log($"Win condition met! Killed: {enemiesKilled}, Total: {totalEnemies}");
            WinGame();
        }
        else
        {
            Debug.Log($"Progress: {enemiesKilled}/{totalEnemies}, Win not yet");
        }
    }

    public void PlayerDied()
    {
        if (gameEnded) return;
        Debug.Log("GameManager: Player died triggered");
        LoseGame();
    }

    void WinGame()
    {
        if (gameEnded) return;

        gameEnded = true;
        Debug.Log("VICTORY! Showing win panel...");

        if (winPanel != null)
        {
            winPanel.SetActive(true);
            Debug.Log("Win panel activated");
        }
        else
        {
            Debug.LogError("Win Panel is null!");
        }

        Time.timeScale = 0f;
    }

    void LoseGame()
    {
        if (gameEnded) return;

        gameEnded = true;
        Debug.Log("GAME OVER! Showing lose panel...");

        if (losePanel != null)
        {
            losePanel.SetActive(true);
            Debug.Log("Lose panel activated");
        }
        else
        {
            Debug.LogError("Lose Panel is null!");
        }

        Time.timeScale = 0f;
    }

    void UpdateUI()
    {
        if (progressSlider != null)
        {
            progressSlider.maxValue = totalEnemies;
            progressSlider.value = enemiesKilled;
            Debug.Log($"Slider updated: {enemiesKilled}/{totalEnemies}");
        }
        else
        {
            Debug.LogWarning("Progress Slider is null!");
        }

        if (progressText != null)
        {
            progressText.text = $"{enemiesKilled}/{totalEnemies}";
        }
        else
        {
            Debug.LogWarning("Progress Text is null!");
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;

        if (SceneController.Instance != null)
        {
            SceneController.Instance.ReloadCurrentScene();
        }
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;

        if (SceneController.Instance != null)
        {
            SceneController.Instance.LoadMainMenu();
        }
    }


    public void DebugCheckEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Debug.Log($"DEBUG: Current enemies in scene: {enemies.Length}");
        foreach (GameObject enemy in enemies)
        {
            Debug.Log($"DEBUG: Enemy alive: {enemy.name}");
        }
    }
}