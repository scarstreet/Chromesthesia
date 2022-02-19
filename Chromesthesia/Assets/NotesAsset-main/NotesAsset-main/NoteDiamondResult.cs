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
