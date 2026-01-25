using System;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private Weapon normalWeapon;
    [SerializeField] private SpecialWeapon specialWeapon;

    public event Action NormalAttackTriggered;
    public event Action SpecialAttackTriggered;

    private bool isNormalAttacking = false;
    private bool isSpecialAttacking = false;

    public void OnNormalAttack()
    {
        if(isSpecialAttacking)
            return;
        isNormalAttacking = true;
        normalWeapon.StartAttack();
        NormalAttackTriggered?.Invoke();
    }
    public void OnNormalAttackAnimationComplete()
    {
        isNormalAttacking = false;
        normalWeapon.EndAttack();
    }

    public void OnSpecialAttack()
    {
        if(isNormalAttacking)
            return;
        isSpecialAttacking = true;
        SpecialAttackTriggered?.Invoke();
    }

    public void OnSpecialAttackAnimationComplete()
    {
        isSpecialAttacking = false;
    }
}
