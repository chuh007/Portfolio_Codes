using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; set; }
    
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetMasterVolume(float value)
    {
        mixer.SetFloat("Master", Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1)) * 20);
        PlayerPrefs.SetFloat("Master", value);

    }
    public void SetBGMVolume(float value)
    {
        mixer.SetFloat("BGM", Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1)) * 20);
        PlayerPrefs.SetFloat("BGM", value);
    }
    public void SetSFXVolume(float value)
    {
        mixer.SetFloat("SFX", Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1)) * 20);
        PlayerPrefs.SetFloat("SFX", value);
    }
    
    public void PlayBGM(AudioClip clip, bool loop = true)
    {
        if (bgmSource.clip == clip && bgmSource.isPlaying) return;
        bgmSource.clip = clip;
        bgmSource.loop = loop;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }
}
