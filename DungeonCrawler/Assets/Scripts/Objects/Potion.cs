using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : Item
{
    [SerializeField]
    private int _healingAmount = 20;
    public static int HealingAmount;
    

    private void Awake()
    {
        HealingAmount = _healingAmount;
    }


}
