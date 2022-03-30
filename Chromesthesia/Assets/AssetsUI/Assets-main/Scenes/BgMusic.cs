using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgMusic : MonoBehaviour
{
    private static BgMusic BackgroundMusic;
    // Start is called before the first frame update
    void Awake()
    {
        if (BackgroundMusic == null)
        {
            BackgroundMusic = this;
            DontDestroyOnLoad(BackgroundMusic);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
