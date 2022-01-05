using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialEffect : MonoBehaviour
{
    
    public void onEndAnimation()
    {
        Character.EndAttack(LevelManager.Instance.turnManager.ActiveCharacterIdCurrentTurn());

        Destroy(this.gameObject);
    }
}
