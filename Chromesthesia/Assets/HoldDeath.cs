using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldDeath : MonoBehaviour
{
  // Start is called before the first frame update
  public GameObject Perfect, Good, Miss;
  public Color nextColor;
  public string nextState = "";

  void Start()
  {
    gameObject.LeanColor(nextColor, 0.5f).setEaseOutQuad();
  }

  // Update is called once per frame
  void Update()
  {

  }
  public void displayNextState()
  {
    transform.eulerAngles = new Vector3(0, 0, 0);
    NoteDiamondResult script;
    GameObject Next;
    if (nextState.Contains("miss") || nextState.Contains("noInput"))
    {
      script = Miss.GetComponent<NoteDiamondResult>();
      script.nextColor = nextColor;
      Next = Miss;
    }
    else if (nextState.Contains("perfect"))
    {
      script = Perfect.GetComponent<NoteDiamondResult>();
      script.nextColor = nextColor;
      Next = Perfect;
    }
    else
    {
      script = Good.GetComponent<NoteDiamondResult>();
      script.nextColor = nextColor;
      Next = Good;
    }
    Destroy(gameObject);
    Instantiate(Next, transform.position, transform.rotation);
  }
}
