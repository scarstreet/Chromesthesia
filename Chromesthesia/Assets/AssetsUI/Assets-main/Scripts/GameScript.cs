using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameScript : MonoBehaviour
{
  public static bool gameIsPaused = false;
  public static bool gameCompleted = false;
  public static bool gameStarted = false;
  public static float songProgress = 0;
  public Image transitionPanel;
  public Text difficulty;
  public Text song;
  Animator animator;
  static double gameStartTime;
  double duration = 10; // SONG DURATION IN SECONDS
  void Start()
  {
    difficulty.text = SongSelectScript.currentDifficulty;
    song.text = SongSelectScript.currentSong.getTitle() + " - " + SongSelectScript.currentSong.getArtist();
    animator = gameObject.GetComponent<Animator>();
    animator.speed = 0;
    StartCoroutine(postStart());
  }

  // Update is called once per frame
  void Update()
  {
    if (gameStarted && !gameIsPaused)
    {
      songProgress = (float)(Time.timeAsDouble - gameStartTime) / (float)(gameStartTime + duration);
      // Debug.Log(songProgress);
      animator.SetFloat("RunnerProgress", songProgress);
      if (songProgress >= 1 && !gameCompleted)
      {
        gameCompleted = true;
        Debug.Log("FINISHED AT = " + songProgress);
        StartCoroutine(toScoreScreen());
      }
    }
  }

  public void pauseGame()
  {
    // TODO - pause the game
    gameIsPaused = true;
    animator.speed = 0;
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
  IEnumerator postStart()
  {
    for (int i = 0; i < 3; i++)
    {
      if (i == 0)
      {
        transitionPanel.CrossFadeAlpha(0, 0.5f, false);
        yield return new WaitForSeconds(.5f);
      }
      else if (i == 1)
      {
        // TODO - 321 count down
        yield return new WaitForSeconds(0f);
      }
      else
      {
        if (!gameStarted)
        {
          animator.SetFloat("RunnerProgress", 0);
          gameStartTime = Time.timeAsDouble;
          Debug.Log("START TIME  = " + gameStartTime);
          Debug.Log("Progress start  = " + ((Time.timeAsDouble - gameStartTime) / (gameStartTime + duration)));
          gameStarted = true;
          double animTime = 5;
          animator.speed = (float)(animTime / duration);
        } else if(gameIsPaused) {
          // TODO - take care of pause progress
        }
      }
      yield return new WaitForSeconds(0f);
    }
  }
  IEnumerator toScoreScreen()
  {
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
