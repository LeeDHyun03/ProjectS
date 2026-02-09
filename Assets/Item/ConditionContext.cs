using UnityEngine;

public readonly struct ConditionContext
{
    public readonly float Hp;
    public readonly float MaxHp;
    public readonly float Mp;
    public readonly float MaxMp;
    public readonly bool IsNight;

    public float HpPct => (MaxHp > 0) ? (Hp / MaxHp) * 100f: 0f;
    public float MpPct => (MaxMp > 0) ? (Mp / MaxMp) * 100f : 0f;

    public ConditionContext(float hp, float maxHp, float mp, float maxMp, bool isNight)
    {
        Hp = hp;
        MaxHp = maxHp;
        Mp = mp;
        MaxMp = maxMp;
        IsNight = isNight;
    }
}
