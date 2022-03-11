using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldWait : MonoBehaviour
{
  // Start is called before the first frame update
  public double holdDuration = 5000;
  private double oriHoldDuration = 5000;
  public string timeStatus = "noInput";
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
    deathScript = death.GetComponent<NoteDiamondDeath>();
    animator = gameObject.GetComponent<Animator>();
  }

  // Update is called once per frame
  void Update()
  {

  }

  void changeDuration()
  {
    float speed = (float)(holdDuration / oriHoldDuration);
    animator.speed = 1 / speed;
  }

  public void setState(string dirStatus)
  {
    NoteDiamondResult resultScript;
    statusDetermine(dirStatus);
    if (dirStatus.Contains("noInput") || dirStatus.Contains("tooEarly") || status.Contains("miss"))
    {
      StartCoroutine(FlickerOut());
      resultScript = miss.GetComponent<NoteDiamondResult>();
      resultScript.nextColor = new Color((255f / 255f), (100f / 255f), (100f / 255f), 1); // sum light red
      deathScript.nextColor = new Color((255f / 255f), (100f / 255f), (100f / 255f), 1); // sum light red
      deathScript.nextAnimation = miss;
      if (dirStatus.Contains("noInput"))
      {
        GameMaster.touchable.Dequeue();
      }
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
