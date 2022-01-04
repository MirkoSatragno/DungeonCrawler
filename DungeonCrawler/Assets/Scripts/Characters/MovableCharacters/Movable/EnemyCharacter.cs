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
        if (IsMyTurn() && turnUiLoaded && Input.GetMouseButtonDown(0))
        {
            EndMyTurn();
        }
    }

    public override IEnumerator PlayTurn()
    {
        Debug.Log(this.name + "'s turn");
        StartCoroutine("SetupStartingTurnUI");

        yield return new WaitForSeconds(1);
        turnUiLoaded = true;

        Character player = FindPlayerAround();
        if (player)
        {
            Attack(player);
            
            yield return new WaitForSeconds(1);
        }
        else
        {
            Debug.Log("Enemy wants to move");
        }

        

    }

}
