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
    [SerializeField] GameObject pauseRoot;
    [SerializeField] string mainMenuScene = "TitleScene";

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

    public void Pause()
    {
        isPaused = true;
        PauseState.IsPaused = true;
        if (pauseRoot != null) pauseRoot.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        isPaused = false;
        PauseState.IsPaused = false;
        if (pauseRoot != null) pauseRoot.SetActive(false);
        Time.timeScale = 1f;
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        PauseState.IsPaused = false;
        SceneManager.LoadScene(mainMenuScene);
    }
}
