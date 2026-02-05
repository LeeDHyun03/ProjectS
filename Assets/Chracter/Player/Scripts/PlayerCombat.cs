using System;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private Weapon normalWeapon;
    [SerializeField] private SpecialWeapon specialWeapon;

    public event Action<Vector2> NormalAttackTriggered;
    public event Action<Vector2> SpecialAttackTriggered;

    [SerializeField] private bool isNormalAttacking = false;
    [SerializeField] private bool isSpecialAttacking = false;

    private float attackSpeed = 100f;

    public float AttackSpeed => attackSpeed;

    void OnEnable()
    {
        specialWeapon.SpecialAttackTriggered += OnSpecialAttackDir;
        specialWeapon.OnSpecialAttackComplete += OnSpecialAttackAnimationComplete;
    }

    void OnDisable()
    {
        specialWeapon.SpecialAttackTriggered -= OnSpecialAttackDir;
        specialWeapon.OnSpecialAttackComplete -= OnSpecialAttackAnimationComplete;
    }

    public void SetStats(float newAttackDamage, float newAttackSpeed)
    {
        normalWeapon.SetAttackDamage(newAttackDamage);
        specialWeapon.SetAttackDamage(newAttackDamage);
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
        if(isNormalAttacking || isSpecialAttacking)
            return;
        isSpecialAttacking = true;
        specialWeapon.EnableSpecialAttackCollider();
    }

    public void OnSpecialAttackAnimationComplete()
    {
        isSpecialAttacking = false;
    }

    private void OnSpecialAttackDir(Vector2 attackDir)
    {
        SpecialAttackTriggered?.Invoke(attackDir);
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
