using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void onLevel1ButtonPushed()
    {
        SceneManager.LoadScene(GameManager.LEVEL_NAME_LEVEL_1);
    }

    public void onLevel2ButtonPushed()
    {
        SceneManager.LoadScene(GameManager.LEVEL_NAME_LEVEL_2);
    }

    public void onExitButtonPushed()
    {
        Debug.Log("Exit");
        Application.Quit();
    }
}
