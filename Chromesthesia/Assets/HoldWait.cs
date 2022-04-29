using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldWait : MonoBehaviour
{
  // Start is called before the first frame update
  public Color bgColor;
  public GameScript game;
  public double holdDuration = 5000;
  private double oriHoldDuration = 5000;
  public string timeStatus = "miss";
  public string dirStatus = "";
  public string status = "";
  Animator animator;
  public GameObject perfect;
  public GameObject good;
  public GameObject miss;
  public GameObject death;
  private NoteDiamondDeath deathScript;
  void Start()
  {
    game = GameScript.self.GetComponent<GameScript>();
    deathScript = death.GetComponent<NoteDiamondDeath>();
    animator = gameObject.GetComponent<Animator>();
  }

  // Update is called once per frame
  void Update()
  {
    if (GameScript.gameStarted == false)
    {
      Destroy(gameObject);
    }
    if (PauseScript.pauseOpen == true)
    {
      gameObject.LeanMoveZ(-100, 0f).setIgnoreTimeScale(true);
    }
    else if(PauseScript.pauseOpen == false)
    {
      gameObject.LeanMoveZ(0, 0f).setIgnoreTimeScale(true);
    }
  }
  void Awake()
  {
    DontDestroyOnLoad(gameObject);
  }
  void changeDuration(string input)
  {
    if (input.Contains("start"))
    {
      float speed = (float)(holdDuration / oriHoldDuration);
      animator.speed = 1 / speed;
    }
    else
    {
      animator.speed = 1;
    }
  }

  public void setTimeStat(string state)
  {
    timeStatus = state;
  }
  public void setState(string dirStatus)
  {
    NoteDiamondResult resultScript;
    statusDetermine(dirStatus);
    if (dirStatus.Contains("noInput") || status.Contains("miss"))
    {
      StartCoroutine(FlickerOut());
      resultScript = miss.GetComponent<NoteDiamondResult>();
      resultScript.nextColor = new Color((255f / 255f), (100f / 255f), (100f / 255f), 1); // sum light red
      deathScript.nextColor = new Color((255f / 255f), (100f / 255f), (100f / 255f), 1); // sum light red
      deathScript.nextAnimation = miss;
      if (dirStatus.Contains("noInput"))
      {
        GameScript.combo = 0;
        double time = Time.timeAsDouble;
        if (GameScript.waitingHolds.Count > 0)
        {
          int index = GameScript.waitingHolds.FindIndex((note) => time >= note.timeEnd);
          if (index != -1)
          {
            while (index != -1)
            {
              index = GameScript.waitingHolds.FindIndex((note) => time >= note.timeEnd);
              // Debug.Log(index);
              GameScript.waitingHolds.Remove(GameScript.waitingHolds[index]); // ERR - out of range
            }
          }
        }
      }
      Debug.Log("SWIPE || dir: " + dirStatus + ", time: " + timeStatus + ", result = " + status + " -- " + Time.frameCount);
      game.changeParticleColour(bgColor);
    }
    else
    {
      if (status.Contains("good"))
      {
        resultScript = good.GetComponent<NoteDiamondResult>();
        resultScript.nextColor = new Color((110f / 255f), (225f / 255f), (255f / 255f), 1); // sum light blu
        deathScript.nextColor = new Color((110f / 255f), (225f / 255f), (255f / 255f), 1); // sum light blu
        deathScript.nextAnimation = good;
      }
      else if (status.Contains("good"))
      {
        resultScript = good.GetComponent<NoteDiamondResult>();
        resultScript.nextColor = new Color((110f / 255f), (225f / 255f), (255f / 255f), 1); // sum light blu
        deathScript.nextColor = new Color((110f / 255f), (225f / 255f), (255f / 255f), 1); // sum light blu
        deathScript.nextAnimation = good;
      }
      else if (status.Contains("perfect"))
      {
        resultScript = perfect.GetComponent<NoteDiamondResult>();
        resultScript.nextColor = new Color((255f / 255f), (230f / 255f), (100f / 255f), 1); // sum light yello
        deathScript.nextColor = new Color((255f / 255f), (230f / 255f), (100f / 255f), 1); // sum light yello
        deathScript.nextAnimation = perfect;
      }
      else
      {
        Debug.Log("omegeh, Note has bad timeStatus. This should not happen");
      }
      Destroy(gameObject);
      Instantiate(death, transform.position, transform.rotation);
    }
  }
  void statusDetermine(string dirStatus)
  {
    if (timeStatus.Contains("perfect") && dirStatus.Contains("perfect"))
      status = "perfect";
    else if ((timeStatus.Contains("perfect") || timeStatus.Contains("good")) && (dirStatus.Contains("good") || dirStatus.Contains("perfect")))
      status = "good";
    else if (timeStatus.Contains("good") && dirStatus.Contains("good"))
      status = "good";
    else
      status = "miss";
    // Debug.Log("HOLD || dir: " + dirStatus + ", time: " + timeStatus + ", result = " + status);
  }
  IEnumerator FlickerOut()
  {
    bool wait = true;
    animator.speed = 0;
    gameObject.LeanAlpha(0, 0.09f).setEaseOutBounce();
    while (wait)
    {
      wait = false;
      yield return new WaitForSeconds(.09f);
    }
    Destroy(gameObject);
    Instantiate(death, transform.position, transform.rotation);
  }
}
