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
  public static float elapsed = 0;
  public GameObject countdown;
  public Image transitionPanel;
  public Text difficulty;
  public Text song;
  Animator animator;
  static double gameStartTime;
  static double duration = 5; // SONG DURATION IN SECONDS
  public static void resetStates()
  {
    gameIsPaused = false;
    gameCompleted = false;
    gameStarted = false;
    songProgress = 0;
    elapsed = 0;
    duration = 5;
  }
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
      songProgress = elapsed + ((float)(Time.timeAsDouble - gameStartTime) / (float)(duration));
      if (songProgress >= 1 && !gameCompleted)
      {
        gameCompleted = true;
        StartCoroutine(toScoreScreen());
      }
      animator.SetFloat("RunnerProgress", songProgress);
    }
  }

  public void pauseGame()
  {
    // TODO - pause the game
    elapsed = songProgress;
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
        if (gameIsPaused)
        {
          gameStartTime = Time.timeAsDouble + 5;
          duration = (1 - elapsed) * duration;
          songProgress = elapsed + ((float)(Time.timeAsDouble - gameStartTime + 5) / (float)(duration));
          animator.SetFloat("RunnerProgress", songProgress);
        }
        yield return new WaitForSeconds(.5f);
      }
      else if (i == 1)
      {
        GameObject cd = Instantiate(countdown, new Vector3(0, 0, 0), Quaternion.identity);
        cd.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform, false);
        yield return new WaitForSeconds(4.5f);
      }
      else
      {
        if (!gameStarted)
        {
          animator.SetFloat("RunnerProgress", 0);
          gameStartTime = Time.timeAsDouble;
          gameStarted = true;
          double animTime = 5;
          animator.speed = (float)(animTime / duration);
        }
        else if (gameIsPaused)
        {
          gameIsPaused = false;
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
