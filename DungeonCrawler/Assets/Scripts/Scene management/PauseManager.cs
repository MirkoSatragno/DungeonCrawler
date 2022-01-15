using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{

    public delegate void PauseGameDelegate();
    public static PauseGameDelegate PauseGame;
    public delegate void EndGameDelegate(bool victory);
    public static EndGameDelegate EndGame;

    [SerializeReference, Tooltip("Canvas displayed during pause")]
    private GameObject pauseCanvas;
    [SerializeReference, Tooltip("Canvas displayed when game ends")]
    private GameObject endGameCanvas;
    [SerializeReference, Tooltip("Victory image displayed in endCanvas")]
    private Image victoryImage;
    [SerializeReference, Tooltip("Losss image displayed in endCanvas")]
    private Image lossImage;

    private void Awake()
    {
        Debug.AssertFormat(pauseCanvas, "PauseManager: pauseCanvas reference not found");

        Debug.Assert(endGameCanvas, "PauseManager: endGameCanvas reference not found");

        Debug.Assert(victoryImage && lossImage, "PauseManager: image reference not found");
    }

    private void OnEnable()
    {
        PauseGame += onPauseButtonPressed;
        EndGame += onEndGame;
    }

    private void Start()
    {
        if (pauseCanvas.activeSelf)
            pauseCanvas.SetActive(false);
        if (endGameCanvas.activeSelf)
            endGameCanvas.SetActive(false);
    }

    private void OnDisable()
    {
        PauseGame -= onPauseButtonPressed;
        EndGame -= onEndGame;
    }

    public void onPauseButtonPressed()
    {
        pauseCanvas.SetActive(true);
        GameManager.Instance.SetGamePaused(true);
    }

    public void onEndGame(bool victory)
    {
        endGameCanvas.SetActive(true);
        victoryImage.gameObject.SetActive(victory);
        lossImage.gameObject.SetActive(!victory);
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

    public void onReplayButtonPressed()
    {
        GameManager.Instance.SetGamePaused(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
