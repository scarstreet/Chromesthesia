using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.IO;

public class ScoreScreenScript : MonoBehaviour
{
  public Text ddebug;
  public Text highScore;
  public Text combo;
  public Text score;
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
    // Debug.Log("QWQ " + Application.persistentDataPath);
    // Debug.Log("QAQ " + Application.dataPath);
    // Debug.Log("QAAQ " + Application.dataPath+"/HighScores/"+ SongSelectScript.currentSong.getTitle()+"/info.txt");
    SongSelectScript.currentSong.setScore(new Score((int)GameScript.score, GameScript.misscount, GameScript.goodcount, GameScript.perfectcount, GameScript.combo, GameScript.accuracy,SongSelectScript.getRating()));
    //====================================================
    score.text = GameScript.score.ToString();
    combo.text = GameScript.maxcombo.ToString();
    perfect.text = GameScript.perfectcount.ToString();
    good.text = GameScript.goodcount.ToString();
    miss.text = (GameScript.count-GameScript.perfectcount-GameScript.goodcount).ToString();
    rating.text = SongSelectScript.getRating();
    //====================================================
    SongSelectScript.currentSong.saveScore();
    // ddebug.text = Application.persistentDataPath;
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

  // Update is called once per frame
  void Update()
  {

  }

  public void toSettings()
  {
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
