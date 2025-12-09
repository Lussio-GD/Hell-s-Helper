using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonSound : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
{
    private MenuAudioManager audioManager;

    void Start()
    {
        audioManager = MenuAudioManager.GetInstance();
        if (audioManager == null)
        {
            Debug.LogError("ButtonSound: AudioManager not found!");
        }
        else
        {
            Debug.Log("ButtonSound: Ready!");
        }
    }

    
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Button clicked: " + gameObject.name);
        if (audioManager != null)
        {
            audioManager.OnButtonClick();
        }
    }

    
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Button hover: " + gameObject.name);
        if (audioManager != null)
        {
            audioManager.OnButtonHover();
        }
    }
}