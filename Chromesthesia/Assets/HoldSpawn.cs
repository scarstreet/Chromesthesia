using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldSpawn : MonoBehaviour
{
  // Start is called before the first frame update
  public Color bgColor;
  public GameObject HoldStart;
  public GameObject HoldSpawnDeath;
  Renderer[] renderers;
  Animator animator;
  public double duration = 2500;
  private double originalDuration = 2500;
  private float totalFrame = 150;
  private float spawnFrame = 40;
  public double holdDuration = 5000;
  void Start()
  {
    renderers = GetComponentsInChildren<Renderer>();
    animator = gameObject.GetComponent<Animator>();
    StartCoroutine(FadeIn());
    float speed = (float)(duration / originalDuration); // TODO - make sure the inputted value good.
    animator.speed = 1 / speed;
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
    else
    {
      gameObject.LeanMoveZ(0, 0f).setIgnoreTimeScale(true);
    }
  }
  void Awake()
  {
    DontDestroyOnLoad(gameObject);
  }
    public void setState(string state)
  {
    if (state.Contains("noInput"))
    {
      HoldDeath script = HoldSpawnDeath.GetComponent<HoldDeath>();
      script.nextState = "noInput";
      script.nextColor = new Color((255f / 255f), (100f / 255f), (100f / 255f), 1); // sum light red
      
      Destroy(gameObject);
      Instantiate(HoldSpawnDeath, transform.position, transform.rotation);
    }
    else
    {
      GameScript.self.GetComponent<GameScript>().changeParticleColour(bgColor);
      Destroy(gameObject);
    }
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
