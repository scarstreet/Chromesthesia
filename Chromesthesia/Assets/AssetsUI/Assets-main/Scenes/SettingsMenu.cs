using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    // SerializeField is for a private variable which will also show up in the editor
    [SerializeField] Slider SpeedSlider = null;
    [SerializeField] Slider MusicSlider = null;
    [SerializeField] Slider SFXSlider = null;
    [SerializeField] Text SpeedValue = null;
    [SerializeField] Text MusicValue = null;
    [SerializeField] Text SFXValue = null;

    void Start()
    {
        if (!PlayerPrefs.HasKey("musicVolume"))
        {
            PlayerPrefs.SetFloat("musicVolume", 100);
            LoadMusic_slider();
        }
        else
        {
            LoadMusic_slider();
        }

        if (!PlayerPrefs.HasKey("SFXVolume"))
        {
            PlayerPrefs.SetFloat("SFXVolume", 50);
            LoadSFX_slider();
        }
        else
        {
            LoadSFX_slider();
        }

        if (!PlayerPrefs.HasKey("gameSpeed"))
        {
            PlayerPrefs.SetFloat("gameSpeed", 50);
            LoadSpeed_slider();
        }
        else
        {
            LoadSpeed_slider();
        }
    }

// ======================================================================================
    public void ControlMusic_slider(float volume)
    {
        MusicValue.text = volume.ToString("0");
        SaveMusic_slider();
    }

    private void LoadMusic_slider()
    {
        // set value of volume slider equal to value stored in music value key name 
        MusicSlider.value = PlayerPrefs.GetFloat("musicVolume");
    }

    private void SaveMusic_slider()
    {
        //                    key name      value
        PlayerPrefs.SetFloat("musicVolume", MusicSlider.value);
    }
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ======================================================================================
    public void ControlSFX_slider(float sfxvolume)
    {
        SFXValue.text = sfxvolume.ToString("0");
        SaveSFX_slider();
    }
    private void LoadSFX_slider()
    {
        // set value of volume slider equal to value stored in music value key name 
        SFXSlider.value = PlayerPrefs.GetFloat("SFXVolume");
    }

    private void SaveSFX_slider()
    {
        //                    key name      value
        PlayerPrefs.SetFloat("SFXVolume", SFXSlider.value);
    }
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ======================================================================================
    public void ControlSpeed_slider(float speed)
    {
        AudioListener.volume = SpeedSlider.value;
        SpeedValue.text = speed.ToString("0");
        SaveSpeed_slider();
    }
    
    private void LoadSpeed_slider()
    {
        // set value of volume slider equal to value stored in music value key name 
        SpeedSlider.value = PlayerPrefs.GetFloat("gameSpeed");
    }

    private void SaveSpeed_slider()
    {
        //                    key name      value
        PlayerPrefs.SetFloat("gameSpeed", SpeedSlider.value);
    }
}
