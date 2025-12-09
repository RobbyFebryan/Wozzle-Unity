using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Mixer")]
    public AudioMixer audioMixer;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Audio Clips")]
    public AudioClip[] musicClips;   // index 0 = Main Menu, index 1 = Gameplay
    public AudioClip[] sfxClips;

    private int currentMusicIndex = -1;   // Mencegah musik restart

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);

        // === Register event saat scene berganti ===
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        // Load volume settings
        SetMusicVolume(PlayerPrefs.GetFloat("musicVolume", 0.5f));
        SetSFXVolume(PlayerPrefs.GetFloat("sfxVolume", 0.5f));

        // Mulai musik berdasarkan scene pertama
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    // === Auto Ganti Musik Berdasarkan Scene ===
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu")
        {
            PlayMusic(0); // Musik khusus Main Menu
        }
        else
        {
            PlayMusic(1); // Musik gameplay / scene lainnya
        }
    }

    // === VOLUME ===
    public void SetMusicVolume(float value)
    {
        value = Mathf.Clamp01(value);

        if (musicSource != null)
            musicSource.volume = value;

        PlayerPrefs.SetFloat("musicVolume", value);
    }

    public void SetSFXVolume(float value)
    {
        value = Mathf.Clamp01(value);

        if (sfxSource != null)
            sfxSource.volume = value;

        PlayerPrefs.SetFloat("sfxVolume", value);
    }

    // === MUSIC ===
    public void PlayMusic(int index)
    {
        if (index < 0 || index >= musicClips.Length) return;

        // Hindari musik restart jika index sama
        if (index == currentMusicIndex) return;

        currentMusicIndex = index;

        musicSource.clip = musicClips[index];
        musicSource.loop = true;
        musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource != null)
            musicSource.Stop();
    }

    // === SFX ===
    public void PlaySFX(int index)
    {
        if (index < 0 || index >= sfxClips.Length) return;

        sfxSource.PlayOneShot(sfxClips[index]);
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
            sfxSource.PlayOneShot(clip);
    }
}
