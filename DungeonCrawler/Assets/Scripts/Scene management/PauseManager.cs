using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{

    public delegate void PauseGameDelegate();
    public static PauseGameDelegate PauseGame;

    public GameObject pauseCanvas;

    private void Awake()
    {
        Debug.AssertFormat(pauseCanvas, "PauseManager: pauseCanvas parameter is missing");
    }

    private void OnEnable()
    {
        PauseGame += onPauseButtonPressed;
    }

    private void Start()
    {
        if (pauseCanvas.activeSelf)
            pauseCanvas.SetActive(false);
    }

    private void OnDisable()
    {
        PauseGame -= onPauseButtonPressed;
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

    public void onSaveButtonPressed()
    {
        //TO DO
        Debug.Log("Save");
    }

    public void onLoadButtonPressed()
    {
        //TO DO
        Debug.Log("Load");
    }

    public void onQuitButtonPressed()
    {
        GameManager.Instance.SetGamePaused(false);
        SceneManager.LoadScene(GameManager.LEVEL_NAME_MAIN_MENU);
    }
}
