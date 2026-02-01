using UnityEngine;

public class ProjectileAttack : MonsterAttack
{
    [SerializeField] private string projectileTag;

    [SerializeField] private float lifeTime = 5f;
    [SerializeField] private float speed = 10f;

    public override void Attack()
    {
        ObjectPooler.Instance.SpawnFromPool(projectileTag, transform.position, Quaternion.identity)
            .TryGetComponent<Arrow>(out Arrow projectile);
        if (projectile == null) return;

        projectile.SetDefaultValue(dir, lifeTime, speed, attackDamage, isPlayerSide);
    }

    public void SetProjectileDirection()
    {
        AttackIndicator attackIndicator = ActivateAttackIndicator();
        attackIndicator.OnIndicatorComplete += () =>
        {
            dir = (attackIndicator.transform.right).normalized;
        };
    }
}
