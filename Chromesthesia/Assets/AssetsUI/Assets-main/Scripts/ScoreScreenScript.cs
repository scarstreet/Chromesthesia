using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ScoreScreenScript : MonoBehaviour
{
  public Image transitionPanel;
  public Text difficulty;
  public Text songTitle;
  public Text artistName;
  void Start()
  {
    transitionPanel.CrossFadeAlpha(0, 0.25f, false);
    difficulty.text = SongSelectScript.currentDifficulty;
    songTitle.text = SongSelectScript.currentSong.getTitle();
    artistName.text = SongSelectScript.currentSong.getArtist();
  }

  // Update is called once per frame
  void Update()
  {
  }
  public void toSettings()
  {
    StartCoroutine(changeScene("SettingsScene"));
  }

  public void toSongList() {
    StartCoroutine(changeScene("SongSelect"));
  }

  public void playAgainPressed(){
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
      transitionPanel.CrossFadeAlpha(1, 0.25f, false);
      fadeDone = true;
      yield return new WaitForSeconds(.25f);
    }
    if (scene.Contains("SettingsScene"))
    {
      SceneManager.LoadScene("SettingsScene", LoadSceneMode.Additive);
    }
    SceneManager.LoadScene(scene, LoadSceneMode.Single);
  }
}
