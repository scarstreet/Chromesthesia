using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GradientScript : MonoBehaviour
{
  // Start is called before the first frame update
  Image panel;
  public Animator animator;
  public AudioSource audioSource;
  public float updateStep = 0f;
  public int sampleDataLength = 1024;
  private float currentUpdateTime = 0f;
  public float clipLoudness;
  private float[] clipSampleData;
  public float sizeFactor = 1;
  //   public float minSize = 0;
  //   public float maxSize = 500;

  void Start()
  {
    panel = gameObject.GetComponent<Image>();
    clipSampleData = new float[sampleDataLength];
  }
  void Awake()
  {
    DontDestroyOnLoad(gameObject);
  }
  // Update is called once per frame
  void Update()
  {
    currentUpdateTime += Time.deltaTime;
    if (currentUpdateTime >= updateStep)
    {
      currentUpdateTime = 0f;
      audioSource.clip.GetData(clipSampleData, audioSource.timeSamples); //I read 1024 samples, which is about 80 ms on a 44khz stereo clip, beginning at the current sample position of the clip.
      clipLoudness = 0f;
      float[] testData = new float[4];
      int count = 0;
      foreach (var sample in clipSampleData)
      {
        clipLoudness += Mathf.Abs(sample);
        if (count < 256)
        {
          testData[0] += Mathf.Abs(sample);
        }
        if (count < 512)
        {
          testData[1] += Mathf.Abs(sample);
        }
        if (count < 768)
        {
          testData[2] += Mathf.Abs(sample);
        }
        if (count < 1024)
        {
          testData[3] += Mathf.Abs(sample);
        }
        count += 1;
      }
      // Debug.Log($"{(int)testData[0]} {(int)testData[1]} {(int)testData[3]} {(int)testData[2]}");
      clipLoudness /= sampleDataLength; //clipLoudness is what you are looking for
      clipLoudness *= sizeFactor;
      // clipLoudness = Mathf.Clamp(clipLoudness, 0.9996761f, 10f);
      clipLoudness = Mathf.Clamp(clipLoudness, 0f, .3f);
      clipLoudness *= 2;
      if(clipLoudness > 1){
        clipLoudness = 1;
      }
      animator.SetFloat("loudness", clipLoudness);
    }
  }
}
