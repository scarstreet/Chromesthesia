using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteDiamondDeath : MonoBehaviour
{
  public GameObject nextAnimation;
  public Color nextColor;
  void Start()
  {
    gameObject.LeanColor(nextColor, 0.5f).setEaseOutQuad();
  }

  public void displayNextState()
  {
    Destroy(gameObject);
    transform.eulerAngles = new Vector3(0, 0, 0);
    Instantiate(nextAnimation, transform.position, transform.rotation);
  }
}
