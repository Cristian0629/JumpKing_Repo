using System.Collections;
using UnityEngine;

public class BreakablePlatform : MonoBehaviour
{
    [Header("Timing")]
    [SerializeField] float breakDelay = 0.6f;
    [SerializeField] float destroyAfter = 0.05f;

    [Header("Wobble (Visual Only)")]
    [SerializeField] Transform visual;          // <-- arrastra aquí el hijo (sprite)
    [SerializeField] float wobblePos = 0.04f;   // movimiento pequeño
    [SerializeField] float wobbleSpeed = 35f;

    [Header("Rules")]
    [SerializeField] bool breakOnlyIfPlayerAbove = true;
    [SerializeField] LayerMask playerLayer;

    bool triggered;
    Vector3 visualStartLocalPos;
    Collider2D col;

    void Awake()
    {
        col = GetComponent<Collider2D>();

        if (visual == null) visual = transform; // fallback
        visualStartLocalPos = visual.localPosition;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (triggered) return;
        if (!IsPlayer(collision.collider)) return;

        if (breakOnlyIfPlayerAbove)
        {
            bool fromAbove = false;
            foreach (var c in collision.contacts)
            {
                if (c.normal.y > 0.5f) { fromAbove = true; break; }
            }
            if (!fromAbove) return;
        }

        triggered = true;
        StartCoroutine(BreakRoutine());
    }

    bool IsPlayer(Collider2D other)
    {
        if (playerLayer.value != 0)
        {
            if (((1 << other.gameObject.layer) & playerLayer) != 0) return true;
        }
        return other.transform.root.CompareTag("Player");
    }

    IEnumerator BreakRoutine()
    {
        float t = 0f;

        while (t < breakDelay)
        {
            t += Time.deltaTime;

            // Temblor VISUAL (no mueve collider)
            float s = Mathf.Sin(Time.time * wobbleSpeed);
            visual.localPosition = visualStartLocalPos + Vector3.right * (s * wobblePos);

            yield return null;
        }

        // restaurar
        visual.localPosition = visualStartLocalPos;

        // romper
        if (col != null) col.enabled = false;
        yield return new WaitForSeconds(destroyAfter);
        Destroy(gameObject);
    }
}
