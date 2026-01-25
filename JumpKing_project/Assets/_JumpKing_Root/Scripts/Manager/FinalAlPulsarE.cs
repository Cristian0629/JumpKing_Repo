using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class FinalAlPulsarE : MonoBehaviour
{
    [Header("Cámaras")]
    public Camera camara13;
    public Camera camara15;

    [Header("Música")]
    public AudioSource musicNivel;
    public AudioSource musicVictoria;

    [Header("Volver al menú")]
    public string nombreEscenaMenu = "TitleScene";
    public float segundosDespuesDeMusica = 4f;

    [Header("Fade a negro (UI)")]
    public CanvasGroup blackFade;   // arrastra el CanvasGroup del BlackFade
    public float fadeDuration = 1f;

    private bool jugadorDentro = false;
    private bool yaHecho = false;

    void Start()
    {
        // Asegura que el fade empieza invisible
        if (blackFade != null) blackFade.alpha = 0f;

        // MUY IMPORTANTE: La música de victoria NO debe estar en loop
        if (musicVictoria != null) musicVictoria.loop = false;
    }

    void Update()
    {
        if (!jugadorDentro || yaHecho) return;

        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            yaHecho = true;

            // Cámaras
            if (camara13 != null) camara13.gameObject.SetActive(false);
            if (camara15 != null) camara15.gameObject.SetActive(true);

            // Música
            if (musicNivel != null) musicNivel.Stop();
            if (musicVictoria != null)
            {
                if (!musicVictoria.gameObject.activeSelf) musicVictoria.gameObject.SetActive(true);
                musicVictoria.Play();
            }

            // Secuencia final
            StartCoroutine(SecuenciaFinal());
        }
    }

    IEnumerator SecuenciaFinal()
    {
        // Espera a que termine la música de victoria
        if (musicVictoria != null)
        {
            while (musicVictoria.isPlaying)
                yield return null;
        }

        // Espera extra
        yield return new WaitForSeconds(segundosDespuesDeMusica);

        // Fade a negro
        if (blackFade != null)
            yield return StartCoroutine(FadeToBlack());

        // Cargar menú principal
        SceneManager.LoadScene(0);
    }

    IEnumerator FadeToBlack()
    {
        float t = 0f;
        float start = blackFade.alpha;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            blackFade.alpha = Mathf.Lerp(start, 1f, t / fadeDuration);
            yield return null;
        }

        blackFade.alpha = 1f;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) jugadorDentro = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) jugadorDentro = false;
    }
}
