using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCharacter : MovableCharacter
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (IsMyTurn() && CurrentState == CharacterState.Idle && Input.GetMouseButtonDown(0))
        {
            StartCoroutine("EndMyTurn");
        }
    }

    public override IEnumerator PlayTurn()
    {
        StartCoroutine("SetupStartingTurnUI");

        yield return new WaitForSeconds(preTurnWait);
        CurrentState = CharacterState.Idle;

        Character player = FindPlayerAround();
        if (player)
        {
            Attack(player);
            
            yield return new WaitForSeconds(1);
        }
        else
        {
            //Debug.Log("Enemy wants to move");
            //Se non trovi un player entro un certo raggio, fallo muovere a caso
        }

        

    }

}
