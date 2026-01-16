using System.Collections;
using UnityEngine;

public class BreakablePlatform : MonoBehaviour
{
    [Header("Timing")]
    [SerializeField] float breakDelay = 0.6f;
    [SerializeField] float respawnDelayMin = 2f;
    [SerializeField] float respawnDelayMax = 3f;

    [Header("Wobble (Visual Only)")]
    [SerializeField] Transform visual;
    [SerializeField] float wobblePos = 0.04f;
    [SerializeField] float wobbleSpeed = 35f;

    [Header("Rules")]
    [SerializeField] bool breakOnlyIfPlayerAbove = true;
    [SerializeField] LayerMask playerLayer;

    bool triggered;
    Vector3 visualStartLocalPos;
    Collider2D col;

    // Renderers a ocultar/mostrar (sprite, tilemap, etc.)
    Renderer[] renderersToToggle;

    void Awake()
    {
        col = GetComponent<Collider2D>();

        if (visual == null) visual = transform; // puedes seguir usando esto
        visualStartLocalPos = visual.localPosition;

        // Pillamos TODOS los renderers dentro de "visual"
        renderersToToggle = visual.GetComponentsInChildren<Renderer>(true);
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

            float s = Mathf.Sin(Time.time * wobbleSpeed);
            visual.localPosition = visualStartLocalPos + Vector3.right * (s * wobblePos);

            yield return null;
        }

        // Restaurar posición visual
        visual.localPosition = visualStartLocalPos;

        // "Romper": quitar colisión y ocultar
        if (col != null) col.enabled = false;
        SetVisual(false);

        // Esperar 2-3s (aleatorio)
        float respawnDelay = Random.Range(respawnDelayMin, respawnDelayMax);
        yield return new WaitForSeconds(respawnDelay);

        // Reaparecer
        SetVisual(true);
        if (col != null) col.enabled = true;

        triggered = false;
    }

    void SetVisual(bool on)
    {
        if (renderersToToggle == null) return;
        for (int i = 0; i < renderersToToggle.Length; i++)
        {
            if (renderersToToggle[i] != null)
                renderersToToggle[i].enabled = on;
        }
    }
}


