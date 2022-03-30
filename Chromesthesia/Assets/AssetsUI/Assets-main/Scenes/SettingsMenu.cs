using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    // SerializeField is for a private variable which will also show up in the editor 
    [SerializeField] private Slider SpeedSlider = null;
    [SerializeField] private Slider MusicSlider = null;
    [SerializeField] private Slider SFXSlider = null;
    [SerializeField] private Text SpeedValue = null;
    [SerializeField] private Text MusicValue = null;
    [SerializeField] private Text SFXValue = null;

    void Start()
    {
        if (!PlayerPrefs.HasKey("musicVolume"))
        {
            PlayerPrefs.SetFloat("musicVolume", 100);
            LoadMusic();
        }
        else
        {
            LoadMusic();
        }

        if (!PlayerPrefs.HasKey("SFXVolume"))
        {
            PlayerPrefs.SetFloat("SFXVolume", 100);
            LoadSFX();
        }
        else
        {
            LoadSFX();
        }

        if (!PlayerPrefs.HasKey("GameSpeed"))
        {
            PlayerPrefs.SetFloat("GameSpeed", 100);
            LoadSpeed();
        }
        else
        {
            LoadSpeed();
        }
    }

// =============================================================================================
    public void ControlMusic(float volume)
    {
        MusicValue.text = volume.ToString("0");
        SaveMusic();
    }

    private void LoadMusic()
    {
        // set value of volume slider = to value stored in music value key name 
        MusicSlider.value = PlayerPrefs.GetFloat("musicVolume");
        AudioListener.volume = MusicSlider.value;
    }

    private void SaveMusic()
    {
        //                    key name      value
        PlayerPrefs.SetFloat("musicVolume", MusicSlider.value);
        LoadMusic();
    }

// =============================================================================================
    public void ControlSFX(float volume)
    {
        SFXValue.text = volume.ToString("0");
        SaveSFX();
    }
    private void LoadSFX()
    {
        // set value of volume slider = to value stored in music value key name 
        SFXSlider.value = PlayerPrefs.GetFloat("SFXVolume");
        // AudioListener.volume = SFXSlider.value;
    }

    private void SaveSFX()
    {
        //                    key name      value
        PlayerPrefs.SetFloat("SFXVolume", SFXSlider.value);
    }

// =============================================================================================
    public void ControlSpeed(float speed)
    {
        SpeedValue.text = speed.ToString("0");
        SaveSpeed();
    }
    
    private void LoadSpeed()
    {
        // set value of volume slider = to value stored in music value key name 
        SpeedSlider.value = PlayerPrefs.GetFloat("GameSpeed");
    }

    private void SaveSpeed()
    {
        //                    key name      value
        PlayerPrefs.SetFloat("GameSpeed", SpeedSlider.value);
    }
}
