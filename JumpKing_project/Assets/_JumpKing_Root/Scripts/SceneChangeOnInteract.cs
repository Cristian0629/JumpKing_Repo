using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SceneChangeOnInteract : MonoBehaviour
{
    [Header("Scene")]
    [SerializeField] string sceneToLoad = "LVL_Luisa";

    [Header("Input")]
    [SerializeField] InputActionReference interactAction;

    bool playerInside;

    void OnEnable()
    {
        if (interactAction != null)
            interactAction.action.Enable();
    }

    void OnDisable()
    {
        if (interactAction != null)
            interactAction.action.Disable();
    }

    void Update()
    {
        if (!playerInside) return;

        if (interactAction != null && interactAction.action.WasPressedThisFrame())
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInside = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInside = false;
    }
}
