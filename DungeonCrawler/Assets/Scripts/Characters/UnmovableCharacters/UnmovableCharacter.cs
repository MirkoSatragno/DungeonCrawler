using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnmovableCharacter : Character
{
    // Start is called before the first frame update
    void Start()
    {
        
    }


    public override IEnumerator PlayTurn()
    {
        StartCoroutine("SetupStartingTurnUI");

        yield return new WaitForSeconds(preTurnWait);
        CurrentState = CharacterState.Idle;

        attackedCharacter = FindPlayerAround();
        if (attackedCharacter)
            Attack(attackedCharacter);
        else
            StartCoroutine("EndMyTurn");
        
    }

}
