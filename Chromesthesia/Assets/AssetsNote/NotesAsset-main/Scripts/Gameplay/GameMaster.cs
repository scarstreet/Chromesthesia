using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;

public class GameMaster : MonoBehaviour
{
  public float delaystart;
  public AudioSource audiosource;
  public string highscore;
  public Text currentscore;
  public int score;
  public int tempo;
  public GameObject circle;
  public class Note
  {
    public GameObject obj;
    public int type;
    public string color;
    public float posx, posy;
    public string direction;
    public double timeEnd;
    public double time;
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
  }
  public double time;
  public Text timetext;
  // Start is called before the first frame update
  public static Queue<Note> que = new Queue<Note>();
  public static Queue<Note> isoutthere = new Queue<Note>();
  public static Queue<Note> touchable = new Queue<Note>();
  public static List<(double timing, List<Note> notes)> sametime = new List<(double timing, List<Note> notes)>();

  void Start()
  {
    delaystart = 2f;
    //string highscorepath = Application.persistentDataPath + "";
    string audiopath = "Beatmap/Body Talk";
    audiosource = GetComponent<AudioSource>();
    audiosource.clip = Resources.Load<AudioClip>(audiopath);
    StartCoroutine(startsong());
    //highscore = Resources.Load<audiosource>(audiosource);
    score = 0;
    time = Time.timeAsDouble;
    timetext.text = time.ToString();
    TextAsset theList = Resources.Load<TextAsset>("Beatmap/queuenumber"); //read a textfile from "path" note that it only reads from folder Resources, so you have to put everything (that you said to Load) in resources folder, however you may make any folder inside th resouce folder.
    string text = theList.text;
    string[] words = text.Split('\n');
    List<string> wordlist = new List<string>(words);
    int bodyindex = wordlist.FindIndex(x => x.Contains("<body>")) + 1;
    for (int i = bodyindex; wordlist[i] != ""; i++) //read from after the <body> in the textfile and split it for later use
    {
      string[] list = wordlist[i].Split(',');
      int type = Convert.ToInt16(list[0]);
      string color = list[1];
      float posx = Convert.ToSingle(list[2]);
      float posy = Convert.ToSingle(list[3]);
      string direction = list[4];
      double timeEnd = Convert.ToDouble(list[5]);
      double time = Convert.ToDouble(list[6]);
      que.Enqueue(new Note(type, color, posx, posy, direction, timeEnd, time));
    }
    currentscore.text = score.ToString();
  }

  // Update is called once per frame
  void Update()
  {
    time = Time.timeAsDouble;
    timetext.text = time.ToString();
    if (que.Count == 0)
    {
      StartCoroutine(songfinished());
    }
    else if (que.Count > 0)
    {
      if (time >= que.Peek().time)
      {
        List<Note> Temp = new List<Note>();
        Note last = new Note();
        while (que.Count != 0 && que.Peek().time <= time)
        {
          GameObject create = Instantiate(circle);
          circle.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
          /*
          1. get window size, width & height
          2. height/2, width/2
          3. posx+,posy+height/2
          */
          create.transform.position = new Vector3(que.Peek().posx, que.Peek().posy, 1);
          create.transform.rotation = Quaternion.Euler(0, 0, assignrotate(que.Peek().direction[0]));
          Note temp = new Note(que.Peek().type, que.Peek().color, que.Peek().posx, que.Peek().posy, que.Peek().direction, que.Peek().timeEnd, que.Peek().time + 1f);
          temp.setGameObject(create); //assigning which gameobject to the temp
          isoutthere.Enqueue(temp); //showing the note and put in the queue
          Temp.Add(temp); //the list note of which when there are multiple touchable at the same time
          last = temp;
          que.Dequeue();
        }
        sametime.Add((last.time, Temp));
      }
    }
    if (isoutthere.Count > 0)
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
    if (Input.touchCount > 0)
    {
      for (int i = 0; i < Input.touchCount; i++)
      {
        Touch[] touches = Input.touches;
        Vector2 startPos = touches[i].rawPosition; //the first time the touch connected
        if (touches[i].phase == TouchPhase.Ended && (touches[i].deltaPosition.x < -0.01 || touches[i].deltaPosition.x > 0.01 || touches[i].deltaPosition.y > 0.01 || touches[i].deltaPosition.x < -0.01))
        {
          double timetouched = time;
          Vector2 endPos = Input.touches[i].position; //noting the endposition of the touch
          double rad = Mathf.Atan2(touches[i].deltaPosition.y, touches[i].deltaPosition.x);
          double angle = rad * (180 / Math.PI); //check the angle and change it to degrees from radian
          NoteDiamond s = touchable.Peek().getGameObject().GetComponent<NoteDiamond>();
          s.setState(directionJudgement(angle, touchable.Peek().time));
          touchable.Dequeue();
        }
      }
    }
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
    // Debug.Log(z);
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
    return dir;
  }

  private string directionJudgement(double angle, double time)
  {
    int now = sametime.FindIndex(x => x.timing >= time); // to check what time it is
    string result = "miss"; // final result of direction check
    List<(string dir, string t1, string t2, double a1, double a2)> tolerance = new List<(string, string, string, double, double)>();
    tolerance.Add(("d", "e", "c", -22.5, 22.5));
    tolerance.Add(("c", "d", "x", -22.5,-67.5));
    tolerance.Add(("x", "c", "z", -67.5,-112.5));
    tolerance.Add(("z", "x", "a",-112.5, -157.5));
    tolerance.Add(("a", "z", "q",-157.5,157.5));
    tolerance.Add(("q", "a", "w",157.5,112.5));
    tolerance.Add(("w", "q", "e",112.5,67.5));
    tolerance.Add(("e", "w", "d",67.5,22.5));

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
          double toleranceAngle = 5;
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
      if(result.Contains("miss")){
        sametime[now].notes.Remove(sametime[now].notes[0]);
        if (sametime[now].notes.Count == 0)
        {
          sametime.Remove(sametime[now]);
        }
      }
    }
    return result;
  }

  IEnumerator startsong()
  {
    yield return new WaitForSeconds(delaystart);
    audiosource.Play();
  }
  
  IEnumerator songfinished()
  {
    yield return new WaitForSeconds(5);
    audiosource.Stop();
  }
}