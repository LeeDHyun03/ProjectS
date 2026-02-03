using System;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private Weapon normalWeapon;
    [SerializeField] private SpecialWeapon specialWeapon;
    public event Action<Vector2> NormalAttackTriggered;
    public event Action SpecialAttackTriggered;

    [SerializeField] private bool isNormalAttacking = false;
    [SerializeField] private bool isSpecialAttacking = false;

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
        normalWeapon.SetAttackStat(newAttackDamage);
        attackSpeed = newAttackSpeed;
    }

    public void OnNormalAttack(Vector2 inputAttackDir)
    {
        if (isSpecialAttacking)
            return;
        isNormalAttacking = true;
        NormalAttackTriggered?.Invoke(inputAttackDir);
    }

    public void OnNormalAttackAnimationComplete()
    {
        isNormalAttacking = false;
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
        isSpecialAttacking = true;
    }

    public void DisableRestMode()
    {
        isNormalAttacking = false;
        isSpecialAttacking = false;
    }
}
