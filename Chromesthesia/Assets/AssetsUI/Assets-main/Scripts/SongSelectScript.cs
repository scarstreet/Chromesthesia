using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Score{
  public int score;
  public float accuracy;
}
public class SongInfo{
  string title;
  string artist;
  Score easy, normal, hard;
  SongInfo(){}
  SongInfo(string filename) {
    // open file and do shit
  }
}

public class SongSelectScript : MonoBehaviour
{
  public string currentDifficulty = "EASY"; //"EASY","NORMAL" or "HARD"
  public SongInfo currentSong;
  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {
  }
  public void changeDifficulty(string selected)
  {
    currentDifficulty = selected;
  }
  public void previousSong()
  {
  }

  public void nextSong()
  {
  }

  public void playClicked()
  {
    SceneManager.LoadSceneAsync("GameScreen", LoadSceneMode.Single);
  }

  public void toSettings()
  {
    SceneManager.LoadScene("SettingsScene", LoadSceneMode.Additive);
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
