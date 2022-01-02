using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    
    
    public GameObject pauseCanvas;

    private void Awake()
    {
        Debug.AssertFormat(pauseCanvas, "PauseManager: pauseCanvas parameter is missing");
    }

    private void Start()
    {
        if (pauseCanvas.activeSelf)
            pauseCanvas.SetActive(false);
    }
    public void onPauseButtonPressed()
    {
        pauseCanvas.SetActive(true);
        GameManager.Instance.SetGamePaused(true);
    }

    public void onResumeButtonPressed()
    {
        pauseCanvas.SetActive(false);
        GameManager.Instance.SetGamePaused(false);
    }

    public void onQuitButtonPressed()
    {
        GameManager.Instance.SetGamePaused(true);
        SceneManager.LoadScene(GameManager.Instance.LEVEL_NAME_MAIN_MENU);
    }
}
