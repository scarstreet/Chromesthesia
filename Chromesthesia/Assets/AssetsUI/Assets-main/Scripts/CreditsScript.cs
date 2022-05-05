using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
public class CreditsScript : MonoBehaviour
{
    public Image transitionPanel;
    // Start is called before the first frame update
    void Start()
    {
        transitionPanel.CrossFadeAlpha(0, 0.5f, false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void back()
    {
        Debug.Log("Pressed");
        StartCoroutine(changeScene("MainTitleScreen"));
    }
    IEnumerator changeScene(string scene)
    {
        bool fadeDone = false;
        while (!fadeDone)
        {
            transitionPanel.CrossFadeAlpha(1, 0.5f, true);
            fadeDone = true;
            yield return new WaitForSecondsRealtime(.5f);
        }
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }
}
