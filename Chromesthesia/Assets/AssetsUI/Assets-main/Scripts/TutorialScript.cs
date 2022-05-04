using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.IO;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class TutorialScript : MonoBehaviour
{
  // Start is called before the first frame update
  public static string prevScene;
  public class Finger
  {
    static int pathCnt = 7 * (Screen.currentResolution.refreshRate / 60); // How many entries before can decide stuff
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
    public string checkDirs()
    {
      List<Vector2> pathList = path.ToList();
      string[] dirs = new string[pathCnt - 1];

      for (int i = 0; i < pathList.Count - 1; i++)
      {
        Vector2 deltaPos = pathList[pathList.Count - 1] - pathList[i];
        double rad = Mathf.Atan2(deltaPos.y, deltaPos.x);
        double angle = rad * (180 / Math.PI);
        dirs[i] = GameScript.self.GetComponent<GameScript>().checkDir(angle);
      }
      return String.Join(' ', dirs);
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
      this.timePressed = Time.timeAsDouble;
      double remainDur = this.timePressed - this.timeEnd - 0.5 + 1f;
      Debug.Log(this.timeEnd+" - "+this.timePressed+" - "+0.5+" + "+1f);
      // remainDur = remainDur - (this.timePressed - this.time) - 0.5 + 1f;
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

  string mode = "swipe"; // swipe or hold
  public static bool isTutorialOpen;
  public static Queue<Note> que = new Queue<Note>(), isoutthere = new Queue<Note>(), touchable = new Queue<Note>();
  public static List<(double timing, List<Note> notes)> sametime = new List<(double timing, List<Note> notes)>();
  public static List<Note> existingHolds = new List<Note>(), waitingHolds = new List<Note>();
  public static List<Finger> fingerList = new List<Finger>();
  public Text title, desc, currentMode;
  public Button back;
  string swipeDesc = "Swipe the note when the arrow reaches the main body of the normal swipe note.";
  string holdDesc = "Hold the note before the white filling reaches the middle. Then, wait until the arrow reaches the main body and swipe it according to the direction.";
  public GameObject foreverSwipe, foreverHold;
  public Image transitionPanel;
  void Start()
  {
    // GameScript.gameStarted = true;
    isTutorialOpen = true;
    StartCoroutine(addNote());
    transitionPanel.CrossFadeAlpha(0, 0.5f, false);
  }
  // Update is called once per frame
  void Update()
  {
    if (que.Count > 0) // Putting stuff to queue
    {
      if (Time.timeAsDouble >= que.Peek().time)
      {
        List<Note> Temp = new List<Note>();
        Note last = new Note();
        while (que.Count != 0 && que.Peek().time <= Time.timeAsDouble)
        {
          GameObject create = que.Peek().type == 0 ? Instantiate(foreverSwipe) : Instantiate(foreverHold);
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
          if(mode.Contains("hold")) {
            create.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
          }
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
      if (Time.timeAsDouble >= isoutthere.Peek().time)
      {
        while (isoutthere.Count != 0 && isoutthere.Peek().time <= Time.timeAsDouble)
        {
          touchable.Enqueue(isoutthere.Peek());
          isoutthere.Dequeue();
        }
      }
    }
    if (Input.touchCount > 0 && !IsPointerOverUIObject()) // If an input is detected; 
    {
      Debug.Log("Touches " + Input.touchCount);
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
        else if ((touches[i].phase == TouchPhase.Ended))
        {
          double angle = fingerList[idx].decideAngle();
          // Debug.Log("Angle : " + checkDir(angle) + " Direction : " + touchable.Peek().direction);
          int holdIndex = waitingHolds.FindIndex((note) => note.fingerId == touches[i].fingerId);
          if (holdIndex != -1)
          {
            // IF HOLD'S SWIPE
            Note held = waitingHolds[holdIndex];
            HoldWait script = held.obj.GetComponent<HoldWait>();
            script.setState(directionJudgement(angle, held.time, fingerList[idx]));
            // Debug.Log("Status :" + script.status);
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
              Debug.Log("Hi??");
              s.setState(directionJudgement(angle, touchable.Peek().time, fingerList[idx]));
              touchable.Dequeue();
            }
          }
        }
      }
    }
  }
  public void changeMode()
  {
    mode = mode.Contains("swipe") ? "hold" : "swipe";
    title.text = mode.Contains("swipe") ? "NORMAL SWIPE" : "HOLD-SWIPE";
    desc.text = mode.Contains("swipe") ? swipeDesc : holdDesc;
    currentMode.text = mode.Contains("swipe") ? "SWITCH TO HOLD-SWIPE" : "SWITCH TO NORMAL SWIPE";
  }

  public void backButton()
  {
    if(prevScene == null) {
      prevScene = "MainTitleScreen";
    }
    isTutorialOpen = false;
    StartCoroutine(changeScene(prevScene));
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
  public string checkDir(double angle)
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
  private string directionJudgement(double angle, double time, Finger finger)
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
          double toleranceAngle = 22.5;
          if (angle <= adjacentAngle + toleranceAngle || angle >= adjacentAngle - toleranceAngle)
          { // to tolerate or to not tolerate
            result = "good";
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
        Debug.Log("InpurDir : " + dir + ", sametimes: - " + Time.frameCount);
        foreach (Note n in sametime[now].notes)
        {
          Debug.Log(n.direction + " - " + sametime[now].timing + " - " + Time.frameCount);
        }
        Debug.Log(finger.checkDirs() + " - " + Time.frameCount);
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
  IEnumerator addNote()
  {
    while (true)
    {
      if (mode.Contains("swipe"))
      {
        que.Enqueue(new Note(0, "#FFFFFF", -4.972133f, -0.3314161f, "w", 0, Time.timeAsDouble + 3));
        yield return new WaitForSecondsRealtime(3f);
      }
      else
      {
        que.Enqueue(new Note(1, "#FFFFFF", -4.972133f, -0.3314161f, "w", Time.timeAsDouble + 2, Time.timeAsDouble + 5));
        yield return new WaitForSecondsRealtime(7f);
      }
    }
  }
}
