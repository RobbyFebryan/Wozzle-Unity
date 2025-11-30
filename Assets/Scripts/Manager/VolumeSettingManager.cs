using UnityEngine;
using UnityEngine.UI;

public class VolumeSettingManager : MonoBehaviour
{
    [Header("Sliders")]
    public Slider musicSlider;
    public Slider sfxSlider;

    private void Start()
    {
        // Load saved volume
        if (PlayerPrefs.HasKey("musicVolume"))
            musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
        else
            musicSlider.value = 1f;

        if (PlayerPrefs.HasKey("sfxVolume"))
            sfxSlider.value = PlayerPrefs.GetFloat("sfxVolume");
        else
            sfxSlider.value = 1f;

        // Assign listeners
        musicSlider.onValueChanged.AddListener(OnMusicChange);
        sfxSlider.onValueChanged.AddListener(OnSFXChange);
        
        // Apply initial volume to SoundManager
        OnMusicChange(musicSlider.value);
        OnSFXChange(sfxSlider.value);
    }

    public void OnMusicChange(float value)
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.SetMusicVolume(value);
        }
    }

    public void OnSFXChange(float value)
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.SetSFXVolume(value);
        }
    }

    private void OnDestroy()
    {
        // Remove listeners to prevent memory leaks
        if (musicSlider != null)
            musicSlider.onValueChanged.RemoveListener(OnMusicChange);
        if (sfxSlider != null)
            sfxSlider.onValueChanged.RemoveListener(OnSFXChange);
    }
}