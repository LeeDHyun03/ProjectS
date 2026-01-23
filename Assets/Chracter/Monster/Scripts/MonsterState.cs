public class MonsterState : State
{
    MonsterMovement movement => GetComponent<MonsterMovement>();
    public MonsterType monsterType => (MonsterType)myType;
    CombatStat myCombatStat;

    public override void Awake()
    {
        base.Awake();
        myCombatStat = MonsterManager.inst.CombatStatList[(int)monsterType.combatType];
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