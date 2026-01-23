using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class EntityType
{
    public float maxHp;
    public float attackDamage;
    public float attackRange;
    public float attackDelay;
    public float moveSpeed;
}
[System.Serializable]
public class PlayerType : EntityType
{

}
public abstract class State : MonoBehaviour
{
    protected EntityType myType;
    protected float currentHp;
    public bool isDead = false;

    public virtual void Awake()
    {
        currentHp = myType.maxHp;
    }
    public abstract void Dead();
    public virtual void TakeDamage(float damage)
    {
        currentHp -= damage;

        if(currentHp >= 0)
        {
            isDead = true;
            Dead();
        }
    }
}