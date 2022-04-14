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
    if (GameScript.gameIsPaused == true) {
      ps.Pause(true);
      ps.Stop(true);
    } else {
      ps.Pause(false);
      ps.Play(true);
    }
    if (GameScript.gameCompleted == true)
    {
      Destroy(gameObject);
    }
  }
}
