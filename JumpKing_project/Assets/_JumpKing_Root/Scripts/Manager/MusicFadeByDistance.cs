using UnityEngine;

public class MusicFadeByDistance : MonoBehaviour
{
    [Header("Referencias")]
    public Transform player;        // Arrastra tu Player aquí
    public AudioSource musicSource; // Arrastra el AudioSource de la música de la escena

    [Header("Distancias")]
    public float fadeStartDistance = 6f; // A esta distancia empieza a bajar
    public float fadeEndDistance = 1.5f; // A esta distancia llega al mínimo

    [Header("Volumen")]
    [Range(0f, 1f)] public float minVolume = 0.15f; // volumen mínimo al estar muy cerca
    [Range(0f, 1f)] public float maxVolume = 1f;    // volumen normal lejos

    [Header("Suavizado")]
    public float smoothSpeed = 6f; // más alto = cambia más rápido

    void Reset()
    {
        // Intenta autollenar el player si tiene tag Player
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    void Update()
    {
        if (player == null || musicSource == null) return;

        float d = Vector2.Distance(player.position, transform.position);

        // 0 cuando está en fadeStart (lejos), 1 cuando está en fadeEnd (cerca)
        float t = Mathf.InverseLerp(fadeStartDistance, fadeEndDistance, d);

        // Si está lejos => t=0 => maxVolume. Si está cerca => t=1 => minVolume
        float targetVolume = Mathf.Lerp(maxVolume, minVolume, t);

        // Suavizado para que no sea brusco
        musicSource.volume = Mathf.Lerp(musicSource.volume, targetVolume, Time.deltaTime * smoothSpeed);
    }
}
