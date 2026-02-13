using UnityEngine;

public abstract class MageProjectileAttack : MonsterAttack
{
    [SerializeField, Min(1)] protected int actionCount = 1;
    [SerializeField] protected float radius = 3f;

    protected Vector3 targetPos;
    
    public void SetTargetPosition(Vector3 pos) => targetPos = pos;

    public void SetAttackIndicatorPosition(Vector3 pos)
    {
        targetPos = pos;

        Vector2 circleSize = new Vector2(radius * 2f, radius * 2f);
        var indicator = ActivateAttackIndicator(IndicatorShape.Circle, pos, Vector3.zero);

        if (indicator != null)
        {
            indicator.OnIndicatorComplete += BroadcastOnStartedAttack;
            indicator.StartIndicator(circleSize, attackSpeed);
        }

    }

    public override void Attack()
    {
        // »ç¿îµå
    }

    protected abstract void DoAction(Vector3 indicatorCenterPos);
}
