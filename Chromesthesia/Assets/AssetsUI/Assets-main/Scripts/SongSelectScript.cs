using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Score
{
  public bool notPlayedYet = true;
  public int score = 0;
  public int combo = 0;
  public double accuracy = 0;
  public int miss = 0, good = 0, perfect = 0;
  public string rating = "";
  public Score(int i_score, int i_miss, int i_good, int i_perfect, int i_combo, double i_acc, string i_rating)
  {
    notPlayedYet = false;
    score = i_score;
    miss = i_miss;
    good = i_good;
    perfect = i_perfect;
    combo = i_combo;
    accuracy = i_acc;
    rating = i_rating;
  }
  public Score(string i_new, string i_score, string i_miss, string i_good, string i_perfect, string i_combo, string i_acc, string i_rating)
  {
    if (i_new.Contains("true"))
    {
      notPlayedYet = true;
    }
    score = Convert.ToInt32(i_score.Split('=')[1]);
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
    // //Debug.Log(score + " " + miss + " " + good + " " + perfect + " " + combo + " " + accuracy + " " + rating + " " + notPlayedYet);
  }
}
public class SongInfo
{
  Sprite image;
  AudioClip audio;
  string title;
  string artist;
  public double duration;
  public Score easy, normal, hard; // Highscores for each difficulty
  public string BM_easyPath, BM_normalPath, BM_hardPath;
  public string getTitle() { return title; }
  public string getArtist() { return artist; }
  public double pvStart, pvEnd;
  public Color pvColor;
  public Sprite getImage() { return image; }
  public AudioClip getAudio() { return audio; }
  public SongInfo(string folder)
  {
    TextAsset i = Resources.Load<TextAsset>("Songs/" + folder + "/info");
    List<string> info = new List<string>(i.text.Split('\n'));

    // BASIC INFO =================================================================
    title = info.Find(i => i.Contains("song-name")).Split("=")[1];
    artist = info.Find(i => i.Contains("artist")).Split("=")[1];
    duration = Convert.ToDouble(info.Find(i => i.Contains("duration")).Split("=")[1]);

    // CHECK HIGHSCORE ============================================================
    string folderpath = Application.persistentDataPath;
    string songinfopath = Application.persistentDataPath + "/" + title + ".txt";
    if (!File.Exists(songinfopath))
    {
      TextAsset getTemplate = Resources.Load<TextAsset>("Template/info");
      string toWrite = getTemplate.text;
      Directory.CreateDirectory(folderpath);
      File.WriteAllText(songinfopath, toWrite);
    }
    string Anotherinfo = File.ReadAllText(songinfopath);
    List<string> information = new List<string>(Anotherinfo.Split('\n'));

    // SCORES =====================================================================
    int e_id = information.FindIndex(i => i.Contains("> Easy")); // Easy index
    easy = new Score(information[e_id + 1], information[e_id + 2], information[e_id + 3], information[e_id + 4], information[e_id + 5], information[e_id + 6], information[e_id + 7], information[e_id + 8]);

    int n_id = information.FindIndex(i => i.Contains("> Normal")); // Normal index
    normal = new Score(information[n_id + 1], information[n_id + 2], information[n_id + 3], information[n_id + 4], information[n_id + 5], information[n_id + 6], information[n_id + 7], information[n_id + 8]);

    int h_id = information.FindIndex(i => i.Contains("> Hard")); // Hard index
    hard = new Score(information[h_id + 1], information[h_id + 2], information[h_id + 3], information[h_id + 4], information[h_id + 5], information[h_id + 6], information[h_id + 7], information[h_id + 8]);

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

    // AUDIO =====================================================================
    string audioPath = info.Find(i => i.Contains("> audio")).Split("=")[1];
    audioPath = "Songs/" + folder + "/" + audioPath;
    audio = Resources.Load<AudioClip>(audioPath);

    // PREVIEW INFO ==============================================================
    pvStart = Convert.ToDouble(info.Find(i => i.Contains("song-start")).Split("=")[1]);
    pvEnd = Convert.ToDouble(info.Find(i => i.Contains("song-end")).Split("=")[1]);
    string color = info.Find(i => i.Contains("color=")).Split("=")[1];
    ColorUtility.TryParseHtmlString(color, out pvColor);
  }
  public void setScore(Score newscore) //only if highscore
  {
    if (SongSelectScript.currentDifficulty.Contains("EASY") && easy.score < newscore.score)
      easy = newscore;
    else if (SongSelectScript.currentDifficulty.Contains("NORMAL") && normal.score < newscore.score)
      normal = newscore;
    else if (SongSelectScript.currentDifficulty.Contains("HARD") && hard.score < newscore.score)
      hard = newscore;
  }
  public void saveScore()
  {
    string songinfopath = Application.persistentDataPath + "/" + title + ".txt";
    string info = File.ReadAllText(songinfopath);
    List<string> i = new List<string>(info.Split('\n'));
    if (SongSelectScript.currentDifficulty.Contains("EASY"))
    {
      int index = i.FindIndex(ii => ii.Contains("> Easy"));
      int i2 = easy.score;
      int i3 = easy.miss;
      int i4 = easy.good;
      int i5 = easy.perfect;
      int i6 = easy.combo;
      double i7 = easy.accuracy;
      string i8 = easy.rating;
      i[index + 1] = $"new=false";
      i[index + 2] = $"score={i2}";
      i[index + 3] = $"miss={i3}";
      i[index + 4] = $"good={i4}";
      i[index + 5] = $"perfect={i5}";
      i[index + 6] = $"combo={i6}";
      i[index + 7] = $"accuracy={i7}";
      i[index + 8] = $"rating={i8}";
    }
    else if (SongSelectScript.currentDifficulty.Contains("NORMAL"))
    {
      int index = i.FindIndex(ii => ii.Contains("> Normal"));
      int i2 = normal.score;
      int i3 = normal.miss;
      int i4 = normal.good;
      int i5 = normal.perfect;
      int i6 = normal.combo;
      double i7 = normal.accuracy;
      string i8 = normal.rating;
      i[index + 1] = $"new=false";
      i[index + 2] = $"score={i2}";
      i[index + 3] = $"miss={i3}";
      i[index + 4] = $"good={i4}";
      i[index + 5] = $"perfect={i5}";
      i[index + 6] = $"combo={i6}";
      i[index + 7] = $"accuracy={i7}";
      i[index + 8] = $"rating={i8}";
    }
    else if (SongSelectScript.currentDifficulty.Contains("HARD"))
    {
      int index = i.FindIndex(ii => ii.Contains("> Hard"));
      int i2 = hard.score;
      int i3 = hard.miss;
      int i4 = hard.good;
      int i5 = hard.perfect;
      int i6 = hard.combo;
      double i7 = hard.accuracy;
      string i8 = hard.rating;
      i[index + 1] = $"new=false";
      i[index + 2] = $"score={i2}";
      i[index + 3] = $"miss={i3}";
      i[index + 4] = $"good={i4}";
      i[index + 5] = $"perfect={i5}";
      i[index + 6] = $"combo={i6}";
      i[index + 7] = $"accuracy={i7}";
      i[index + 8] = $"rating={i8}";
    }
    string newFile = string.Join("\n", i);
    // //Debug.Log(newFile);
    File.WriteAllText(songinfopath, newFile);
  }
}

public class SongSelectScript : MonoBehaviour
{
  public static string mapPath;
  public static int currentSongIndex;
  public static string currentDifficulty = "EASY"; //"EASY","NORMAL" or "HARD"
  private List<SongInfo> allSongs;
  public GameObject particles;
  public static List<GameObject> particleList;
  public Image backgroundPanel;

  public static bool justStarted = true;
  public static SongInfo currentSong;
  public SongInfo nextSong;
  public SongInfo prevSong;
  // vvv UI ELEMENTS vvv ========================================================
  public AudioSource audioSource;
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
    if (!justStarted)
    {
      int index = allSongs.FindIndex((i) => i.getTitle() == currentSong.getTitle());
      allSongs[index] = currentSong;
    }
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
    changeParticleColour(currentSong.pvColor);
  }

  public static string getRating()
  {
    if (GameScript.score == 1000000) return "EX";
    else if (GameScript.score > 950000) return "S";
    else if (GameScript.score > 930000) return "A+";
    else if (GameScript.score > 900000) return "A";
    else if (GameScript.score > 800000) return "B";
    else if (GameScript.score > 700000) return "C";
    else return "F";
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
    EventSystem sceneEventSystem = FindObjectOfType<EventSystem>();
    if (sceneEventSystem == null)
    {
      GameObject eventSystem = new GameObject("EventSystem");
      eventSystem.AddComponent<EventSystem>();
      eventSystem.AddComponent<StandaloneInputModule>();
    }
    getAllSongs();
    if (justStarted)
    {
      currentSong = allSongs[0];
      justStarted = false;
    }
    Transform[] transformList = particles.GetComponentsInChildren<Transform>();
    particleList = new List<GameObject>();
    foreach (Transform t in transformList)
    {
      particleList.Add(t.gameObject);
    }

    int index = allSongs.FindIndex((i) => i.getTitle() == currentSong.getTitle());
    nextSong = index + 1 == allSongs.Count ? allSongs[0] : allSongs[index + 1];
    prevSong = index - 1 == -1 ? allSongs[allSongs.Count - 1] : allSongs[index - 1];
    updateUI();
    audioSource.clip = currentSong.getAudio();
    audioSource.time = (float)currentSong.pvStart;
    audioSource.Play();
    // audioSource.PlayScheduled(currentSong.pvStart);
    audioSource.volume = SettingsScript.getMusic() / 100;
    // //Debug.Log(SettingsScript.getMusic() + "/100");
    transitionPanel.CrossFadeAlpha(0, 0.5f, false);
  }

  // Update is called once per frame
  void Update()
  {
    if (audioSource.time >= currentSong.pvEnd)
    {
      StartCoroutine(fadeMusic(0, true));
    }
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
    if (angle >= -90 && angle < 90) // right
      dir = "d";
    else
      dir = "a";
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
      yield return new WaitForSecondsRealtime(.5f);
    }
    updateUI();
    songUIfade(1);
  }

  IEnumerator changeSong(string which)
  {
    songUIfade(0);
    StartCoroutine(fadeMusic(0, false));
    audioSource.Stop();
    bool faded = false;
    while (!faded)
    {
      faded = true;
      yield return new WaitForSecondsRealtime(.5f);
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
    audioSource.clip = currentSong.getAudio();
    updateUI();
    StartCoroutine(fadeMusic(1, false));
    songUIfade(1);
  }
  public void playPressed()
  {
    GameScript.resetStates();
    StartCoroutine(changeScene("GameScreen"));
  }

  public void toSettings()
  {
    SettingsScript.prevActiveScene = "SongSelect";
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
  public void changeParticleColour(Color color)
  {
    backgroundPanel.CrossFadeColor(color, .2f, false, false);
    foreach (GameObject p in particleList)
    {
      LeanTween.color(p, color, .2f);
    }
  }

  IEnumerator fadeMusic(int which,bool restart)
  {
    // which == 1 ? in : out
    if(which == 1) { // FADE IN
      audioSource.time = (float)currentSong.pvStart;
      audioSource.Play();
      // //Debug.Log("song started" + Time.timeAsDouble);
      while (audioSource.volume < SettingsScript.getMusic() / 100)
      {
        audioSource.volume += 0.5f * 2;
        yield return new WaitForSeconds(0.001f);
      }
      audioSource.volume = SettingsScript.getMusic() / 100;
    }
    else { // FADE OUT
      while (audioSource.volume > 0f)
      {
        audioSource.volume -= 0.05f;
        yield return new WaitForSeconds(0.001f);
      }
      // //Debug.Log("song ended" + Time.timeAsDouble);
      audioSource.Stop();
      if(restart){
        audioSource.time = (float)currentSong.pvStart;
        audioSource.Play();
        // //Debug.Log("song restarted -- "+ audioSource.time +"||"+Time.timeAsDouble);
        while (audioSource.volume < SettingsScript.getMusic() / 100)
        {
          audioSource.volume += 0.5f * 2;
          yield return new WaitForSeconds(0.001f);
        }
        audioSource.volume = SettingsScript.getMusic() / 100;
      }
    }
  }
  public void back()
  {
    StartCoroutine(changeScene("MainTitleScreen"));
  }
}
