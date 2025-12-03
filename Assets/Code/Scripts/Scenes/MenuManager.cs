using UnityEngine;
using System.Collections;

public class MenuManager : MonoBehaviour
{
    private MenuAudioManager audioManager;

    void Start()
    {
        audioManager = MenuAudioManager.GetInstance();
    }

    public void PlayGame()
    {
        PlayButtonClickSound();
        StartCoroutine(LoadGameAfterSound());
    }

    public void ShowCredits()
    {
        PlayButtonClickSound();
        StartCoroutine(LoadCreditsAfterSound());
    }

    public void BackToMainMenu()
    {
        PlayButtonClickSound();
        StartCoroutine(LoadMainMenuAfterSound());
    }

    public void QuitGame()
    {
        PlayButtonClickSound();
        StartCoroutine(QuitAfterSound());
    }

    public void OnButtonHover()
    {
        if (audioManager != null)
        {
            audioManager.OnButtonHover();
        }
    }

    private void PlayButtonClickSound()
    {
        if (audioManager != null)
        {
            audioManager.OnButtonClick();
        }
    }

    private IEnumerator LoadGameAfterSound()
    {
        yield return new WaitForSeconds(0.3f);

        if (audioManager != null)
        {
            audioManager.StopMenuMusic();
        }

        if (SceneController.Instance != null)
        {
            SceneController.Instance.LoadMainGame();
        }
    }

    private IEnumerator LoadCreditsAfterSound()
    {
        yield return new WaitForSeconds(0.3f);

        if (SceneController.Instance != null)
        {
            SceneController.Instance.LoadCreditsScene();
        }
    }

    private IEnumerator LoadMainMenuAfterSound()
    {
        yield return new WaitForSeconds(0.3f);

        if (SceneController.Instance != null)
        {
            SceneController.Instance.LoadMainMenu();
        }
    }

    private IEnumerator QuitAfterSound()
    {
        yield return new WaitForSeconds(0.3f);

        if (SceneController.Instance != null)
        {
            SceneController.Instance.QuitGame();
        }
    }
}