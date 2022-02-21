using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEditor;

public class SettingsScript : MonoBehaviour
{
  // Start is called before the first frame update
  private static string prevActiveScene;
  public static bool settingsOpen = false;
  void Start()
  {
    settingsOpen = true;
    prevActiveScene = SceneManager.GetActiveScene().name;
    Debug.Log(prevActiveScene);
    SceneManager.UnloadSceneAsync(prevActiveScene);
  }

  private void OnDestroy() {
    settingsOpen = false;
  }

  // Update is called once per frame
  void Update()
  {}

  public void toBack(){
    SceneManager.LoadScene(prevActiveScene, LoadSceneMode.Single);
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
