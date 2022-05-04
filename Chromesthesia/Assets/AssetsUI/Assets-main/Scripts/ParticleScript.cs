using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleScript : MonoBehaviour
{
  ParticleSystem ps;
  // Start is called before the first frame update
  void Start()
  {
    ps = gameObject.GetComponent<ParticleSystem>();
  }
  void Awake()
  {
    DontDestroyOnLoad(gameObject);
  }
  void Update()
  {
    if (PauseScript.pauseOpen == true) {
      gameObject.LeanMoveZ(-100, 0f).setIgnoreTimeScale(true);
      ps.Pause(true);
      ps.Stop(true);
    } else if (GameScript.gameStarted && PauseScript.pauseOpen == false || GameScript.gameStarted){
      gameObject.LeanMoveZ(1, 0f).setIgnoreTimeScale(true);
      ps.Pause(false);
      ps.Play(true);
    } else {
      
    }
    if (GameScript.gameCompleted == true)
    {
      Destroy(gameObject);
    }
  }
}
