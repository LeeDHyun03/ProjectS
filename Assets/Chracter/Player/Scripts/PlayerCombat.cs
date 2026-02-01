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

    private float attackDamage = 10f;
    private float attackSpeed = 100f;

    public float AttackSpeed => attackSpeed;

    void OnEnable()
    {
        specialWeapon.OnSpecialAttackComplete += OnSpecialAttackAnimationComplete;
    }

    void OnDisable()
    {
        specialWeapon.OnSpecialAttackComplete -= OnSpecialAttackAnimationComplete;
    }

    public void SetStats(float newAttackDamage, float newAttackSpeed)
    {
        attackDamage = newAttackDamage;
        attackSpeed = newAttackSpeed;
    }

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
        specialWeapon.EnableSpecialAttackCollider();
        SpecialAttackTriggered?.Invoke();
    }

    public void OnSpecialAttackAnimationComplete()
    {
        isSpecialAttacking = false;
    }

    public void EnableRestMode()
    {
        isNormalAttacking = true;
        normalWeapon.StartAttack();
    }

    public void DisableRestMode()
    {
        isNormalAttacking = false;
        normalWeapon.EndAttack();
    }
}
