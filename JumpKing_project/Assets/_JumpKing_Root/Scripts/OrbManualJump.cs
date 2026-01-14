using UnityEngine;

public class OrbManualJump : MonoBehaviour
{
    [Header("Boost")]
    [SerializeField] float verticalBoost = 12f;
    [SerializeField] float horizontalBoost = 7f;

    [Header("Timing")]
    [SerializeField] float inputWindow = 0.18f; // tiempo para pulsar salto tras tocar el orbe

    [Header("Rules")]
    [SerializeField] bool onlyInAir = true;
    [SerializeField] bool oneUsePerTouch = true;

    float windowTimer = 0f;
    bool playerInside = false;
    bool usedThisTouch = false;

    Rigidbody2D cachedPlayerRb;

    void Update()
    {
        if (windowTimer > 0f)
            windowTimer -= Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // detecta player aunque el collider est� en un hijo
        Transform root = other.transform.root;
        if (!root.CompareTag("Player")) return;

        cachedPlayerRb = root.GetComponent<Rigidbody2D>();
        if (cachedPlayerRb == null) return;

        if (onlyInAir && Mathf.Abs(cachedPlayerRb.linearVelocity.y) < 0.05f)
            return;

        playerInside = true;
        windowTimer = inputWindow;
        usedThisTouch = false;

        // Le avisamos al PlayerController que este orbe es "candidato"
        PlayerController pc = root.GetComponent<PlayerController>();
        if (pc != null) pc.SetManualOrb(this);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Transform root = other.transform.root;
        if (!root.CompareTag("Player")) return;

        playerInside = false;
        windowTimer = 0f;

        PlayerController pc = root.GetComponent<PlayerController>();
        if (pc != null) pc.ClearManualOrb(this);
    }

    // Lo llama el PlayerController cuando el jugador pulsa salto en el aire
    public bool TryActivate(Vector2 moveInput)
    {
        if (!playerInside) return false;
        if (windowTimer <= 0f) return false;
        if (oneUsePerTouch && usedThisTouch) return false;
        if (cachedPlayerRb == null) return false;

        // Direcci�n (solo izquierda/derecha)
        float xDir = Mathf.Clamp(moveInput.x, -1f, 1f);

        // Si no pulsa direcci�n, puedes elegir:
        // - que impulse recto hacia arriba (xDir = 0)
        // - o que use la direcci�n actual de velocidad.
        // Aqu� lo dejamos en 0 para que sea �neutral� si no pulsa nada.
        Vector2 boost = new Vector2(xDir * horizontalBoost, verticalBoost);

        cachedPlayerRb.linearVelocity = boost;

        usedThisTouch = true;
        windowTimer = 0f; // se consume la ventana al usarlo
        return true;
    }
}
