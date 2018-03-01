using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseGame : MonoBehaviour {
    public GameObject pauseMenu;
    public GameObject controlsMenu;

    //private PlayerController player;
    private bool paused;

    void Awake()
    {
        pauseMenu.SetActive(false);
        controlsMenu.SetActive(false);
        paused = false;
    }

    // Update is called once per frame
    void Update() {
	    if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }
	}

    public void Pause()
    {
        if (!paused)
        {
            paused = true;
            pauseMenu.SetActive(true);
            Time.timeScale = 0;
        }
        else
        {
            paused = false;
            pauseMenu.SetActive(false);
            controlsMenu.SetActive(false);
            Time.timeScale = 1;
        }
    }

    public void SwapToControlsMenu()
    {
        pauseMenu.SetActive(false);
        controlsMenu.SetActive(true);
    }

    public void SwapToPauseMenu()
    {
        pauseMenu.SetActive(true);
        controlsMenu.SetActive(false);
    }
}
