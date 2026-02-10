using Roguelike.Items;
using UnityEngine;
using System;

public interface IConditionEvaluator
{
    bool Evaluate(ItemEffect eff, int itemLevel, in ConditionContext ctx, ITargetConditionProvider target);
}

public interface ITargetConditionProvider
{
    bool HasTarget { get; }
    bool TargetIsBoss { get; }
    bool TargetHasSuperArmor { get; }

    int GetTargetKeywordStacks(string keywordIdOrName);
    int GetTargetKeywordMax(string keywordIdOrName);
}

public class ConditionEvaluator : IConditionEvaluator
{
    public bool Evaluate(ItemEffect eff, int itemLevel, in ConditionContext ctx, ITargetConditionProvider target)
    {
        if (eff == null) return false;

        switch (eff.ConditionExpr)
        {
            case EConditionExpr.None:
                return true;

            case EConditionExpr.NightOnly:
                return ctx.IsNight;

            case EConditionExpr.HPAboveByLevel:
                {
                    float th = GetLevelValue(eff.ConditionValuesByLevel, itemLevel);
                    return ctx.HpPct >= th;
                }

            case EConditionExpr.MPAboveByLevel:
                {
                    float th = GetLevelValue(eff.ConditionValuesByLevel, itemLevel);
                    return ctx.MpPct >= th;
                }

            case EConditionExpr.NotBoss:
                return target != null && target.HasTarget && !target.TargetIsBoss;

            case EConditionExpr.NotHasSuperArmor:
                return target != null && target.HasTarget && !target.TargetHasSuperArmor;

            //case EConditionExpr.KeywordAtMax:
            //    {
            //        if (target == null || !target.HasTarget) return false;
            //        var k = eff.ConditionArg;
            //        int stacks = target.GetTargetKeywordStacks(k);
            //        int max = target.GetTargetKeywordMax(k);
            //        return max > 0 && stacks >= max;
            //    }

            //case EConditionExpr.KeywordProc:
            //    // 이건 보통 “이벤트 발생 시” 트리거로 쓰는 게 맞아서
            //    // 조건부 While 평가에서는 false로 두고, 발동형 단계에서 처리 권장
            //    return false;

            default:
                return false;
        }
    }

    private static float GetLevelValue(float[] arr, int level)
    {
        if (arr == null || arr.Length == 0) return 0f;
        int idx = Math.Clamp(level - 1, 0, arr.Length - 1);
        return arr[idx];
    }

    private static float GetCondValue(ItemEffect eff, int itemLevel)
    {
        if (eff.ConditionValuesByLevel == null || eff.ConditionValuesByLevel.Length == 0) return 0f;
        int idx = Mathf.Clamp(itemLevel - 1, 0, eff.ConditionValuesByLevel.Length - 1);
        return eff.ConditionValuesByLevel[idx];
    }
}
