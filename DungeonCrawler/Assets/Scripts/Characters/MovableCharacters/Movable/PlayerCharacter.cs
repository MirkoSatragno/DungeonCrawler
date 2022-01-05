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

        if(CurrentState == CharacterState.Dieing)
        {
            Color spriteColor = sprite.color;
            spriteColor.a =  Mathf.Max(0f, spriteColor.a - Time.deltaTime / disappearingDuration);
            sprite.color = spriteColor;

            if (spriteColor.a == 0)
            {
                Debug.Log("PUFF");
                Destroy(gameObject);
            }
                
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

    override protected void Die() 
    {
        CurrentState = CharacterState.Dieing;

        LevelManager.Instance.RemoveCharacter(CharacterId);
    }
}
