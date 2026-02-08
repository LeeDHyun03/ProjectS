using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonsterAttack : MonoBehaviour
{
    protected Vector3 dir = Vector3.zero;
    protected float attackDamage = 10f;
    protected float attackSpeed = 1f;
    protected bool isPlayerSide = false;

    public event Action<string, bool> OnAttackEnd;
    public event Action OnStartedAttack;

    private readonly List<IAttackIndicator> activeIndicators = new();

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

    public void DespawnIndicator()
    {
        for (int i = activeIndicators.Count - 1; i >= 0; i--)
        {
            var indicator = activeIndicators[i];
            if (indicator is MonoBehaviour mb && mb != null)
                ObjectPooler.ReturnToPool(mb.gameObject);
        }

        activeIndicators.Clear();
    }
    public enum IndicatorShape { Box, Circle }
    protected virtual IAttackIndicator ActivateAttackIndicator(IndicatorShape shape, Vector3 pos, Vector3 dir)
    {
        string tag;
        Quaternion rot;

        switch (shape)
        {
            case IndicatorShape.Box:
                {
                    tag = "AttackIndicator";
                    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 90f;
                    rot = Quaternion.Euler(0f,0f,angle);
                    break;
                }
            case IndicatorShape.Circle:
                {
                    tag = "AttackCircleIndicator";
                    rot = Quaternion.identity;
                    break;
                }
            default:
                return null;
        }

        var go = ObjectPooler.Instance.SpawnFromPool(tag, pos, rot);
        if (go == null) return null;

        if (!go.TryGetComponent<IAttackIndicator>(out var indicator))
            return null;

        activeIndicators.Add(indicator);
        indicator.OnIndicatorComplete += () =>
        {
            activeIndicators.Remove(indicator);
        };

        return indicator;
    }

}
