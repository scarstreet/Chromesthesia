using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameScript : MonoBehaviour
{
  public static bool gameIsPaused;
  public static bool gameCompleted;
  public Image transitionPanel;
  public Image progressBar;
  public Text difficulty;
  public Text song;
  double gameStartTime;
  int totalWidth;
  public static bool songStarted = false;
  void Start()
  {
    gameIsPaused = false;
    gameCompleted = false;
    transitionPanel.CrossFadeAlpha(0, 0.25f, false);
    totalWidth = Screen.width;
    songStarted = true;
    progressBar.transform.localScale = new Vector3(0, 0, 0);
    difficulty.text = SongSelectScript.currentDifficulty;
    song.text = SongSelectScript.currentSong.getTitle() + " - " + SongSelectScript.currentSong.getArtist();
    gameStartTime = Time.timeAsDouble;
  }

  // Update is called once per frame
  void Update()
  {
    double progress = Time.timeAsDouble / (gameStartTime + 5);
    progressBar.transform.localScale = new Vector3((float)progress, 1, 0);
    if ((Time.timeAsDouble >= gameStartTime + 5) && !gameCompleted)
    {
      gameCompleted = true;
      StartCoroutine(toScoreScreen());
    }
  }

  public void pauseGame()
  {
    // TODO - pause the game
    gameIsPaused = true;
    SceneManager.LoadScene("PauseScreen", LoadSceneMode.Additive);
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
  IEnumerator toScoreScreen() {
    bool fadeDone = false;
    while (!fadeDone)
    {
      transitionPanel.CrossFadeAlpha(1, 0.5f, false);
      fadeDone = true;
      yield return new WaitForSeconds(.5f);
    }
    SceneManager.LoadScene("ScoreScreen", LoadSceneMode.Single);
  }
}
