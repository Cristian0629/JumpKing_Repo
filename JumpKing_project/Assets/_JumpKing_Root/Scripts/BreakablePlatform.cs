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

    [Header("Above Check (Reliable)")]
    [SerializeField] float aboveEpsilon = 0.05f;
    [SerializeField] float mustBeFallingVelY = 0.05f;

    [Header("Sprites")]
    [SerializeField] Sprite normalSprite;
    [SerializeField] Sprite crackedSprite;

    [Header("Wood FX (Child Particle System)")]
    [SerializeField] ParticleSystem woodParticles; // Pon aquí el Particle System hijo (WoodParticles)

    bool triggered;
    Vector3 visualStartLocalPos;
    Collider2D col;

    Renderer[] renderersToToggle;
    SpriteRenderer sr;

    void Awake()
    {
        col = GetComponent<Collider2D>();

        if (visual == null) visual = transform;
        visualStartLocalPos = visual.localPosition;

        renderersToToggle = visual.GetComponentsInChildren<Renderer>(true);

        sr = visual.GetComponent<SpriteRenderer>();
        if (sr != null && normalSprite != null)
            sr.sprite = normalSprite;

        // Si no lo asignaste a mano, intenta encontrarlo en hijos (incluso si está desactivado)
        if (woodParticles == null)
            woodParticles = GetComponentInChildren<ParticleSystem>(true);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (triggered) return;
        if (!IsPlayer(collision.collider)) return;

        if (breakOnlyIfPlayerAbove && !CameFromAbove(collision))
            return;

        // Cambiar a sprite "agrietado" al pisar
        if (sr != null && crackedSprite != null)
            sr.sprite = crackedSprite;

        triggered = true;
        StartCoroutine(BreakRoutine());
    }

    bool CameFromAbove(Collision2D collision)
    {
        if (col == null) return false;

        Bounds playerB = collision.collider.bounds;
        Bounds platB = col.bounds;

        // Debe estar por encima de la plataforma
        bool playerIsAboveTop = playerB.min.y >= (platB.max.y - aboveEpsilon);
        if (!playerIsAboveTop) return false;

        // Debe venir cayendo (si golpea desde abajo, esto suele ser positivo)
        if (collision.relativeVelocity.y > mustBeFallingVelY) return false;

        return true;
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

        // FX de madera justo al romper
        PlayWoodFX();

        // Romper: quitar colisión y ocultar
        if (col != null) col.enabled = false;
        SetVisual(false);

        // Esperar respawn
        float respawnDelay = Random.Range(respawnDelayMin, respawnDelayMax);
        yield return new WaitForSeconds(respawnDelay);

        // Reaparecer
        SetVisual(true);
        if (col != null) col.enabled = true;

        // Volver al sprite normal
        if (sr != null && normalSprite != null)
            sr.sprite = normalSprite;

        triggered = false;
    }

    void PlayWoodFX()
    {
        if (woodParticles == null) return;

        // Por si el objeto está desactivado en la jerarquía
        woodParticles.gameObject.SetActive(true);

        // Reinicia para que siempre se vea el burst desde 0
        woodParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        woodParticles.Play(true);
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
