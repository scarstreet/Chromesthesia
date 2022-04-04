using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountDown : MonoBehaviour
{
  Animator animator;
  // Start is called before the first frame update
  void Start()
  {
    animator = gameObject.GetComponent<Animator>();
    animator.speed = 1;
  }

  // Update is called once per frame
  void Update()
  {

  }
  public void ihms()
  {
    Destroy(gameObject);
  }
}
