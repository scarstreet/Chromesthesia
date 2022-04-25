using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountDown : MonoBehaviour
{
  Animator animator;
  // Start is called before the first frame update
  void Start()
  {
    gameObject.LeanMoveZ(100, 0f);
    animator = gameObject.GetComponent<Animator>();
    animator.speed = 1;
  }
  void Awake()
  {
    DontDestroyOnLoad(gameObject);
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
