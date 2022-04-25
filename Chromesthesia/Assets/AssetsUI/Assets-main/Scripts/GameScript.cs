using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.IO;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class GameScript : MonoBehaviour
{
  public class Finger
  {
    static int pathCnt = 7 * (Screen.currentResolution.refreshRate/60); // How many entries before can decide stuff
    double touchStarted;
    Queue<Vector2> path;
    public int fingerId;
    public bool taken;
    public Finger(int id, Vector2 pos)
    {
      path = new Queue<Vector2>();
      path.Enqueue(pos);
      touchStarted = Time.timeAsDouble;
      fingerId = id;
    }
    public void updatePath(Vector2 posNow)
    {
      if (path.Count < pathCnt)
      { // 60fps, saving for every 0.16 seconds, so 10 frames saved 
        path.Enqueue(posNow);
      }
      else
      {
        path.Dequeue();
        path.Enqueue(posNow);
      }
    }
    public double decideAngle()
    {
      List<Vector2> pathList = path.ToList();
      Vector2 deltaPos = pathList[pathList.Count - 1] - pathList[0];
      double rad = Mathf.Atan2(deltaPos.y, deltaPos.x);
      double angle = rad * (180 / Math.PI); //check the angle and change it to degrees from radian
      return angle;
    }
    public bool isHold()
    {
      List<Vector2> pathList = path.ToList();
      Vector2 deltaPos = pathList[pathList.Count - 1] - pathList[0];
      // Debug.Log(deltaPos);
      if (Math.Abs(deltaPos.x) <= 50 && Math.Abs(deltaPos.y) <= 50)
      {
        return true;
      }
      return false;
    }
    public bool isJudgable()
    {
      if (path.Count < pathCnt)
      {
        return false;
      }
      return true;
    }
  }
  public class Note
  {
    // static info ===================================================================
    public GameObject obj;
    public string color, direction;
    public double time, timeEnd;
    public float posx, posy;
    public int type;
    // dynamic info ===================================================================
    public double timeSpawned; // Needed to calculate how much time left for the hold 
    public double timePressed; // Needed to calculate how much time left for the hold
    public int fingerId;
    public Note()
    {
      this.type = 0;
      this.color = "#FFFFFF";
      this.posx = 0;
      this.posy = 0;
      this.direction = "";
      this.timeEnd = 0;
      this.time = 0;
    }
    public Note(int type, string color, float posx, float posy, string direction, double timeEnd, double time)
    {
      this.type = type;
      this.color = color;
      this.posx = posx;
      this.posy = posy;
      this.direction = direction;
      this.timeEnd = timeEnd;
      this.time = time;
    }
    public void setGameObject(GameObject obj)
    {
      this.obj = obj;
    }
    public GameObject getGameObject()
    {
      return obj;
    }
    public void setHoldProperties(int fingerId)
    {
      this.fingerId = fingerId;
      this.timePressed = songProgress * duration;
      double remainDur = this.timeEnd - this.time;
      remainDur = remainDur - (this.timePressed - this.time) - 0.5;
      HoldSpawn objScript = this.obj.GetComponent<HoldSpawn>();
      HoldWait script = objScript.HoldStart.GetComponent<HoldWait>();
      script.holdDuration = remainDur * 1000;
      script.bgColor = objScript.bgColor;
      GameObject temp = objScript.HoldStart;
      Vector3 tempPos = objScript.gameObject.transform.position;
      Quaternion tempRot = objScript.gameObject.transform.rotation;
      objScript.setState("touched"); // here lies the previous hold object, dead
      this.obj = Instantiate(temp, tempPos, tempRot); // reincarnate to become holdWait
    }
  }

  // GAMEPLAY LISTS =====================================================================================================
  public static Queue<Note> que = new Queue<Note>(), isoutthere = new Queue<Note>(), touchable = new Queue<Note>();
  public static List<(double timing, List<Note> notes)> sametime = new List<(double timing, List<Note> notes)>();
  public static List<Note> existingHolds = new List<Note>(), waitingHolds = new List<Note>();
  public static List<Finger> fingerList = new List<Finger>();

  // GAMEPLAY STATES ===============================================================================================
  public static bool gameIsPaused = false, gameCompleted = false, gameStarted = false;
  public static int combo, maxcombo, perfectcount, goodcount, misscount;
  public static float songProgress = 0, elapsed = 0;
  public static double score, accuracy;
  public static int count;
  static double gameStartTime, time, duration; // SONG DURATION IN SECONDS
  public float delaystart;
  public double addscore,addaccuracy;
  // GAMEOBJECT RESOURCE ============================================================================================
  public GameObject circle, hold, countdown, particles;
  public Button pauseButton;
  public static List<GameObject> particleList;
  public static AudioSource audiosource;
  public static GameObject self;
  // UI =============================================================================================================
  public Text difficulty, song, currentscore, combotext;
  public Image backgroundPanel, transitionPanel;
  public Animator animator;

  public static void resetStates()
  {
    score = 0;
    combo = 0;
    maxcombo = 0;
    perfectcount = 0;
    goodcount = 0;
    misscount = 0;
    count = 0;
    accuracy = 0f;
    que = new Queue<Note>();
    isoutthere = new Queue<Note>();
    touchable = new Queue<Note>();
    sametime = new List<(double timing, List<Note> notes)>();
    existingHolds = new List<Note>();
    waitingHolds = new List<Note>();
    Time.timeScale = 1;
    AudioListener.pause = false;
    if (audiosource)
    {
      audiosource.Stop();
    }
    gameIsPaused = false;
    gameCompleted = false;
    gameStarted = false;
    songProgress = 0;
    elapsed = 0;
    duration = SongSelectScript.currentSong.duration;
    score = 0;
    Destroy(self);
    // self = gameObject;
  }
  void Awake()
  {
    DontDestroyOnLoad(gameObject);
    foreach (Transform go in gameObject.GetComponentsInChildren<Transform>())
    {
      GameObject ddol = go.gameObject;
      DontDestroyOnLoad(ddol);
    }
  }
  void Start()
  {
    pauseButton.interactable = false;
    score = 0;
    combo = 0;
    maxcombo = 0;
    perfectcount = 0;
    goodcount = 0;
    misscount = 0;
    count = 0;
    accuracy = 0f;
    self = gameObject;
    difficulty.text = SongSelectScript.currentDifficulty;
    Transform[] transformList = particles.GetComponentsInChildren<Transform>();
    particleList = new List<GameObject>();
    // Debug.Log(transformList.Length);
    foreach (Transform t in transformList)
    {
      particleList.Add(t.gameObject);
    }
    song.text = SongSelectScript.currentSong.getTitle() + " - " + SongSelectScript.currentSong.getArtist();
    Time.timeScale = 0;
    //=====================================================================================
    if (!gameStarted)
    {
      currentscore.text = ((int)score).ToString();
      audiosource = GetComponent<AudioSource>();
      audiosource.clip = SongSelectScript.currentSong.getAudio();
      audiosource.volume = SettingsScript.Music / 100;
      Debug.Log(SettingsScript.Music + "/100");
      TextAsset theList = Resources.Load<TextAsset>(SongSelectScript.beatmapPath()); //read a textfile from "path" note that it only reads from folder Resources, so you have to put everything (that you said to Load) in resources folder, however you may make any folder inside th resouce folder.
      string text = theList.text;
      string[] words = text.Split('\n');
      List<string> wordlist = new List<string>(words);
      int bodyindex = wordlist.FindIndex(x => x.Contains("<body>")) + 1;
      for (int i = bodyindex; i < wordlist.Count; i++) //read from after the <body> in the textfile and split it for later use
      {
        string[] list = wordlist[i].Split(',');
        if (list.Length == 1)
        {
          break;
        }
        int type = Convert.ToInt16(list[0]);
        string color = list[1];
        float posx = Convert.ToSingle(list[2]);
        float posy = Convert.ToSingle(list[3]);
        string direction = list[4];
        double timeEnd = Convert.ToDouble(list[5]);
        double time = Convert.ToDouble(list[6]);
        if (time > 0f)//remove after you finish debugging
          que.Enqueue(new Note(type, color, posx, posy, direction, timeEnd, time));
        count++;
      }
      addscore = (double)1000000 / count;
      addaccuracy = (double)100 / count;
      currentscore.text = ((int)score).ToString();
    }
    //=======================================================================================
    StartCoroutine(postStart());
  }

  public void pauseDone()
  {
    StartCoroutine(postStart());
  }
  void Update()
  {
    if (gameStarted && !gameIsPaused)
    {
      pauseButton.interactable = true;
      songProgress = ((float)(Time.timeAsDouble - gameStartTime) / (float)(duration));
      animator.SetFloat("runner", songProgress);
      time = songProgress * duration;
      combotext.text = combo.ToString();
      if (que.Count == 0)
      {
        StartCoroutine(songfinished());
      }
      else if (que.Count > 0) // Putting stuff to queue
      {
        if (time >= que.Peek().time)
        {
          List<Note> Temp = new List<Note>();
          Note last = new Note();
          while (que.Count != 0 && que.Peek().time <= time)
          {
            GameObject create = que.Peek().type == 0 ? Instantiate(circle) : Instantiate(hold);
            if (que.Peek().type == 0)
            {
              NoteDiamond script = create.GetComponent<NoteDiamond>();
              ColorUtility.TryParseHtmlString(que.Peek().color, out script.bgColor);
            }
            else
            {
              HoldSpawn script = create.GetComponent<HoldSpawn>();
              ColorUtility.TryParseHtmlString(que.Peek().color, out script.bgColor);
            }
            circle.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            create.transform.position = new Vector3(que.Peek().posx, que.Peek().posy, 1);
            create.transform.rotation = Quaternion.Euler(0, 0, assignrotate(que.Peek().direction[0]));
            Note temp = new Note(que.Peek().type, que.Peek().color, que.Peek().posx, que.Peek().posy, que.Peek().direction, que.Peek().timeEnd, que.Peek().time + 1f); //changed to 1.5f (trying)
            temp.timeSpawned = Time.timeAsDouble;
            temp.setGameObject(create); //assigning which gameobject to the temp
            isoutthere.Enqueue(temp); //showing the note and put in the queue
            Temp.Add(temp); //the list note of which when there are multiple touchable at the same time
            last = temp;
            que.Dequeue();
            if (temp.type == 1)
            {
              existingHolds.Add(temp);
            }
          }
          sametime.Add((last.time, Temp));
        }
      }
      if (isoutthere.Count > 0) // Putting stuff to the screen
      {
        if (time >= isoutthere.Peek().time)
        {
          while (isoutthere.Count != 0 && isoutthere.Peek().time <= time)
          {
            touchable.Enqueue(isoutthere.Peek());
            isoutthere.Dequeue();
          }
        }
      }
      if (Input.touchCount > 0) // If an input is detected; 
      {
        Touch[] touches = Input.touches;
        for (int i = 0; i < Input.touchCount; i++)
        {
          // creating & updating fingerlist --------------------------------------------------------------------------
          int idx = fingerList.FindIndex((ii) => ii.fingerId == touches[i].fingerId);
          if (idx != -1)
          { // if the current finger's Id is already in the array
            fingerList[idx].updatePath(touches[i].position);
          }
          else
          {
            fingerList.Add(new Finger(touches[i].fingerId, touches[i].position));
          }
          // judgements ---------------------------------------------------------------------------------------------
          if (fingerList[idx].isHold() && fingerList[idx].isJudgable()) // ERR - index out of range
          {
            foreach (Note note in existingHolds)
            {
              if (touchable.Contains(note) && waitingHolds.FindIndex((n) => n.fingerId == touches[i].fingerId) == -1)
              {
                note.setHoldProperties(touches[i].fingerId);
                HoldSpawn script = note.obj.GetComponent<HoldSpawn>();
                waitingHolds.Add(note);
                touchable = new Queue<Note>(touchable.Where(x => x != note)); // remove from original 
              }
            }
          }
          else if ((touches[i].phase == TouchPhase.Ended) && (touches[i].deltaPosition != touches[i].rawPosition))
          {

            double angle = fingerList[idx].decideAngle();
            Debug.Log("Angle : " + checkDir(angle) + " Direction : " + touchable.Peek().direction);
            int holdIndex = waitingHolds.FindIndex((note) => note.fingerId == touches[i].fingerId);
            if (holdIndex != -1)
            {
              // IF HOLD'S SWIPE
              Note held = waitingHolds[holdIndex];
              HoldWait script = held.obj.GetComponent<HoldWait>();
              script.setState(directionJudgement(angle, held.time));
              waitingHolds.Remove(held);
            }
            else
            { // IF SWIPE
              if (touchable.Count > 0)
              {
                // TODO - tryna solve missing ref error by code below
                while (touchable.Peek().getGameObject() == null)
                {
                  touchable.Dequeue(); // to dequeue all previously missed notes
                }
                NoteDiamond s = touchable.Peek().getGameObject().GetComponent<NoteDiamond>();

                s.setState(directionJudgement(angle, touchable.Peek().time));
                /*
                TODO : FIX THE PERFECT GOOD MISS SCORE AND COMBO HERE CAUSE IT BUGS WHEN IT'S UP THERE
                HOW? DIRECTIONJUDGEMENT IS THE LAST JUDGEMENT MEANING THAT IT IS DEFINED LASTLY HERE, SO THIS IS THE FACTOR, THE POINT IS JUST MOVE IT HERE
                */
                touchable.Dequeue();
              }
            }
          }
        }
      }
      if (songProgress >= 1 && !gameCompleted)
      {
        gameCompleted = true;
        StartCoroutine(toScoreScreen());
      }
      // animator.SetFloat("RunnerProgress", songProgress);
    }
    combotext.text = combo.ToString();
  }

  public void pauseGame()
  {
    Time.timeScale = 0;
    AudioListener.pause = true;
    gameIsPaused = true;
    PauseScript.pauseOpen = true;
    pauseButton.interactable = false;
    SceneManager.LoadScene("PauseScreen", LoadSceneMode.Additive);
  }
  private float assignrotate(char dir) //this function is to check the note direction and assign it by looking at the deltaPosition
  {
    float z = 0;
    if (dir == 'w')
      z = 0;
    else if (dir == 'q')
      z = 45;
    else if (dir == 'a')
      z = 90;
    else if (dir == 'z')
      z = 135;
    else if (dir == 'x')
      z = 180;
    else if (dir == 'c')
      z = 225;
    else if (dir == 'd')
      z = 270;
    else if (dir == 'e')
      z = 315;
    // // Debug.Log(z);
    return z;
  }

  /*
  Direction :
  w = up;
  q = upleft;
  a = left;
  z = downleft;
  x = down;
  c = downright;
  d = right;
  e = upright;
  */

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
    // Debug.Log(angle + " " + dir);
    return dir;
  }

  private string directionJudgement(double angle, double time)
  {
    int now = sametime.FindIndex(x => x.timing >= time); // to check what time it is

    string result = "miss"; // final result of direction check
    List<(string dir, string t1, string t2, double a1, double a2)> tolerance = new List<(string, string, string, double, double)>();
    tolerance.Add(("d", "e", "c", -22.5, 22.5));
    tolerance.Add(("c", "d", "x", -22.5, -67.5));
    tolerance.Add(("x", "c", "z", -67.5, -112.5));
    tolerance.Add(("z", "x", "a", -112.5, -157.5));
    tolerance.Add(("a", "z", "q", -157.5, 157.5));
    tolerance.Add(("q", "a", "w", 157.5, 112.5));
    tolerance.Add(("w", "q", "e", 112.5, 67.5));
    tolerance.Add(("e", "w", "d", 67.5, 22.5));

    string dir = checkDir(angle);
    if (now != -1)
    {
      string dirs = "";
      foreach (Note note in sametime[now].notes)
      {
        dirs += note.direction + ", ";
      }
      if (sametime[now].notes.FindIndex(x => x.direction[0] == dir[0]) != -1) // if this direction exists
      {
        Note sameDirNote = sametime[now].notes.Find(x => x.direction[0] == dir[0]);

        sametime[now].notes.Remove(sameDirNote);
        if (sametime[now].notes.Count == 0)
        {

          sametime.Remove(sametime[now]);
        }
        result = "perfect";
        perfectcount++;
        score += addscore;
        accuracy += addaccuracy;
        combo++;
        if (combo > maxcombo)
          maxcombo++;
        //
        currentscore.text = Math.Round(score, 0).ToString();
      }
      else
      {
        int tId = tolerance.FindIndex(s => s.dir.Contains(dir)); // tolerance index
        int tolerate1 = sametime[now].notes.FindIndex(x => x.direction.Contains(tolerance[tId].t1));
        int tolerate2 = sametime[now].notes.FindIndex(x => x.direction.Contains(tolerance[tId].t2));
        if (tolerate1 != -1 || tolerate2 != -1)
        { // If found neighboring tolerance
          int toRemove = tolerate1 != -1 ? tolerate1 : tolerate2;
          double adjacentAngle = tolerate1 != -1 ? tolerance[tId].a1 : tolerance[tId].a2;
          double toleranceAngle = 5;
          if (angle <= adjacentAngle + toleranceAngle || angle >= adjacentAngle - toleranceAngle)
          { // to tolerate or to not tolerate
            result = "good";
            goodcount++;
            score += addscore * 0.75;
            accuracy += addaccuracy *0.5;
            combo++;
            if (combo > maxcombo)
              maxcombo++;
            currentscore.text = Math.Round(score, 0).ToString();
          }

          sametime[now].notes.Remove(sametime[now].notes[toRemove]);
          if (sametime[now].notes.Count == 0)
          {

            sametime.Remove(sametime[now]);
          }
        }
      }
      if (result.Contains("miss") || result.Contains("noInput"))
      {
        combo = 0;

        sametime[now].notes.Remove(sametime[now].notes[0]);
        if (sametime[now].notes.Count == 0)
        {

          sametime.Remove(sametime[now]);
        }
      }
    }
    return result;
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
  IEnumerator startsong()
  {
    if (!gameStarted)
    {
      yield return new WaitForSeconds(delaystart);
    }
    else
    {
      audiosource.time = songProgress * (float)duration; //0f is for debugging,remove after use
    }
    audiosource.Play();
  }

  IEnumerator songfinished()
  {
    yield return new WaitForSeconds(7);
    audiosource.Stop();
  }
  IEnumerator postStart()
  {
    for (int i = 0; i < 3; i++)
    {
      if (i == 0)
      {
        animator.SetFloat("runner", songProgress);
        transitionPanel.CrossFadeAlpha(0, 0.5f, true);
        yield return new WaitForSecondsRealtime(.5f);
      }
      else if (i == 1)
      {
        GameObject cd = Instantiate(countdown, new Vector3(0, 0, 100), Quaternion.identity);
        cd.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform, false);
        yield return new WaitForSecondsRealtime(4.5f);
      }
      else
      {
        delaystart = 2f;
        if (!gameStarted)
        {
          time = Time.timeAsDouble + 0f; //remove 0f after you finish debugging
          combotext.text = combo.ToString();
          gameStartTime = Time.timeAsDouble;
          StartCoroutine(startsong());
          gameStarted = true;
          pauseButton.interactable = false;
        }
        else if (gameIsPaused)
        {
          gameIsPaused = false;
          AudioListener.pause = false;
          pauseButton.interactable = true;
        }
        Time.timeScale = 1;
      }
      yield return new WaitForSecondsRealtime(0f);
    }
  }
  IEnumerator toScoreScreen()
  {
    bool fadeDone = false;
    while (!fadeDone)
    {
      transitionPanel.CrossFadeAlpha(1, 0.5f, true);
      fadeDone = true;
      yield return new WaitForSecondsRealtime(.5f);
    }
    SceneManager.LoadScene("ScoreScreen", LoadSceneMode.Single);
    Destroy(gameObject);
  }

  public void changeParticleColour(Color color)
  {
    backgroundPanel.CrossFadeColor(color, .2f, false, false);
    // particles.LeanColor(color, .2f);
    foreach (GameObject p in particleList)
    {
      LeanTween.color(p, color, .2f);
    }
  }
}