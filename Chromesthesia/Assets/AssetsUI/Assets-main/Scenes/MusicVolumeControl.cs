using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MusicVolumeControl : MonoBehaviour
{
    [SerializeField] private Slider MusicSlider = null;
    [SerializeField] private Text Value = null;

    void Start()
    {
        if(!PlayerPrefs.HasKey("musicVolume"))
        {
            PlayerPrefs.SetFloat("musicVolume", 1);
            Load();
        }
        else
        {
            Load();
        }
    }
    public void ControlVolume(float volume)
    {
        AudioListener.volume = MusicSlider.value;
        Value.text = volume.ToString("0.0");
        Save();
    } // proceed

    private void Load()
    {
        // set value of volume slider = to value stored in music value key name 
        MusicSlider.value = PlayerPrefs.GetFloat("musicVolume");
    }

    private void Save()
    {
        //                    key name      value
        PlayerPrefs.SetFloat("musicVolume", MusicSlider.value);
    }
}
