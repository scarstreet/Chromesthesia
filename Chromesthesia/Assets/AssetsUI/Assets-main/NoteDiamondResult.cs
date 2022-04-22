using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteDiamondResult : MonoBehaviour
{
  // Start is called before the first frame update
  public GameObject again;
  public Color nextColor;
  void Start()
  {
    gameObject.LeanColor(nextColor, 0f).setEaseOutQuad();
    StartCoroutine(FlickerOut());
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
    if (GameScript.gameIsPaused == true)
    {
      gameObject.LeanMoveZ(-100, 0f).setIgnoreTimeScale(true);
    }
    else
    {
      gameObject.LeanMoveZ(0, 0f).setIgnoreTimeScale(true);
    }
  }
  IEnumerator FlickerOut()
  {
    bool wait = true;
    while (wait)
    {
      wait = false;
      yield return new WaitForSeconds(.75f);
    }
    gameObject.LeanAlpha(0, 0.09f).setEaseInOutBounce().setLoopPingPong();
  }
  public void EndNote()
  {
    Destroy(gameObject);
    // Instantiate(again, transform.position, transform.rotation);
  }
}
