using System.Runtime.CompilerServices;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    private static AudioManager instance;

    public static AudioManager Instance
    {
        get
        {
            if (instance == null) Debug.Log("No hay AudioManager!");
            return instance;
        }

    }
    //Fin del Singleton

    public AudioSource musicSource;
    public AudioSource SFXSource;
    public AudioClip[] musicLibrary;
    public AudioClip[] SFXLibrary;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 🔥 CLAVE
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void PlayMusic(int musicToPlay)
    { 
        musicSource.clip = musicLibrary[musicToPlay];
        musicSource.Play(); //Reproducir la música desde el principio
    }

    public void PlaySFX(int sfxToPlay)
    {
        SFXSource.PlayOneShot(SFXLibrary[sfxToPlay]);
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void PauseAllAudio()
    {
        musicSource.Pause();
        SFXSource.Pause();
    }

    public void UnPauseAllAudio()
    {
        musicSource.UnPause();
        SFXSource.UnPause();
    }


    public void PauseMusic()
    {
        musicSource.Pause();
    }

    public void UnPauseMusic()
    {
        musicSource.UnPause();
    }
}