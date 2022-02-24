using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;

public class GameMaster : MonoBehaviour
{
    public Text currentscore;
    public int score;
    public int tempo;
    public GameObject circle;
    public class Note
    {
        public GameObject obj;
        public double time;
        public float posx,posy;
        public string direction;
        public string status;
        public Note()
        {
            this.time = 0;
            this.posx = 0;
            this.posy = 0;
            this.direction = "";
        }
        public Note(double time,float posx,float posy,string direction)
        {
            this.time = time;
            this.posx = posx;
            this.posy = posy;
            this.direction = direction;
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
        score=0;
        time = Time.timeAsDouble;
        timetext.text = time.ToString();
        TextAsset theList = Resources.Load<TextAsset>("Beatmap/queuenumber");
        string text = theList.text;
        string[] words = text.Split('\n');
        foreach(string notes in words)
        {
            string[] list = notes.Split(',');
            double time = Convert.ToDouble(list[0]);
            float posx = Convert.ToSingle(list[1]);
            float posy = Convert.ToSingle(list[2]);
            string direction = list[3];
            que.Enqueue(new Note(time,posx,posy,direction));
        }
        currentscore.text = score.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        time = Time.timeAsDouble;
        timetext.text = time.ToString();
        if(sametime.Count > 0){
            // while(time>sametime[0].timing){
            //     sametime.Remove(sametime[0]);
            // }
        }
        if(que.Count > 0){
            if(time >= que.Peek().time)
            {
                List<Note> Temp = new List<Note>();
                Note last = new Note();
                while(que.Count!=0 && que.Peek().time <= time)
                {
                    GameObject create = Instantiate(circle);
                    circle.transform.localScale = new Vector3(0.3f,0.3f,0.3f);
                    create.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(que.Peek().posx, que.Peek().posy,1));
                    // create.transform.rotation = Quaternion.Euler(90,90, 90);
                    Note temp = new Note(que.Peek().time+1f,que.Peek().posx,que.Peek().posy,que.Peek().direction);
                    temp.setGameObject(create);
                    isoutthere.Enqueue(temp);
                    Temp.Add(temp);
                    last = temp;
                    que.Dequeue();
                }
                sametime.Add((last.time,Temp));
                Debug.Log("SMAETIME XCIOBRT: "+ sametime[0].timing);
            }
        }
        if(isoutthere.Count > 0)
        {
            if(time >= isoutthere.Peek().time)
            {
                while(isoutthere.Count!=0 && isoutthere.Peek().time <= time)
                {
                    touchable.Enqueue(isoutthere.Peek());
                    isoutthere.Dequeue();
                }
            }
        }
        if(Input.touchCount>0)
        {
            for(int i=0;i<Input.touchCount;i++)
            {
                // Touch touch = Input.GetTouch(0);
                Touch[] touches = Input.touches;
                Vector2 startPos = touches[i].rawPosition;
                // Vector3 touchPosition = Camera.main.ScreenToWorldPoint(Input.touches[i].position);
                if(touches[i].phase == TouchPhase.Ended && (touches[i].deltaPosition.x <-0.15 || touches[i].deltaPosition.x > 0.15 || touches[i].deltaPosition.y > 0.15 || touches[i].deltaPosition.x < -0.15))
                {
                    double timetouched = time;
                    Vector2 endPos = Input.touches[i].position;
                    // Debug.Log(touches[i].deltaPosition);
                    double rad = Mathf.Atan2(touches[i].deltaPosition.y,touches[i].deltaPosition.x); 
                    double angle = rad * (180 / Math.PI);
                    NoteDiamond s = touchable.Peek().getGameObject().GetComponent<NoteDiamond>();
                    // Debug.Log(angle);
                    if(getDir(angle,touchable.Peek().time)==true && (touchable.Peek().time + 1 - time < 0.38  && touchable.Peek().time + 1 - time > -0.15))
                    {
                        Debug.Log(touchable.Peek().time+1.5 - time);
                        s.setState("perfect");
                    }
                    else if(getDir(angle,touchable.Peek().time)==true && (touchable.Peek().time + 1 - time < 0.8  && touchable.Peek().time + 1 - time > -0.3))
                    {
                        Debug.Log(touchable.Peek().time+1.5 - time);
                        s.setState("good");
                    }
                    else
                    {
                        Debug.Log(touchable.Peek().time+1.5 - time);
                        s.setState("miss");
                    }
                    touchable.Dequeue();
                }
            }
        }
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
        string dir="";
        if(angle >=-22.5 && angle < 22.5)
            dir = "d";
        else if(angle >=22.5 && angle<67.5)
            dir = "e";
        else if(angle >=67.5 && angle<112.5)
            dir = "w";
        else if(angle >=112.5 && angle<157.5)
            dir = "q";
        else if((angle >=157.5 && angle <=180) || (angle<-157.5 && angle >=-180))
            dir = "a";
        else if(angle >=-157.5 && angle<-112.5)
            dir = "z";
        else if(angle >=-112.5 && angle<-67.5)
            dir = "x";
        else if(angle >=-67.5 && angle<-22.5)
            dir = "c";
        return dir;
    }

    private bool getDir(double angle,double time)
    {
        Debug.Log("timing = "+time);
        Debug.Log(sametime.Count);
        int now = sametime.FindIndex(x => x.timing >= time);
        foreach((double t,List<Note> n) s in sametime){
            Debug.Log("AVAIL TIME = "+s.t);
        }
        string dir = checkDir(angle);
        Debug.Log("now = "+now+", dir = "+dir);
        if(now != -1){
            string dirs = "";
            foreach(Note note in sametime[now].notes){
                dirs += note.direction + ", ";
            }
            Debug.Log(time+" : (input)"+dir+", (selection)"+dirs);
            if(sametime[now].notes.FindIndex(x => x.direction[0] == dir[0]) != -1) {
                Note sameDirNote = sametime[now].notes.Find(x => x.direction[0] == dir[0]);
                Debug.Log("Removing "+sameDirNote.direction);
                sametime[now].notes.Remove(sameDirNote);
                if(sametime[now].notes.Count == 0){
                    sametime.Remove(sametime[now]);
                }
                return true;
            }
        }
        return false;
    }
}
