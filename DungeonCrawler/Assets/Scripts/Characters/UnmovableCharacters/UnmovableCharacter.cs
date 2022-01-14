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
                Die();
                

        }
    }

    public override IEnumerator PlayTurn()
    {
        StartCoroutine("SetupStartingTurnUI");

        
        CurrentState = CharacterState.Idle;

        attackedCharacter = LevelManager.FindTaggedObjectAround<PlayerCharacter>(GameManager.TAG_PLAYER, transform.position, boxCollider.size);
        if (attackedCharacter)
        {
            yield return new WaitForSeconds(preTurnWait);
            Attack(attackedCharacter);
        }
        else
            StartCoroutine("EndMyTurn");
        
    }

    override public IEnumerator EndMyTurn()
    {
        //better wait before the possible death wait, to be sure the player has already entered dieing state
        if (attackedCharacter)
        {
            yield return new WaitForSeconds(postTurnWait);

            if (attackedCharacter.CurrentState == CharacterState.Dieing)
                yield return new WaitForSeconds(attackedCharacter.disappearingDuration);
        }
        

        DisableTurnUI();
        TurnManager.EndTurn(CharacterId);
    }

    protected override void Die()
    {
        base.Die();

        PauseManager.EndGame(true);
    }

}
