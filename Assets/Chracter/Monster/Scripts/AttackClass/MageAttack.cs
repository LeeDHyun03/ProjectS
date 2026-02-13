using UnityEngine;

public class MageAttack : MageProjectileAttack
{
    [SerializeField] private string projectileTag;

    protected override void DoAction(Vector3 indicatorCenterPos)
    {
        var go = ObjectPooler.Instance.SpawnFromPool(projectileTag, indicatorCenterPos, Quaternion.identity);
        if (go == null)
            return;

        if (!go.TryGetComponent<Meteo>(out var meteo))
            return;

        meteo.SetDefaultValue(
            targetPos: indicatorCenterPos, 
            damage: attackDamage, 
            radius: radius, 
            isPlayerSide: isPlayerSide, 
            fallingTime: attackSpeed
            );
        meteo.StartFall();
    }
}
