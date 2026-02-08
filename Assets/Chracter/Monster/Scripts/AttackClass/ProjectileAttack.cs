using UnityEngine;

public class ProjectileAttack : MonsterAttack
{
    [SerializeField] private string projectileTag;
    [SerializeField] private float lifeTime = 3f;
    [SerializeField] private float speed = 10f;
    [SerializeField] private float arrowIndicatorThickness = 1f;

    public override void Attack()
    {
        var go = ObjectPooler.Instance.SpawnFromPool(projectileTag, transform.position, Quaternion.identity);
        if (go == null || !go.TryGetComponent<Arrow>(out var projectile)) return;

        projectile.SetDefaultValue(dir, lifeTime, speed, attackDamage, isPlayerSide);
    }

    public void SetAttackIndicatorDirection()
    {
        var indicator = ActivateAttackIndicator(IndicatorShape.Box, transform.position, dir);
        if (indicator == null) return;

        indicator.OnIndicatorComplete += BroadcastOnStartedAttack;

        float travelDistance = speed * lifeTime;
        Vector2 size = new Vector2(travelDistance, arrowIndicatorThickness);

        indicator.StartIndicator(size, attackSpeed);

        if (indicator is MonoBehaviour mb)
        {
            mb.transform.SetParent(transform, worldPositionStays: true);
            mb.transform.localPosition = Vector3.zero;
        }
    }
}
