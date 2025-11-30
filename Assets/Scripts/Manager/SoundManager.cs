using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Mixer")]
    public AudioMixer audioMixer;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Audio Clips")]
    public AudioClip[] musicClips;
    public AudioClip[] sfxClips;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // FIX: Jadikan root agar DDOL pasti bekerja
        transform.SetParent(null);

        DontDestroyOnLoad(gameObject);
    }


    private void Start()
    {
        // set volume
        if (PlayerPrefs.HasKey("musicVolume"))
            SetMusicVolume(PlayerPrefs.GetFloat("musicVolume"));
        else
            SetMusicVolume(0.5f);

        if (PlayerPrefs.HasKey("sfxVolume"))
            SetSFXVolume(PlayerPrefs.GetFloat("sfxVolume"));
        else
            SetSFXVolume(0.5f);

        // >>> Tambahkan ini <<<
        if (musicClips.Length > 0)
            PlayMusic(0);
    }

    // --- VOLUME CONTROL ---
    // Method ini WAJIB ada agar VolumeSettingManager bisa mengatur volume!
    
    public void SetMusicVolume(float value)
    {
        // Clamp value between 0 and 1
        value = Mathf.Clamp01(value);
        
        // Set volume langsung ke AudioSource
        if (musicSource != null)
        {
            musicSource.volume = value;
        }
        
        // Save to PlayerPrefs
        PlayerPrefs.SetFloat("musicVolume", value);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float value)
    {
        // Clamp value between 0 and 1
        value = Mathf.Clamp01(value);
        
        // Set volume langsung ke AudioSource
        if (sfxSource != null)
        {
            sfxSource.volume = value;
        }
        
        // Save to PlayerPrefs
        PlayerPrefs.SetFloat("sfxVolume", value);
        PlayerPrefs.Save();
    }

    // --- MUSIC ---

    public void PlayMusic(int index)
    {
        if (index < 0 || index >= musicClips.Length) return;

        musicSource.clip = musicClips[index];
        musicSource.loop = true;
        musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }

    // --- SFX ---
    
    public void PlaySFX(int index)
    {
        if (index < 0 || index >= sfxClips.Length) return;

        sfxSource.PlayOneShot(sfxClips[index]);
    }
    
    public void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }
}