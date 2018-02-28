using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuitOnClick : MonoBehaviour {

    public void Quit()
    {
#if UNITY_EDITOR
        GetComponent<Button>().interactable = false;
        UnityEditor.EditorApplication.isPlaying = false;
#else
        GetComponent<Button>().interactable = false;
        Application.Quit();
#endif
    }

}
