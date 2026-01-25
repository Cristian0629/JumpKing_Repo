using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

// ✅ ESTADO GLOBAL DE PAUSA
public static class PauseState
{
    public static bool IsPaused;
}

public class PauseMenu : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] GameObject pauseRoot;
    [SerializeField] string mainMenuScene = "TitleScene";

    [Header("UI Audio")]
    [SerializeField] AudioSource uiSfxSource;
    [SerializeField] AudioClip uiClickClip;
    [SerializeField, Range(0f, 0.2f)] float uiActionDelay = 0.06f; // pequeño delay para que “entre” el click

    bool isPaused;

    void Awake()
    {
        // ✅ AUTO-ASIGNAR pauseRoot SI NO ESTÁ PUESTO
        if (pauseRoot == null)
        {
            Canvas canvas = GetComponentInChildren<Canvas>(true);
            if (canvas != null)
                pauseRoot = canvas.gameObject;
        }

        // ✅ Asegura AudioSource (en ESTE objeto, no dentro de pauseRoot)
        if (uiSfxSource == null)
            uiSfxSource = GetComponent<AudioSource>();

        if (pauseRoot != null)
            pauseRoot.SetActive(false);

        isPaused = false;
        PauseState.IsPaused = false;
        Time.timeScale = 1f;
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused) Resume();
            else Pause();
        }
    }

    // =========================
    // PAUSA / RESUME
    // =========================
    public void Pause()
    {
        isPaused = true;
        PauseState.IsPaused = true;

        if (pauseRoot != null) pauseRoot.SetActive(true);

        // ✅ Pausar música del nivel
        if (AudioManager.Instance != null)
            AudioManager.Instance.PauseMusic();

        Time.timeScale = 0f;
    }

    public void Resume()
    {
        isPaused = false;
        PauseState.IsPaused = false;

        if (pauseRoot != null) pauseRoot.SetActive(false);

        Time.timeScale = 1f;

        // ✅ Reanudar música del nivel
        if (AudioManager.Instance != null)
            AudioManager.Instance.UnPauseMusic();
    }


    // =========================
    // MÉTODOS PARA BOTONES (USAR ESTOS EN ONCLICK)
    // =========================

    // Botón "Continue"
    public void Button_Continue()
    {
        PlayUIClick();

        // Si estás en pausa (timeScale 0), usa tiempo real.
        StartCoroutine(ResumeAfterDelayRealtime(uiActionDelay));
    }

    // Botón "Main Menu"
    public void Button_MainMenu()
    {
        PlayUIClick();
        StartCoroutine(GoToMainMenuAfterDelayRealtime(uiActionDelay));
    }

    // =========================
    // MAIN MENU
    // =========================
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        PauseState.IsPaused = false;
        isPaused = false;

        if (AudioManager.Instance != null)
            AudioManager.Instance.StopMusic();


        SceneManager.LoadScene(mainMenuScene);
    }

    // =========================
    // AUDIO HELPERS
    // =========================
    void PlayUIClick()
    {
        if (uiSfxSource == null || uiClickClip == null) return;

        uiSfxSource.PlayOneShot(uiClickClip);
    }

    IEnumerator ResumeAfterDelayRealtime(float delay)
    {
        // Mantener pausa mientras suena el click un instante
        if (delay > 0f)
            yield return new WaitForSecondsRealtime(delay);

        Resume();
    }

    IEnumerator GoToMainMenuAfterDelayRealtime(float delay)
    {
        if (delay > 0f)
            yield return new WaitForSecondsRealtime(delay);

        GoToMainMenu();
    }
}
