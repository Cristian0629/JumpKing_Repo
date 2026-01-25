using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenFader : MonoBehaviour
{
    public static ScreenFader Instance;

    [Header("Fade")]
    public CanvasGroup blackFade;
    public float fadeDuration = 1f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (blackFade != null)
                blackFade.alpha = 0f;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void FadeToBlack()
    {
        StartCoroutine(Fade(0f, 1f));
    }

    public void FadeFromBlack()
    {
        StartCoroutine(Fade(1f, 0f));
    }

    public void FadeAndLoadScene(string sceneName)
    {
        StartCoroutine(FadeLoad(sceneName));
    }

    IEnumerator Fade(float from, float to)
    {
        float t = 0f;
        blackFade.alpha = from;

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            blackFade.alpha = Mathf.Lerp(from, to, t / fadeDuration);
            yield return null;
        }

        blackFade.alpha = to;
    }

    IEnumerator FadeLoad(string sceneName)
    {
        yield return Fade(0f, 1f);
        SceneManager.LoadScene(sceneName);
        yield return null; // esperar 1 frame
        yield return Fade(1f, 0f);
    }
}
