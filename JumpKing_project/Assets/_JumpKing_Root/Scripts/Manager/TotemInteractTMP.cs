using UnityEngine;
using UnityEngine.InputSystem;

public class TotemInteractTMP : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject promptObject;               // Text (TMP) (GameObject)
    [SerializeField] InputActionReference interactAction;   // Player/Interact

    [Header("Float Animation")]
    [SerializeField] float floatAmplitude = 0.06f;
    [SerializeField] float floatSpeed = 2f;

    Transform promptTransform;
    Vector3 promptStartLocalPos;
    bool playerInside;

    void Awake()
    {
        if (promptObject != null)
        {
            promptTransform = promptObject.transform;
            promptStartLocalPos = promptTransform.localPosition;
            promptObject.SetActive(false);
        }
    }

    void OnEnable()
    {
        if (interactAction != null) interactAction.action.Enable();
    }

    void OnDisable()
    {
        if (interactAction != null) interactAction.action.Disable();
    }

    void Update()
    {
        if (PauseState.IsPaused) return;

        // Flotación
        if (playerInside && promptObject != null && promptObject.activeSelf && promptTransform != null)
        {
            float y = Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
            promptTransform.localPosition = promptStartLocalPos + new Vector3(0f, y, 0f);
        }

        // Input
        if (!playerInside) return;
        if (interactAction == null) return;

        if (interactAction.action.WasPressedThisFrame())
        {
            Debug.Log("Interacción con el tótem!");
            // aquí tu lógica real
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.transform.root.CompareTag("Player")) return;

        playerInside = true;
        if (promptObject != null) promptObject.SetActive(true);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.transform.root.CompareTag("Player")) return;

        playerInside = false;

        if (promptObject != null) promptObject.SetActive(false);

        // Reset posición al salir (para que no se quede desplazado)
        if (promptTransform != null)
            promptTransform.localPosition = promptStartLocalPos;
    }
}
