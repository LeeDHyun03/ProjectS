using UnityEngine;
using System.Collections.Generic;

public class MonsterState : MonoBehaviour
{
    MonsterType myType;
    CombatStat myCombatStat;

    float currentHp;
    private void Awake()
    {
        currentHp = myType.maxHp;
        myCombatStat = MonsterManager.inst.CombatStatList[(int)myType.combatType];
    }
    public bool isDie() =>
        currentHp >= 0;
}