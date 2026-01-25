using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class EntityType
{
    public string name;
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
    [SerializeField]protected EntityType myType;
    protected float currentHp;
    public bool isDead = false;

    public virtual void Awake()
    {
    }
    public virtual void Start()
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
    public float GetCurrentHp()
    {
        return currentHp;
    }
}