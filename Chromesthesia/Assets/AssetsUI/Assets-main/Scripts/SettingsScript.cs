using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEditor;

public class SettingsScript : MonoBehaviour
{
  // Start is called before the first frame update
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
      transitionPanel.CrossFadeAlpha(1, 0.5f, false);
      fadeDone = true;
      yield return new WaitForSeconds(.25f);
    }
    SceneManager.LoadScene(scene, LoadSceneMode.Single);
  }
}
