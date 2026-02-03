using UnityEngine;

public class ProjectileAttack : MonsterAttack
{
    [SerializeField] private string projectileTag;

    [SerializeField] private float lifeTime = 3f;
    [SerializeField] private float speed = 10f;
    [SerializeField] private float ArrowAttackIndicatorSizeY = 1f;

    public override void Attack()
    {
        ObjectPooler.Instance.SpawnFromPool(projectileTag, transform.position, Quaternion.identity)
            .TryGetComponent<Arrow>(out Arrow projectile);
        if (projectile == null) return;

        projectile.SetDefaultValue(dir, lifeTime, speed, attackDamage, isPlayerSide);
    }

    public void SetAttackIndicatorDirection()
    {
        AttackIndicator attackIndicator = ActivateAttackIndicator();

        attackIndicator.OnIndicatorComplete += BroadcastOnStartedAttack;

        float lastPosX = (speed * lifeTime);

        Vector2 attackIndicatorSize = new Vector2(lastPosX, ArrowAttackIndicatorSizeY);
        attackIndicator.StartIndicator(attackIndicatorSize, attackSpeed);
    }

    protected override AttackIndicator ActivateAttackIndicator()
    {
        ObjectPooler.Instance.SpawnFromPool("AttackIndicator", transform.position, Quaternion.Euler(Vector3.zero))
            .TryGetComponent<AttackIndicator>(out AttackIndicator attackIndicator);
        if (attackIndicator == null) return null;
        attackIndicator.transform.parent = this.transform;
        attackIndicator.transform.localPosition = Vector3.zero;
        Quaternion attackIndicatorDir = Quaternion.LookRotation(Vector3.forward, dir);
        attackIndicator.transform.rotation = attackIndicatorDir * Quaternion.Euler(0f, 0f, 180f);
        return attackIndicator;
    }
}
