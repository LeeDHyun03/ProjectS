using System;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public enum AttackType
    {
        NormalAttacked = 0,
        SpecialAttacked = 1
    }

    [SerializeField] private Weapon normalWeapon;
    [SerializeField] private SpecialWeapon specialWeapon;
    [SerializeField] private FlameOnItem flameOnItem;
    [SerializeField] private ElectricItem electricItem;
    [SerializeField] private StigmaItem stigmaItem;

    public event Action<Vector2> NormalAttackTriggered;
    public event Action<Vector2> SpecialAttackTriggered;

    public event Action<Monster, AttackType> OnAttackSuccessed;

    [SerializeField] private bool isNormalAttacking = false;
    [SerializeField] private bool isSpecialAttacking = false;

    private float attackSpeed = 100f;

    public float AttackSpeed => attackSpeed;

    void OnEnable()
    {
        normalWeapon.OnAttackSuccessed += SuccessedNormalAttack;
        specialWeapon.OnAttackSuccessed += SuccessedSpecialAttack;
        specialWeapon.SpecialAttackTriggered += OnSpecialAttackDir;
        specialWeapon.OnSpecialAttackComplete += OnSpecialAttackAnimationComplete;
    }

    void OnDisable()
    {
        specialWeapon.SpecialAttackTriggered -= OnSpecialAttackDir;
        specialWeapon.OnSpecialAttackComplete -= OnSpecialAttackAnimationComplete;
    }

    public void SetStats(float newAttackDamage, float newAttackSpeed, float critChance, float critDamage, float newMeleeDamage, float newRangedDamage, float newAnger, float newPride, float newJealousy)
    {
        normalWeapon.SetAttackDamage(newAttackDamage, critChance, critDamage, newMeleeDamage);
        specialWeapon.SetAttackDamage(newAttackDamage, critChance, critDamage, newRangedDamage);
        electricItem.SetJealousy(newJealousy);
        stigmaItem.SetPride(newPride);
        flameOnItem.SetAnger(newAnger);
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

    private void SuccessedNormalAttack(Monster monster)
    {
        OnAttackSuccessed?.Invoke(monster, AttackType.NormalAttacked);
    }

    private void SuccessedSpecialAttack(Monster monster)
    {
        OnAttackSuccessed?.Invoke(monster, AttackType.SpecialAttacked);
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
