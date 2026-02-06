using System;
using UnityEngine;

public abstract class MonsterAttack : MonoBehaviour
{
    protected Vector3 dir = Vector3.zero;
    protected float attackDamage = 10f;
    protected float attackSpeed = 1f;
    protected bool isPlayerSide = false;

    public event Action<string, bool> OnAttackEnd;
    public event Action OnStartedAttack;

    protected void BroadcastOnStartedAttack()
    {
        OnStartedAttack?.Invoke();
    }

    public void SetAttackStat(float newAttackDamage, float newAttackSpeed)
    {
        attackDamage = newAttackDamage;
        attackSpeed = newAttackSpeed;
    }

    public void SetIsPlayerSide(bool newIsPlayerSide)
    {
        isPlayerSide = newIsPlayerSide;
    }

    public abstract void Attack();

    public virtual void AttackEnd()
    {
        OnAttackEnd?.Invoke("isAttack", false);
    }

    public void SetAttackDir(Vector3 attackDir)
    {
        dir = attackDir;
    }

    protected virtual AttackIndicator ActivateAttackIndicator()
    {
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        angle += 90;
        Vector3 attackDir = new Vector3(0, 0, angle);
        ObjectPooler.Instance.SpawnFromPool("AttackIndicator", transform.position, Quaternion.Euler(attackDir))
            .TryGetComponent<AttackIndicator>(out AttackIndicator attackIndicator);
        if (attackIndicator == null) return null;
        return attackIndicator;
    }
}
