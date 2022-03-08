using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System;

public class Score{
  bool notPlayedYet = false;
  int score = 0;
  int combo = 0;
  double accuracy = 0;
  int miss = 0,good = 0,perfect = 0;
  string rating = "";
  public Score(string i_new,string i_score,string i_miss,string i_good,string i_perfect, string i_combo, string i_acc, string i_rating){
    if(i_new.Contains("true")){
      notPlayedYet = true;
    }
    score = Convert.ToInt16(i_score.Split('=')[1]);
    miss = Convert.ToInt16(i_miss.Split('=')[1]);
    good = Convert.ToInt16(i_good.Split('=')[1]);
    perfect = Convert.ToInt16(i_perfect.Split('=')[1]);
    combo = Convert.ToInt16(i_combo.Split('=')[1]);
    accuracy = Convert.ToDouble(i_acc.Split('=')[1]);
    rating = i_rating.Split('=')[1];
    Debug.Log(score + " " + miss + " " + good + " " + perfect + " " + combo + " " + accuracy + " " + rating + " " + notPlayedYet);
  }
}
public class SongInfo{
  Sprite image;
  string title;
  string artist;
  Score easy, normal, hard; // Highscores for each difficulty
  string BM_easyPath, BM_normalPath, BM_hardPath;
  public string getTitle() { return title; }
  public string getArtist() { return artist; }
  public Sprite getImage() { return image; }
  public SongInfo(string folder) {
    TextAsset i = Resources.Load<TextAsset>("Songs/"+folder+"/info");
    List<string> info = new List<string>(i.text.Split('\n'));

    // BASIC INFO =================================================================
    title = info.Find(i => i.Contains("song-name")).Split("=")[1];
    artist = info.Find(i => i.Contains("artist")).Split("=")[1];

    // SCORES =====================================================================
    int e_id = info.FindIndex(i => i.Contains("> Easy")); // Easy index
    easy = new Score(info[e_id + 1], info[e_id + 2], info[e_id + 3], info[e_id + 4], info[e_id + 5], info[e_id + 6], info[e_id + 7], info[e_id + 8]);

    int n_id = info.FindIndex(i => i.Contains("> Normal")); // Normal index
    normal = new Score(info[n_id + 1], info[n_id + 2], info[n_id + 3], info[n_id + 4], info[n_id + 5], info[n_id + 6], info[n_id + 7], info[n_id + 8]);

    int h_id = info.FindIndex(i => i.Contains("> Hard")); // Hard index
    hard = new Score(info[h_id + 1], info[h_id + 2], info[h_id + 3], info[h_id + 4], info[h_id + 5], info[h_id + 6], info[h_id + 7], info[h_id + 8]);

    // FILES ======================================================================
    BM_easyPath = info.Find(i => i.Contains("> beatmap-easy")).Split("=")[1];
    BM_easyPath = "Songs/" + folder + "/" + BM_easyPath;
    BM_normalPath = info.Find(i => i.Contains("> beatmap-normal")).Split("=")[1];
    BM_normalPath = "Songs/" + folder + "/" + BM_normalPath;
    BM_hardPath = info.Find(i => i.Contains("> beatmap-hard")).Split("=")[1];
    BM_hardPath = "Songs/" + folder + "/" + BM_hardPath;

    // IMAGE =====================================================================
    string imagePath = info.Find(i => i.Contains("> image")).Split("=")[1];
    imagePath = "Songs/" + folder + "/" + imagePath;
    image = Resources.Load<Sprite>(imagePath);
  }
}

public class SongSelectScript : MonoBehaviour
{
  public static string currentDifficulty = "EASY"; //"EASY","NORMAL" or "HARD"
  private List<SongInfo> allSongs;
  public static SongInfo currentSong;
  public static SongInfo nextSong;
  public static SongInfo prevSong;
  // vvv UI ELEMENTS vvv ========================================================
  public Image currentSongImage;
  public Image nextSongImage;
  public Image prevSongImage;

  public Text songTitle;
  public Text artistName;
  public Text nextSongText;
  public Text prevSongText;
  public void getAllSongs()
  {
    List<SongInfo> list = new List<SongInfo>();
    TextAsset s = Resources.Load<TextAsset>("songlist");
    string text = s.text;
    string[] folders = text.Split('\n');
    List<string> songFolderList = new List<string>(folders);
    foreach(string folder in songFolderList){
      list.Add(new SongInfo(folder));
    }
    allSongs = list;
  }
  public void updateUI()
  {
    songTitle.text = currentSong.getTitle();
    artistName.text = currentSong.getArtist();
    currentSongImage.sprite = currentSong.getImage();
    nextSongImage.sprite = nextSong.getImage();
    prevSongImage.sprite = prevSong.getImage();
    nextSongText.text = nextSong.getTitle() + " - " + nextSong.getArtist();
    prevSongText.text = prevSong.getTitle() + " - " + prevSong.getArtist();
  }
  /***************************************************************************
    RUNTIME FUNCTIONS BELOW THIS LINE
  ****************************************************************************/
  public void Start()
  {
    getAllSongs();
    currentSong = allSongs[0];
    nextSong = 1 >= allSongs.Count ? allSongs[allSongs.Count - 1] : allSongs[1];
    prevSong = allSongs[allSongs.Count - 1];
    updateUI();
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
    updateUI();
  }

  public void nextSongPressed()
  {
    Debug.Log("nextsing");
    currentSong = nextSong;
    int index = allSongs.FindIndex(song => song == currentSong);
    nextSong = index + 1 == allSongs.Count ? allSongs[0] : allSongs[index + 1];
    prevSong = index - 1 == -1 ? allSongs[allSongs.Count - 1] : allSongs[index - 1];
    updateUI();
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
