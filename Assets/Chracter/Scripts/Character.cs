using UnityEngine;

public abstract class Character : MonoBehaviour
{
    [Header("Common State")]
    protected float currentHp;
    protected float maxHp;
    protected float moveSpeed;
    protected float attackDamage;
    protected float attackSpeed;

    protected bool isDead = false;

    public virtual void InitializeCharacter(CharacterStateDataContainer.CommonStats baseStats)
    {
        maxHp = baseStats.maxHp;
        currentHp = maxHp;
        moveSpeed = baseStats.moveSpeed;
        attackDamage = baseStats.attackDamage;
        attackSpeed = baseStats.attackSpeed;
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
