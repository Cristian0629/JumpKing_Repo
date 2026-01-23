using UnityEngine;

public class OrbManualJump : MonoBehaviour
{
    [Header("Boost")]
    [SerializeField] float verticalBoost = 12f;
    [SerializeField] float horizontalBoost = 7f;

    [Header("Rules")]
    [SerializeField] bool onlyInAir = true;

    // Si lo activas, solo te deja 1 salto por cada vez que entras al orbe.
    // Si lo desactivas, puedes spamearlo estando dentro.
    [SerializeField] bool oneUsePerTouch = false;

    bool playerInside = false;
    bool usedThisTouch = false;

    Rigidbody2D cachedPlayerRb;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Transform root = other.transform.root;
        if (!root.CompareTag("Player")) return;

        cachedPlayerRb = root.GetComponent<Rigidbody2D>();
        if (cachedPlayerRb == null) return;

        playerInside = true;
        usedThisTouch = false;

        // Avisamos al PlayerController que este orbe está disponible
        PlayerController pc = root.GetComponent<PlayerController>();
        if (pc != null) pc.SetManualOrb(this);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Transform root = other.transform.root;
        if (!root.CompareTag("Player")) return;

        playerInside = false;
        cachedPlayerRb = null;

        PlayerController pc = root.GetComponent<PlayerController>();
        if (pc != null) pc.ClearManualOrb(this);
    }

    // Lo llama el PlayerController cuando el jugador pulsa salto
    public bool TryActivate(Vector2 moveInput)
    {
        if (!playerInside) return false;
        if (cachedPlayerRb == null) return false;

        if (onlyInAir && Mathf.Abs(cachedPlayerRb.linearVelocity.y) < 0.05f)
            return false;

        if (oneUsePerTouch && usedThisTouch) return false;

        float xDir = Mathf.Clamp(moveInput.x, -1f, 1f);

        Vector2 boost = new Vector2(xDir * horizontalBoost, verticalBoost);
        cachedPlayerRb.linearVelocity = boost;

        usedThisTouch = true;
        return true;
    }
}
