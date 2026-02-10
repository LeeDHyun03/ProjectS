using UnityEngine;

public abstract class Character : MonoBehaviour
{
    [Header("Common State")]
    protected float currentHp;
    protected float maxHp;
    protected float moveSpeed;
    protected float defaultMoveSpeed;
    protected float attackDamage;
    protected float defaultAttackDamage;
    protected float normalAttackSpeed;

    protected bool isDead = false;

    public bool IsDead => isDead;
    public float GetCurrentHp => currentHp;
    public float GetMaxHp => maxHp;
    public float GetAttackDamage => attackDamage;
    public float SetAttackDamage(float damage) => attackDamage = damage;
    public float ResetAttackDamage() => attackDamage = defaultAttackDamage; 
    public float GetMoveSpeed => moveSpeed;
    public float SetMoveSpeed(float speed) => moveSpeed = speed;
    public float ResetMoveSpeed() => moveSpeed = defaultMoveSpeed;
    public virtual void InitializeCharacter(CharacterStateDataContainer.CommonStats baseStats)
    {
        maxHp = baseStats.maxHp;
        currentHp = maxHp;
        defaultMoveSpeed = baseStats.moveSpeed;
        moveSpeed = defaultMoveSpeed;
        defaultAttackDamage = baseStats.attackDamage;
        attackDamage = defaultAttackDamage;
        normalAttackSpeed = baseStats.attackSpeed;
    }

    public virtual void TakeDamage(float damage)
    {
        currentHp -= damage;
        if (currentHp <= 0)
        {
            currentHp = 0;
            isDead = true;
            Dead();
        }
    }

    public abstract void Dead();
}
