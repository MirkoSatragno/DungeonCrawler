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
        if (IsMyTurn() && turnUiLoaded && Input.GetMouseButtonDown(0))
        {
            EndMyTurn();
        }
    }

    public override IEnumerator PlayTurn()
    {
        Debug.Log(this.name + "'s turn");
        StartCoroutine("SetupStartingTurnUI");

        //TO DO remove the wait
        yield return new WaitForSeconds(1);
        turnUiLoaded = true;

        
        Debug.Log("Player wants to move");

    }
    
    override protected void SetupStartingTurnUI()
    {
        turnCircle.SetActive(true);

        Debug.Log("Player needs additional UI");
    }
}
