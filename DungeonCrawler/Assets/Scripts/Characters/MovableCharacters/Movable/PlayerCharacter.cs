using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MovableCharacter
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

        //TO DO remove the wait
        yield return new WaitForSeconds(preTurnWait);
        CurrentState = CharacterState.Idle;

    }
    
    override protected void SetupStartingTurnUI()
    {
        turnCircle.SetActive(true);

        Debug.Log("Player needs additional UI");
    }
}
