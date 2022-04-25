using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

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
  public static string prevActiveScene;
  public static bool settingsOpen = false;
  public Image transitionPanel;
  public static float Speed;
  public static float SFX;
  public static float Music;
  public static void setSpeed(float speed) { Speed = speed; }
  public static void setSFX(float sfx) { SFX = sfx; }
  public static void setMusic(float music) { Music = music; }
  public static float getSpeed() { return Speed; }
  public static float getSFX() { return SFX; }
  public static float getMusic() { return Music; }
  void Start()
  {
    transitionPanel.CrossFadeAlpha(0, 0.5f, true);
    settingsOpen = true;
    // prevActiveScene = SceneManager.GetActiveScene().name;
    // if (prevActiveScene == null)
    // {
    //   prevActiveScene = "MainTitleScreen";
    // }
    // if (prevActiveScene.Contains("SettingsScene"))
    // {
    //   // TODO - MAKE A BETTER WAY TO STORE PREVIOUS SCREENS!!!
    //   prevActiveScene = "MainTitleScreen";
    // }
    // Debug.Log("settings prevscene is " + prevActiveScene);
    SceneManager.SetActiveScene(SceneManager.GetSceneByName("SettingsScene"));
    try
    {
      SceneManager.UnloadSceneAsync(prevActiveScene);
    }
    catch
    {
      Debug.Log("Can't unload scene");
    }

    loadSettings();
    SpeedSlider.value = Speed;
    SFXSlider.value = SFX;
    MusicSlider.value = Music;
  }
  private void OnDestroy()
  {
    settingsOpen = false;
  }

  public void toBack()
  {
    Debug.Log("Going back to " + prevActiveScene);
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
    Debug.Log("Going back to " + scene);
    if (scene == "PauseScreen")
    {
      //   SceneManager.SetActiveScene(SceneManager.GetSceneByName(scene));
      SceneManager.LoadScene(scene, LoadSceneMode.Additive);
      SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("SettingsScene"));
    }
    else
    {
      SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }
  }

  public void ResetSettings()
  {
    SpeedSlider.value = 5;
    SFXSlider.value = 100;
    MusicSlider.value = 100;
    Speed = SpeedSlider.value;
    SFX = SFXSlider.value;
    Music = MusicSlider.value;
    saveSettings();
  }
  // ======================================================================================

  public void saveSettings()
  {
    string folderpath = Application.persistentDataPath;
    string settingspath = Application.persistentDataPath + "/settings.txt";
    string info = File.ReadAllText(settingspath);
    List<string> i = new List<string>(info.Split("\n"));
    Speed = Mathf.Round(SpeedSlider.value * 1f);
    SFX = Mathf.Round(SFXSlider.value * 1f);
    Music = Mathf.Round(MusicSlider.value * 1f);
    i[0] = $"Speed={Speed}";
    i[1] = $"SFX={SFX}";
    i[2] = $"Music={Music}";
    string newFile = string.Join("\n", i);
    Debug.Log(newFile);
    File.WriteAllText(settingspath, newFile);
  }
  public static void loadSettings()
  {
    string folderpath = Application.persistentDataPath;
    string settingspath = Application.persistentDataPath + "/settings.txt";
    string info = File.ReadAllText(settingspath);
    List<string> i = new List<string>(info.Split("\n"));
    Speed = float.Parse(i[0].Split('=')[1]);
    SFX = float.Parse(i[1].Split('=')[1]);
    Music = float.Parse(i[2].Split('=')[1]);
  }

  public void ControlSpeed_slider(float speed)
  {
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
    PlayerPrefs.SetFloat("SFXVolume", SpeedSlider.value);
    string folderpath = Application.persistentDataPath;
    string settingspath = Application.persistentDataPath + "/settings.txt";
    string info = File.ReadAllText(settingspath);
    List<string> i = new List<string>(info.Split("\n"));
    Speed = Mathf.Round(SpeedSlider.value * 1f);
    i[0] = $"Speed={Speed}";
    string newFile = string.Join("\n", i);
    Debug.Log(newFile);
    File.WriteAllText(settingspath, newFile);
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
    string folderpath = Application.persistentDataPath;
    string settingspath = Application.persistentDataPath + "/settings.txt";
    string info = File.ReadAllText(settingspath);
    List<string> i = new List<string>(info.Split("\n"));
    SFX = Mathf.Round(SFXSlider.value * 1f);
    i[1] = $"SFX={SFX}";
    string newFile = string.Join("\n", i);
    Debug.Log(newFile);
    File.WriteAllText(settingspath, newFile);
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
    string folderpath = Application.persistentDataPath;
    string settingspath = Application.persistentDataPath + "/settings.txt";
    string info = File.ReadAllText(settingspath);
    List<string> i = new List<string>(info.Split("\n"));
    Music = Mathf.Round(MusicSlider.value * 1f);
    i[2] = $"Music={Music}";
    string newFile = string.Join("\n", i);
    Debug.Log(newFile);
    File.WriteAllText(settingspath, newFile);
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
