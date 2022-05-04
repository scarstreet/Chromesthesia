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
  // public GameScript game;
  void Start()
  {
    pauseOpen = true;
    EventSystem sceneEventSystem = FindObjectOfType<EventSystem>();
    if (sceneEventSystem == null)
    {
      GameObject eventSystem = new GameObject("EventSystem");
      eventSystem.AddComponent<EventSystem>();
      eventSystem.AddComponent<StandaloneInputModule>();
    }
    if (SceneManager.GetActiveScene().name != "SettingsScene" && SceneManager.GetActiveScene().name != "PauseScreen")
    {
      prevActiveScene = SceneManager.GetActiveScene().name;
      SceneManager.SetActiveScene(SceneManager.GetSceneByName("PauseScreen"));
      // SceneManager.UnloadSceneAsync(prevActiveScene);
    }
    transitionPanel.CrossFadeAlpha(0, 0f, false);
    difficulty.text = SongSelectScript.currentDifficulty;
    song.text = SongSelectScript.currentSong.getTitle() + " - " + SongSelectScript.currentSong.getArtist();
  }

  private void OnDestroy()
  {
  }

  public void toSettings()
  {
    SettingsScript.prevActiveScene = "PauseScreen";
    StartCoroutine(changeScene("SettingsScene"));
  }
  public void toTutorial()
  {
    TutorialScript.prevScene = "PauseScreen";
    StartCoroutine(changeScene("TutorialScreen"));
  }
  public void continuePressed()
  {
    pauseOpen = false;
    StartCoroutine(changeScene("GameScreen"));
  }
  public void restartPressed()
  {
    pauseOpen = false;
    GameScript.self.GetComponent<GameScript>().changeParticleColour(new Color(1f, 1f, 1f));
    GameScript.resetStates();
    StartCoroutine(changeScene("GameScreen"));
  }
  public void giveUpPressed()
  {
    // TODO - clear all dem data
    pauseOpen = false;
    GameScript.self.GetComponent<GameScript>().changeParticleColour(new Color(1f, 1f, 1f));
    GameScript.resetStates();
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
    else if (scene.Contains("GameScreen") && GameScript.gameStarted)
    {
      // SceneManager.SetActiveScene(SceneManager.GetSceneByName("GameScreen"));
      GameScript.self.GetComponent<GameScript>().pauseDone();
      SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("PauseScreen"));
    }
    else 
    {
      SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }
  }
}
