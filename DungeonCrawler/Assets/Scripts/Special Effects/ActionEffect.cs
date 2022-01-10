using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionEffect : SpecialEffect
{
    override public void onEndAnimation()
    {
        Character.EndAttack(LevelManager.Instance.turnManager.ActiveCharacterIdCurrentTurn());

        base.onEndAnimation();
    }
}
