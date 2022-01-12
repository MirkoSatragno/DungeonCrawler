using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardPlayer : PlayerCharacter
{
    public delegate void EndPowerDelegate();
    public static EndPowerDelegate EndPower;


    override protected void OnEnable()
    {

        base.OnEnable();
        Obstacle.RemoveObstacle += PowerButtonPressed;
        EndPower += onEndPower;
    }

    override protected void OnDisable()
    {
        base.OnDisable();
        Obstacle.RemoveObstacle -= PowerButtonPressed;
        EndPower += onEndPower;
    }




    override protected void SetupStartingTurnUI()
    {
        base.SetupStartingTurnUI();
        playerTurnCanvas.showCorrectPowerButton();
    }

    public void PowerButtonPressed()
    {
        DisableInputTurnUI();
        CurrentState = CharacterState.Busy;
        animator.SetFloat(ANIMATOR_PARAMETER_SPEED_NAME, ANIMATOR_PARAMETER_SPEED_FAST);
    }

    public void onEndPower()
    {
        animator.SetFloat(ANIMATOR_PARAMETER_SPEED_NAME, ANIMATOR_PARAMETER_SPEED_SLOW);
        LevelManager.Instance.InstantiateBoss();
        StartCoroutine("EndMyTurn");
    }
}
