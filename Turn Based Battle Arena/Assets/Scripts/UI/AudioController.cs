using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour {

    public AudioClip menuSelect;
    public AudioClip menuMove;
    public AudioClip menuBack;

    public void playMenuSelect()
    {
        GetComponent<AudioSource>().clip = menuSelect;
        GetComponent<AudioSource>().Play();
    }

    public void playMenuMove()
    {
        GetComponent<AudioSource>().clip = menuMove;
        GetComponent<AudioSource>().Play();
    }

    public void playMenuBack()
    {
        GetComponent<AudioSource>().clip = menuBack;
        GetComponent<AudioSource>().Play();
    }
}
