using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System;

public class Score
{
  bool notPlayedYet = false;
  public int score = 0;
  public int combo = 0;
  public double accuracy = 0;
  int miss = 0, good = 0, perfect = 0;
  public string rating = "";
  public Score(string i_new, string i_score, string i_miss, string i_good, string i_perfect, string i_combo, string i_acc, string i_rating)
  {
    if (i_new.Contains("true"))
    {
      notPlayedYet = true;
    }
    score = Convert.ToInt16(i_score.Split('=')[1]);
    miss = Convert.ToInt16(i_miss.Split('=')[1]);
    good = Convert.ToInt16(i_good.Split('=')[1]);
    perfect = Convert.ToInt16(i_perfect.Split('=')[1]);
    combo = Convert.ToInt16(i_combo.Split('=')[1]);
    accuracy = Convert.ToDouble(i_acc.Split('=')[1]);
    rating = i_rating.Split('=')[1];
    if (rating.Contains('0'))
    {
      rating = "";
    }
    // Debug.Log(score + " " + miss + " " + good + " " + perfect + " " + combo + " " + accuracy + " " + rating + " " + notPlayedYet);
  }
}
public class SongInfo
{
  Sprite image;
  string title;
  string artist;
  public Score easy, normal, hard; // Highscores for each difficulty
  public string BM_easyPath, BM_normalPath, BM_hardPath;
  public string getTitle() { return title; }
  public string getArtist() { return artist; }
  public Sprite getImage() { return image; }
  public SongInfo(string folder)
  {
    Debug.Log("Songs/" + folder + "/info");
    TextAsset i = Resources.Load<TextAsset>("Songs/" + folder + "/info");
    List<string> info = new List<string>(i.text.Split('\n'));
    Debug.Log(info);

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
  public static bool justStarted = true;
  public static SongInfo currentSong;
  public SongInfo nextSong;
  public SongInfo prevSong;
  // vvv UI ELEMENTS vvv ========================================================
  public Image currentSongImage;
  public Image nextSongImage;
  public Image prevSongImage;
  public Image transitionPanel;

  public Text songTitle;
  public Text artistName;
  public Text nextSongText;
  public Text prevSongText;
  public Text easyText;
  public Text normalText;
  public Text hardText;
  public Text highscore;
  public Text combo;
  public Text accuracy;
  public Text rating;

  public void getAllSongs()
  {
    List<SongInfo> list = new List<SongInfo>();
    TextAsset s = Resources.Load<TextAsset>("songlist");
    string text = s.text;
    string[] folders = text.Split('\n');
    List<string> songFolderList = new List<string>(folders);
    foreach (string folder in songFolderList)
    {
      list.Add(new SongInfo(folder));
    }
    allSongs = list;
  }
  public void scoreUIfade(float fade)
  {
    highscore.CrossFadeAlpha(fade, 0.25f, false);
    combo.CrossFadeAlpha(fade, 0.25f, false);
    accuracy.CrossFadeAlpha(fade, 0.25f, false);
    rating.CrossFadeAlpha(fade, 0.25f, false);
  }
  public void songUIfade(float fade)
  {
    songTitle.CrossFadeAlpha(fade, 0.25f, false);
    artistName.CrossFadeAlpha(fade, 0.25f, false);
    nextSongText.CrossFadeAlpha(fade, 0.25f, false);
    prevSongText.CrossFadeAlpha(fade, 0.25f, false);
    currentSongImage.CrossFadeAlpha(fade, 0.25f, false);
    nextSongImage.CrossFadeAlpha(fade, 0.25f, false);
    prevSongImage.CrossFadeAlpha(fade, 0.25f, false);
    highscore.CrossFadeAlpha(fade, 0.25f, false);
    combo.CrossFadeAlpha(fade, 0.25f, false);
    accuracy.CrossFadeAlpha(fade, 0.25f, false);
    rating.CrossFadeAlpha(fade, 0.25f, false);
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
    if (currentDifficulty.Contains("EASY"))
    {
      easyText.color = new Color((255f / 255f), (255f / 255f), (255f / 255f), 1);
      normalText.color = new Color((100f / 255f), (100f / 255f), (100f / 255f), 1);
      hardText.color = new Color((100f / 255f), (100f / 255f), (100f / 255f), 1);
      highscore.text = currentSong.easy.score.ToString();
      combo.text = currentSong.easy.combo.ToString();
      accuracy.text = currentSong.easy.accuracy.ToString() + '%';
      rating.text = currentSong.easy.rating;
    }
    if (currentDifficulty.Contains("NORMAL"))
    {
      easyText.color = new Color((100f / 255f), (100f / 255f), (100f / 255f), 1);
      normalText.color = new Color((255f / 255f), (255f / 255f), (255f / 255f), 1);
      hardText.color = new Color((100f / 255f), (100f / 255f), (100f / 255f), 1);
      highscore.text = currentSong.normal.score.ToString();
      combo.text = currentSong.normal.combo.ToString();
      accuracy.text = currentSong.normal.accuracy.ToString() + '%';
      rating.text = currentSong.normal.rating;
    }
    if (currentDifficulty.Contains("HARD"))
    {
      easyText.color = new Color((100f / 255f), (100f / 255f), (100f / 255f), 1);
      normalText.color = new Color((100f / 255f), (100f / 255f), (100f / 255f), 1);
      hardText.color = new Color((255f / 255f), (255f / 255f), (255f / 255f), 1);
      highscore.text = currentSong.hard.score.ToString();
      combo.text = currentSong.hard.combo.ToString();
      accuracy.text = currentSong.hard.accuracy.ToString() + '%';
      rating.text = currentSong.hard.rating;
    }
  }

  public static string beatmapPath()
  {
    if (currentDifficulty.Contains("EASY"))
    {
      return currentSong.BM_easyPath;
    }
    else if (currentDifficulty.Contains("NORMAL"))
    {
      return currentSong.BM_normalPath;
    }
    else
    {
      return currentSong.BM_hardPath;
    }
  }
  /***************************************************************************
    RUNTIME FUNCTIONS BELOW THIS LINE
  ****************************************************************************/
  public void Start()
  {
    getAllSongs();
    if (justStarted)
    {
      currentSong = allSongs[0];
      justStarted = false;
    }
    int index = allSongs.FindIndex((i) => i.getTitle() == currentSong.getTitle());
    nextSong = index + 1 == allSongs.Count ? allSongs[0] : allSongs[index + 1];
    prevSong = index - 1 == -1 ? allSongs[allSongs.Count - 1] : allSongs[index - 1];
    updateUI();
    transitionPanel.CrossFadeAlpha(0, 0.5f, false);
  }

  // Update is called once per frame
  void Update()
  {
    if (Input.touchCount > 0 && IsPointerOverUIObject() == false)
    {
      Touch touch = Input.GetTouch(0);
      if (touch.phase == TouchPhase.Ended && (touch.deltaPosition != touch.rawPosition))
      {
        double rad = Mathf.Atan2(touch.deltaPosition.y, touch.deltaPosition.x);
        double angle = rad * (180 / Math.PI); //check the angle and change it to degrees from radian
        string dir = checkDir(angle);
        if (dir.Contains('a') || dir.Contains('q') || dir.Contains('z'))
        {
          nextSongPressed();
        }
        else if (dir.Contains('d') || dir.Contains('e') || dir.Contains('c'))
        {
          previousSongPressed();
        }
      }
    }
  }
  public void changeDifficulty(string selected)
  {
    currentDifficulty = selected;
    updateUI();
    StartCoroutine(changeDifficulty());
  }

  private string checkDir(double angle)
  {
    string dir = "";
    if (angle >= -22.5 && angle < 22.5) // right
      dir = "d";
    else if (angle >= 22.5 && angle < 67.5) // up right
      dir = "e";
    else if (angle >= 67.5 && angle < 112.5) // up
      dir = "w";
    else if (angle >= 112.5 && angle < 157.5) // up left
      dir = "q";
    else if ((angle >= 157.5 && angle <= 180) || (angle < -157.5 && angle >= -180)) // left
      dir = "a";
    else if (angle >= -157.5 && angle < -112.5) // down left
      dir = "z";
    else if (angle >= -112.5 && angle < -67.5) // down
      dir = "x";
    else if (angle >= -67.5 && angle < -22.5) // down right
      dir = "c";
    return dir;
  }

  public void previousSongPressed()
  {
    StartCoroutine(changeSong("prev"));
  }

  public void nextSongPressed()
  {
    StartCoroutine(changeSong("next"));
  }

  IEnumerator changeDifficulty()
  {
    scoreUIfade(0);
    bool faded = false;
    while (!faded)
    {
      faded = true;
      yield return new WaitForSeconds(.25f);
    }
    updateUI();
    songUIfade(1);
  }

  IEnumerator changeSong(string which)
  {
    songUIfade(0);
    bool faded = false;
    while (!faded)
    {
      faded = true;
      yield return new WaitForSeconds(.25f);
    }
    if (which.Contains("next"))
    {
      currentSong = nextSong;
    }
    else
    {
      currentSong = prevSong;
    }
    int index = allSongs.FindIndex(song => song == currentSong);
    nextSong = index + 1 == allSongs.Count ? allSongs[0] : allSongs[index + 1];
    prevSong = index - 1 == -1 ? allSongs[allSongs.Count - 1] : allSongs[index - 1];
    updateUI();
    songUIfade(1);
  }
  public void playPressed()
  {
    GameScript.resetStates();
    StartCoroutine(changeScene("GameScreen"));
  }

  public void toSettings()
  {
    StartCoroutine(changeScene("SettingsScene"));
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
      transitionPanel.CrossFadeAlpha(1, 0.5f, false);
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
