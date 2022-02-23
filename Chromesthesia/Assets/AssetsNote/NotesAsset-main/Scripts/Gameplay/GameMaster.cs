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
    public Queue<Note> que = new Queue<Note>();
    public Queue<Note> isoutthere = new Queue<Note>();
    public Queue<Note> touchable = new Queue<Note>();

    void Start()
    {
        score=0;
        time = Time.timeAsDouble;
        timetext.text = time.ToString();
        TextAsset theList = Resources.Load<TextAsset>("Beatmap/queuenumber");
        string text = theList.text;
        // char trigger='\n';
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
        currentscore.text = score.ToString();
        if(que.Count==0)
        {
            // Debug.Log("Queue empty");
        }
        else if(time >= que.Peek().time)
        {
            while(que.Count!=0 && que.Peek().time <= time)
            {
                GameObject create = Instantiate(circle);
                circle.transform.localScale = new Vector3(0.3f,0.3f,0.3f);
                create.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(que.Peek().posx, que.Peek().posy,1));
                // create.transform.rotation = Quaternion.Euler(90,90, 90);
                Note temp = new Note(que.Peek().time+1f,que.Peek().posx,que.Peek().posy,que.Peek().direction);
                temp.setGameObject(create);
                isoutthere.Enqueue(temp);
                que.Dequeue();
            }
        }
        if(isoutthere.Count==0)
        {
            //Debug.Log("Isoutthere empty");
        }
        else if(time >= isoutthere.Peek().time)
        {
            while(isoutthere.Count!=0 && isoutthere.Peek().time <= time)
            {
                touchable.Enqueue(isoutthere.Peek());
                //Debug.Log(touchable.Peek().time);
                isoutthere.Dequeue();
            }
        }
        if(Input.touchCount>0)
        {
            for(int i=0;i<Input.touchCount;i++)
            {
                Touch touch = Input.GetTouch(0);
                Vector2 startPos = touch.rawPosition;
                // Vector3 touchPosition = Camera.main.ScreenToWorldPoint(Input.touches[i].position);
                if(touch.phase == TouchPhase.Ended)
                {
                    Vector2 endPos = Input.touches[i].position;
                    // double rad = Mathf.Atan(((endPos.y-startPos.y)/(endPos.x-startPos.x)));
                    double rad = Mathf.Atan2(touch.deltaPosition.y,touch.deltaPosition.x); 
                    double angle = rad * (180 / Math.PI);
                    NoteDiamond s = touchable.Peek().getGameObject().GetComponent<NoteDiamond>();
                    Debug.Log(angle);
                    if(check(angle,touchable.Peek().direction)==true && checktime(touchable.time+))
                    {
                        s.setState("perfect");
                    }
                    else
                        s.setState("miss");
                    touchable.Dequeue();
                }
            }
        }
    }

    public bool check(double angle,string truecircle)
    {
        string dir="";
        if(angle >=-22.5 && angle < 22.5)
            dir = "right";
        else if(angle >=22.5 && angle<67.5)
            dir = "upright";
        else if(angle >=67.5 && angle<112.5)
            dir = "up";
        else if(angle >=112.5 && angle<157.5)
            dir = "upleft";
        else if(angle >=157.5 && angle<-157.5)
            dir = "left";
        else if(angle <=-157.5 && angle<-112.5)
            dir = "downleft";
        else if(angle <=-112.5 && angle<-67.5)
            dir = "down";
        else if(angle <=-67.5 && angle<-22.5)
            dir = "downright";
        Debug.Log(dir);
        if(dir==truecircle)
            return true;
        return false;
    }
}
