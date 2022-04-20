using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEngine.UI;

public class SettingsScript : MonoBehaviour
{
  // Start is called before the first frame update

  // SerializeField is for a private variable which will also show up in the editor
    [SerializeField] Slider SpeedSlider = null;
    [SerializeField] Slider SFXSlider = null;
    [SerializeField] Slider MusicSlider = null;
    [SerializeField] Text SpeedValue = null;
    [SerializeField] Text SFXValue = null;
    [SerializeField] Text MusicValue = null;
  private static string prevActiveScene;
  public static bool settingsOpen = false;
  public Image transitionPanel;
  void Start()
  {
    transitionPanel.CrossFadeAlpha(0, 0.5f, false);
    settingsOpen = true;
    prevActiveScene = SceneManager.GetActiveScene().name;
    if (prevActiveScene.Contains("SettingsScene"))
    {
      // TODO - MAKE A BETTER WAY TO STORE PREVIOUS SCREENS!!!
      prevActiveScene = "MainTitleScreen";
    }
    Debug.Log(prevActiveScene);
    SceneManager.UnloadSceneAsync(prevActiveScene);

    if (!PlayerPrefs.HasKey("gameSpeed"))
    {
        PlayerPrefs.SetFloat("gameSpeed", 5);
        LoadSpeed_slider();
    }
    else
    {
        LoadSpeed_slider();
    }

    if (!PlayerPrefs.HasKey("SFXVolume"))
    {
        PlayerPrefs.SetFloat("SFXVolume", 100);
        LoadSFX_slider();
    }
    else
    {
        LoadSFX_slider();
    }

    if (!PlayerPrefs.HasKey("musicVolume"))
    {
        PlayerPrefs.SetFloat("musicVolume", 100);
        LoadMusic_slider();
    }
    else
    {
        LoadMusic_slider();
    }

  }
  private void OnDestroy()
  {
    settingsOpen = false;
  }

  // Update is called once per frame
  void Update()
  { }

  public void toBack()
  {
    StartCoroutine(changeScene(prevActiveScene));
  }
  private bool IsPointerOverUIObject()
  {
    var eventDataCurrentPosition = new PointerEventData(EventSystem.current)
    {
      position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)
    };
    var results = new List<RaycastResult>();
    EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
    return results.Count > 0;
  }
  IEnumerator changeScene(string scene)
  {
    bool fadeDone = false;
    while (!fadeDone)
    {
      transitionPanel.CrossFadeAlpha(1, 0.5f, true);
      fadeDone = true;
      yield return new WaitForSecondsRealtime(.5f);
    }
    SceneManager.LoadScene(scene, LoadSceneMode.Single);
  }

  public void ResetSettings()
  {
    SpeedSlider.value = 5;
    SFXSlider.value = 100;
    MusicSlider.value = 100;
  }

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
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public void ControlSpeed_buttonUp()
    {
      // Debug.Log("OH YA, Bring da music Upp");
      SpeedSlider.value += 1;
      SaveSpeed_slider();
    }

    public void ControlSpeed_buttonDown()
    {
      SpeedSlider.value -= 1;
      SaveSpeed_slider();
    }
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
        public void ControlSFX_buttonUp()
    {
      // Debug.Log("OH YA, Bring da music Upp");
      SFXSlider.value += 1;
      SaveSFX_slider();
    }
    public void ControlSFX_buttonDown()
    {
      SFXSlider.value -= 1;
      SaveSFX_slider();
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
    public void ControlMusic_buttonUp()
    {
      // Debug.Log("OH YA, Bring da music Upp");
      MusicSlider.value += 1;
      SaveMusic_slider();
    }
    public void ControlMusic_buttonDown()
    {
      MusicSlider.value -= 1;
      SaveMusic_slider();
    }
}
