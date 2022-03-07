using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteDiamond : MonoBehaviour
{
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
  private string status; // ''/'miss'/'good'/'perfect'
  // Start is called before the first frame update
  void Start()
  {
    status = "noInput";
    deathScript = death.GetComponent<NoteDiamondDeath>();
    renderers = GetComponentsInChildren<Renderer>();
    animator = gameObject.GetComponent<Animator>();
    StartCoroutine(FadeIn());
    float speed = (float)(duration / originalDuration); // TODO - make sure the inputted value good.
    animator.speed = 1 / speed;
  }
  
  public void setState(string state)
  {
    NoteDiamondResult resultScript;
    status = state;
    Debug.Log(state);
    if (status == "miss" || status=="noInput")
    {
      resultScript = miss.GetComponent<NoteDiamondResult>();
      resultScript.nextColor = new Color((255f / 255f), (100f / 255f), (100f / 255f), 1); // sum light red
      deathScript.nextColor = new Color((255f / 255f), (100f / 255f), (100f / 255f), 1); // sum light red
      deathScript.nextAnimation = miss;
      if(status=="noInput"){
        // Debug.Log(gameObject + "!!! Dequeueing !!!");
        GameMaster.touchable.Dequeue();
      }
    }
    else if (status == "good")
    {
      resultScript = good.GetComponent<NoteDiamondResult>();
      resultScript.nextColor = new Color((110f / 255f), (225f / 255f), (255f / 255f), 1); // sum light blu
      deathScript.nextColor = new Color((110f / 255f), (225f / 255f), (255f / 255f), 1); // sum light blu
      deathScript.nextAnimation = good;
    }
    else if (status == "perfect")
    {
      resultScript = perfect.GetComponent<NoteDiamondResult>();
      resultScript.nextColor = new Color((255f / 255f), (230f / 255f), (100f / 255f), 1); // sum light yello
      deathScript.nextColor = new Color((255f / 255f), (230f / 255f), (100f / 255f), 1); // sum light yello
      deathScript.nextAnimation = perfect;
      // GameMaster.touchable.Dequeue();
    }
    else
    {
      Debug.Log("omegeh, Note has bad status. This should not happen");
    }
    Destroy(gameObject);
    Instantiate(death, transform.position, transform.rotation);
  }
  
  private void displayResult(string state)
  {
    status = state;
  }

  IEnumerator FadeIn()
  {
    float opPerMS = 5 / (float)(duration / (totalFrame / spawnFrame));
    for (float alpha = 0; alpha < 1; alpha += opPerMS)
    {
      foreach (Renderer renderer in renderers)
      {
        Color c = renderer.material.color;
        c.a = alpha;
        renderer.material.color = c;
      }
      yield return new WaitForSeconds(.001f);
    }
  }

}
