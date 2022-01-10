using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnmovableCharacter : Character
{
    // Start is called before the first frame update

    void Update()
    {
        if (CurrentState == CharacterState.Dieing)
        {
            Color spriteColor = sprite.color;
            spriteColor.a = Mathf.Max(0f, spriteColor.a - Time.deltaTime / disappearingDuration);
            sprite.color = spriteColor;

            if (spriteColor.a == 0)
                Destroy(gameObject);

        }
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
