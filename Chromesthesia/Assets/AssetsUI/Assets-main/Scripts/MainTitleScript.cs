using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainTitleScript : MonoBehaviour
{
  public Image transitionPanel;
  // Start is called before the first frame update
  void Start()
  {
    EventSystem sceneEventSystem = FindObjectOfType<EventSystem>();
    if (sceneEventSystem == null)
    {
      GameObject eventSystem = new GameObject("EventSystem");
      eventSystem.AddComponent<EventSystem>();
      eventSystem.AddComponent<StandaloneInputModule>();
    }
    Application.targetFrameRate = (Screen.currentResolution.refreshRate<60)?60:Screen.currentResolution.refreshRate;
    // Debug.Log(Application.targetFrameRate);
    transitionPanel.CrossFadeAlpha(0, 0.5f, false);
    string folderpath = Application.persistentDataPath;
    string settingspath = Application.persistentDataPath + "/settings.txt";
    if (!File.Exists(settingspath))
    {
        TextAsset getTemplate = Resources.Load<TextAsset>("Template/settings");
        string toWrite = getTemplate.text;
        Directory.CreateDirectory(folderpath);
        File.WriteAllText(settingspath, toWrite);
        SettingsScript.Speed = 5;
        SettingsScript.SFX = 50;
        SettingsScript.Music = 50;
    }
    else
    {
      SettingsScript.loadSettings();
    }
  }

  // Update is called once per frame
  void Update()
  {
    if (Input.GetMouseButtonDown(0) && !IsPointerOverUIObject())
    {
      StartCoroutine(changeScene("SongSelect"));
      // SceneManager.LoadScene("SongSelect", LoadSceneMode.Single);
    }
  }

  public void toSettings()
  {
    SettingsScript.prevActiveScene = "MainTitleScreen";
    StartCoroutine(changeScene("SettingsScene"));
  }
  public void toTutorial()
  {
    TutorialScript.prevScene = "MainTitleScreen";
    StartCoroutine(changeScene("TutorialScreen"));
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
    if (scene.Contains("SettingsScene"))
    {
      SceneManager.LoadScene("SettingsScene", LoadSceneMode.Additive);
    }
    SceneManager.LoadScene(scene, LoadSceneMode.Single);
  }
}
