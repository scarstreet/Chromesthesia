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
  Score easy, normal, hard; // Highscores for each difficulty
  SongInfo(){}
  SongInfo(string filename) {
    // open file and do shit
  }
}

public class SongSelectScript : MonoBehaviour
{
  public static string currentDifficulty = "EASY"; //"EASY","NORMAL" or "HARD"
  private List<SongInfo> allSongs;
  public static SongInfo currentSong;
  public static SongInfo nextSong;
  public static SongInfo prevSong;
  // Start is called before the first frame update
  public void Start()
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
  public void previousSongPressed()
  {
    Debug.Log("prevsing");
    currentSong = prevSong;
    int index = allSongs.FindIndex(song => song == currentSong);
    nextSong = index + 1 == allSongs.Count ? allSongs[0] : allSongs[index + 1];
    prevSong = index - 1 == -1 ? allSongs[allSongs.Count-1] : allSongs[index - 1];
  }

  public void nextSongPressed()
  {
    Debug.Log("nextsing");
    currentSong = nextSong;
    int index = allSongs.FindIndex(song => song == currentSong);
    nextSong = index + 1 == allSongs.Count ? allSongs[0] : allSongs[index + 1];
    prevSong = index - 1 == -1 ? allSongs[allSongs.Count - 1] : allSongs[index - 1];
  }

  public void playPressed()
  {
    SceneManager.LoadScene("GameScreen", LoadSceneMode.Single);
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
