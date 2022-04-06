using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PauseScript : MonoBehaviour
{
  private static string prevActiveScene;
  public static bool pauseOpen = false;
  public Image transitionPanel;
  public Text difficulty;
  public Text song;
  void Start()
  {
    pauseOpen = true;
    if (SceneManager.GetActiveScene().name != "SettingsScene" && SceneManager.GetActiveScene().name != "PauseScreen")
    {
      prevActiveScene = SceneManager.GetActiveScene().name;
      SceneManager.UnloadSceneAsync(prevActiveScene);
    }
    transitionPanel.CrossFadeAlpha(0, 0f, false);
    difficulty.text = SongSelectScript.currentDifficulty;
    song.text = SongSelectScript.currentSong.getTitle() + " - " + SongSelectScript.currentSong.getArtist();
  }

  private void OnDestroy()
  {
    pauseOpen = false;
  }

  // Update is called once per frame
  void Update()
  {
  }

  public void toSettings()
  {
    StartCoroutine(changeScene("SettingsScene"));
  }
  public void continuePressed()
  {
    StartCoroutine(changeScene("GameScreen"));
  }
  public void restartPressed()
  {
    // TODO - the restart
    GameScript.resetStates();
    StartCoroutine(changeScene("GameScreen"));
  }
  public void giveUpPressed()
  {
    // TODO - clear all dem data
    StartCoroutine(changeScene("SongSelect"));
    // SceneManager.LoadScene("SongSelect", LoadSceneMode.Single);
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
