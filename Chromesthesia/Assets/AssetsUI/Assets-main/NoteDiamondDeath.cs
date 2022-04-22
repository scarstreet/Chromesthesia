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
  public void displayNextState()
  {
    Destroy(gameObject);
    transform.eulerAngles = new Vector3(0, 0, 0);
    Instantiate(nextAnimation, transform.position, transform.rotation);
  }
}
