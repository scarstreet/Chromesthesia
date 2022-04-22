using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteDiamond : MonoBehaviour
{
  public Color bgColor = new Color(1f,1f,1f);
  public GameScript game;
  Renderer[] renderers;
  private IEnumerator coroutine;
  Animator animator;
  public GameObject perfect;
  public GameObject good;
  public GameObject miss;
  public GameObject death;
  private NoteDiamondDeath deathScript;

  /*********************************************************************
		Right now, the animation for this image is 2 seconds:
		40 frames scaling down, everything else is summoned. anyways,
		2 secs = 120 frames at 60 fps. so 40 is 1/3 of the whole duration.
		if I wanna fade in till 40, gotta take 1/3 of the duration
	*********************************************************************/
  public double duration = 2500; // in miliseconds
  private double originalDuration = 2500;
  private float totalFrame = 150;
  private float spawnFrame = 40;
  public string timeStatus = "noInput"; // ''/'miss'/'good'/'perfect'
  public string status = "";
  // Start is called before the first frame update
  void Start()
  {
    gameObject.LeanAlpha(0,0f);
    game = GameScript.self.GetComponent<GameScript>();
    timeStatus = "noInput";
    deathScript = death.GetComponent<NoteDiamondDeath>();
    renderers = GetComponentsInChildren<Renderer>();
    animator = gameObject.GetComponent<Animator>();
    StartCoroutine(FadeIn());
    float speed = (float)(duration / originalDuration); // TODO - make sure the inputted value good.
    animator.speed = 1 / speed;
  }
  void Awake()
  {
    DontDestroyOnLoad(gameObject);
  }
  void Update()
  {
    if (GameScript.gameStarted == false)
    {
      Destroy(gameObject);
    }
    if(GameScript.gameIsPaused == true) {
      gameObject.LeanMoveZ(-100, 0f).setIgnoreTimeScale(true);
    } else {
      gameObject.LeanMoveZ(0, 0f).setIgnoreTimeScale(true);
    }
  }
  
  public void setState(string dirStatus)
  {
    NoteDiamondResult resultScript;
    statusDetermine(dirStatus);
    // TODO - consider both timing/direction and 
    if (status.Contains("miss") || timeStatus.Contains("noInput"))
    {
      GameScript.combo=0;
      resultScript = miss.GetComponent<NoteDiamondResult>();
      resultScript.nextColor = new Color((255f / 255f), (100f / 255f), (100f / 255f), 1); // sum light red
      deathScript.nextColor = new Color((255f / 255f), (100f / 255f), (100f / 255f), 1); // sum light red
      deathScript.nextAnimation = miss;
      if (dirStatus.Contains("noInput"))
      {
        // Debug.Log(gameObject + "!!! Dequeueing !!!");
        if (GameScript.touchable.Count > 0)
        {
          GameScript.touchable.Dequeue();
        }
      }
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
    game.changeParticleColour(bgColor);
    Destroy(gameObject);
    Instantiate(death, transform.position, transform.rotation);
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
    Debug.Log("SWIPE || dir: " + dirStatus + ", time: " + timeStatus + ", result = " + status + " -- " + Time.frameCount);
  }

  public void changeTimeState(string state)
  {
    timeStatus = state;
  }

  IEnumerator FadeIn()
  {
    gameObject.LeanAlpha(1, .75f);
    for (int i = 0; i < 1; i++)
    {
      yield return new WaitForSeconds(.75f);
    }
    //   float opPerMS = 10 / (float)(duration / (totalFrame / spawnFrame));
    //   for (float alpha = 0; alpha <= 1; alpha += opPerMS)
    //   {
    //     foreach (Renderer renderer in renderers)
    //     {
    //       Color c = renderer.material.color;
    //       c.a = alpha;
    //       renderer.material.color = c;
    //     }
    //     yield return new WaitForSeconds(.001f);
    //   }
    // }
  }
}
