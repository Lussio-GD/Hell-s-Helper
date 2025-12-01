using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuManager : MonoBehaviour
{
    private MenuAudioManager audioManager;

    void Start()
    {
        
        audioManager = MenuAudioManager.GetInstance();

        if (audioManager == null)
        {
            Debug.LogError("MenuAudioManager not found! Make sure there's an AudioManager in the scene.");
        }
        else
        {
            Debug.Log("AudioManager found and ready!");
        }
    }

    public void PlayGame()
    {
        Debug.Log("PlayGame called!");
        PlayButtonClickSound();
        StartCoroutine(LoadAfterSound("MainGame", true));
    }

    public void ShowCredits()
    {
        Debug.Log("ShowCredits called!");
        PlayButtonClickSound();
        StartCoroutine(LoadAfterSound("CreditsScene", false));
    }

    public void BackToMainMenu()
    {
        Debug.Log("BackToMainMenu called!");
        PlayButtonClickSound();
        StartCoroutine(LoadAfterSound("MainMenu", false));
    }

    public void QuitGame()
    {
        Debug.Log("QuitGame called!");
        PlayButtonClickSound();
        StartCoroutine(QuitAfterSound());
    }

    public void OnButtonHover()
    {
        Debug.Log("Button hover detected!");
        if (audioManager != null)
        {
            audioManager.OnButtonHover();
        }
        else
        {
            Debug.LogWarning("AudioManager is null on hover!");
        }
    }

    private void PlayButtonClickSound()
    {
        if (audioManager != null)
        {
            audioManager.OnButtonClick();
        }
        else
        {
            Debug.LogWarning("AudioManager is null on click!");
        }
    }

    private IEnumerator LoadAfterSound(string sceneName, bool stopMusic)
    {
        // Wait for click sound to play completely
        yield return new WaitForSeconds(0.3f);

        // Stop music if going to game scene
        if (stopMusic && audioManager != null)
        {
            audioManager.StopMenuMusic();
        }

        Debug.Log($"Loading scene: {sceneName}");
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator QuitAfterSound()
    {
        yield return new WaitForSeconds(0.3f);

        Debug.Log("Quitting application...");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}