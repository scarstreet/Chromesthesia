using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PauseScript : MonoBehaviour
{
  private static string prevActiveScene;
  public static bool pauseOpen = false;
  void Start()
  {
    pauseOpen = true;
    if(SceneManager.GetActiveScene().name != "SettingsScene" && SceneManager.GetActiveScene().name!= "PauseScreen"){
      prevActiveScene = SceneManager.GetActiveScene().name;
      SceneManager.UnloadSceneAsync(prevActiveScene);
    }
  }

  private void OnDestroy() {
    pauseOpen = false;
  }

  // Update is called once per frame
  void Update()
  {
  }

  public void toSettings()
  {
    SceneManager.LoadScene("SettingsScene", LoadSceneMode.Additive);
  }
  public void continuePressed()
  {
    SceneManager.LoadScene(prevActiveScene, LoadSceneMode.Single);
  }
  public void restartPressed()
  {
    // TODO - the restart
    SceneManager.LoadScene(prevActiveScene, LoadSceneMode.Single);
  }
  public void giveUpPressed()
  {
    // TODO - clear all dem data
    SceneManager.LoadScene("SongSelect", LoadSceneMode.Single);
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
}
