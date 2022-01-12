using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableCharacter : Character
{
    [SerializeField]
    protected float movementDuration = 1f;

    protected Vector3 movementDestination;




    protected void MoveTo(Vector2 direction)
    {
        movementDestination = transform.position + (Vector3)direction;
        TurnToTarget(transform.position + (Vector3)direction);
        animator.SetFloat(ANIMATOR_PARAMETER_SPEED_NAME, ANIMATOR_PARAMETER_SPEED_INTERMEDIATE);
        CurrentState = CharacterState.Moving;
    }

    protected void EndMovement()
    {
        CurrentState = CharacterState.Spectating;
        animator.SetFloat(ANIMATOR_PARAMETER_SPEED_NAME, ANIMATOR_PARAMETER_SPEED_SLOW);
        StartCoroutine("EndMyTurn");
    }

    
}
