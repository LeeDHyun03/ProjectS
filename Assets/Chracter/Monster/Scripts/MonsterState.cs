using UnityEngine;

public class MonsterState : State
{
    MonsterMovement movement => GetComponent<MonsterMovement>();
    public MonsterType monsterType;

    public override void Awake()
    {
        base.Awake();
    }
    public override void Start()
    {
        base.Start();
        Debug.Log("내 타입: "+ MonsterManager.inst.SetMonsterType(0).name);
        myType = MonsterManager.inst.SetMonsterType(0); //임시
        monsterType = (MonsterType)myType;
    }
    public override void Dead()
    {
        throw new System.NotImplementedException();
    }
    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);

        movement.OnHitReaction();
    }
}