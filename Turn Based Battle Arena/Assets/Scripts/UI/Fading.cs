using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fading : MonoBehaviour {
    public Texture2D fadeOutTexture;
    public float sceneFadeSpeed;

    private int drawDepth = -1000;
    private float alpha = 1.0f;
    private int fadeDirection = -1;

    public float audioFadeSpeed;
    private int audioFadeDirection = -1;
    private float volume = 1.0f;

    private void OnGUI()
    {
        alpha += fadeDirection * sceneFadeSpeed * Time.deltaTime;
        alpha = Mathf.Clamp01(alpha);

        volume -= audioFadeDirection * audioFadeSpeed * Time.deltaTime;
        volume = Mathf.Clamp01(volume);

        GetComponent<AudioSource>().volume = volume;

        GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha);
        GUI.depth = drawDepth;
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeOutTexture);
    }

    public float BeginSceneFade(int direction)
    {
        fadeDirection = direction;
        return (sceneFadeSpeed);
    }

    public void BeginAudioFade(int direction)
    {
        audioFadeDirection = direction;
    }

    void OnLevelLoaded()
    {
        BeginSceneFade(-1);
    }
}
