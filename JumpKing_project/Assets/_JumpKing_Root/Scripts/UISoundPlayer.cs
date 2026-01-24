using UnityEngine;

public class UISoundPlayer : MonoBehaviour
{
    [Header("UI Sounds")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip clickSound;

    private void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    public void PlayClick()
    {
        if (clickSound != null && audioSource != null)
            audioSource.PlayOneShot(clickSound);
    }
}
