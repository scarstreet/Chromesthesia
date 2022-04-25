using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ScoreScreenScript : MonoBehaviour
{
  public Text highScore;
  public Text combo;
  public Text score;
  public Text accuracy;
  public Text perfect;
  public Text good;
  public Text miss;
  public Text difficulty;
  public Text rating;
  public Text songTitle;
  public Text artistName;
  public Image transitionPanel;
  public Button playAgainBtn;
  public Button songListBtn;
  public Button settingsBtn;
  Animator animator;
  void Start()
  {
    EventSystem sceneEventSystem = FindObjectOfType<EventSystem>();
    if (sceneEventSystem == null)
    {
      GameObject eventSystem = new GameObject("EventSystem");
      eventSystem.AddComponent<EventSystem>();
      eventSystem.AddComponent<StandaloneInputModule>();
    }
    SongSelectScript.currentSong.setScore(new Score((int)GameScript.score, GameScript.misscount, GameScript.goodcount, GameScript.perfectcount, GameScript.combo, Math.Round(GameScript.accuracy,2),SongSelectScript.getRating()));
    //====================================================
    score.text = ((int)GameScript.score).ToString();
    combo.text = GameScript.maxcombo.ToString();
    perfect.text = GameScript.perfectcount.ToString();
    good.text = GameScript.goodcount.ToString();
    miss.text = (GameScript.count-GameScript.perfectcount-GameScript.goodcount).ToString();
    accuracy.text = (Math.Round(GameScript.accuracy,2)).ToString() + "%";
    rating.text = SongSelectScript.getRating();
    //====================================================
    SongSelectScript.currentSong.saveScore();
    GameScript.resetStates();
    transitionPanel.CrossFadeAlpha(0, 0.5f, false);
    animator = gameObject.GetComponent<Animator>();
    difficulty.text = SongSelectScript.currentDifficulty;
    songTitle.text = SongSelectScript.currentSong.getTitle();
    artistName.text = SongSelectScript.currentSong.getArtist();
    playAgainBtn.enabled = false;
    songListBtn.enabled = false;
    settingsBtn.enabled = false;
  }
  public void stopAnimations()
  {
    animator.speed = 0;
    playAgainBtn.enabled = true;
    songListBtn.enabled = true;
    settingsBtn.enabled = true;
  }

  public void toSettings()
  {
    SettingsScript.prevActiveScene = "ScoreScreen";
    StartCoroutine(changeScene("SettingsScene"));
  }

  public void toSongList()
  {
    StartCoroutine(changeScene("SongSelect"));
  }

  public void playAgainPressed()
  {
    StartCoroutine(changeScene("GameScreen"));
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
