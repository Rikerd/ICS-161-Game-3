using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadSceneOnClick : MonoBehaviour {
    private Fading fade;

    void Awake()
    {
        fade = GameObject.Find("GameManager").GetComponent<Fading>();
    }

    public void LoadByIndex(int sceneIndex)
    {
        GetComponent<Button>().interactable = false;
        StartCoroutine(FadeWait(sceneIndex));
    }

    IEnumerator FadeWait(int sceneIndex)
    {
        float fadeTime = fade.BeginSceneFade(1);
        fade.BeginAudioFade(1);
        Time.timeScale = 1;
        yield return new WaitForSeconds(fadeTime);
        SceneManager.LoadScene(sceneIndex);
    }
}
