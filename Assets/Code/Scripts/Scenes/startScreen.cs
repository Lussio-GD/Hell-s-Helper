using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;




public class startScreen : MonoBehaviour
{


    [SerializeField] private PlayerCombat playerCombat;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private GameObject StartScreen;

     
    [SerializeField] private InputActionReference InputAction;
    bool inputAction = false;

    void Awake()
    {
        playerCombat.enabled = false;
        playerMovement.enabled = false;
        Time.timeScale = 0f;

    }



    private void Start()
    {
        InputAction.action.performed += HandleInputAction;

    }


    void HandleInputAction(InputAction.CallbackContext context)
    {
        inputAction = true;
        Debug.Log(inputAction);

    }




    private void Update()
    {
        if (inputAction == true)
        {
            playerCombat.enabled = true;
            playerMovement.enabled = true;
            Time.timeScale = 1f;
            Destroy(StartScreen);
        }
    }
}
