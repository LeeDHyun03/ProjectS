using Roguelike.Items;
using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(PlayerCharacter))]
[RequireComponent(typeof(PlayerItemStatController))]
public sealed class PlayerConditionalStatController : MonoBehaviour
{
    private PlayerCharacter _player;
    private PlayerItemStatController _stats;
    private readonly IConditionEvaluator _eval = new ConditionEvaluator();

    private readonly HashSet<long> _active = new(); // (itemIdHash + effectIndex) 키

    public int ActiveCount => _active.Count;

    private void Awake()
    {
        _player = GetComponent<PlayerCharacter>();
        _stats = GetComponent<PlayerItemStatController>();
    }

    private void OnEnable()
    {
        _player.OnHpChanged += OnHpChanged;
        _player.OnMpChanged += OnMpChanged;
    }

    private void OnDisable()
    {
        _player.OnHpChanged -= OnHpChanged;
        _player.OnMpChanged -= OnMpChanged;
    }

    private void OnHpChanged(float hp, float maxHp) => Reevaluate();
    private void OnMpChanged(float mp, float maxMp) => Reevaluate();

    public void Reevaluate()
    {
        if (!ItemDataManager.Instance || !ItemDataManager.Instance.IsLoaded) return;

        var ctx = BuildContext();
        ITargetConditionProvider target = null;

        bool changed = false;

        foreach (var inst in _stats.EnumerateRunItems())
        {
            var itemId = (inst.ItemId ?? "").Trim();
            if (!ItemDataManager.Instance.TryGetItem(itemId, out var item)) continue;

            foreach (var eff in item.Effects)
            {
                if (eff == null) continue;
                if (!IsConditionalStatEffect(eff)) continue;

                bool ok = _eval.Evaluate(eff, inst.Level, ctx, target);

                long key = MakeKey(itemId, eff.EffectIndex);
                bool was = _active.Contains(key);

                if (ok && !was) { _active.Add(key); changed = true; }
                else if (!ok && was) { _active.Remove(key); changed = true; }
            }
        }

        if (changed)
            _stats.RebuildAndApply();
    }




    private ConditionContext BuildContext()
    {
        // 밤 여부는 아직 시스템이 없다면 임시 false.
        bool isNight = false;
        return new ConditionContext(_player.CurrentHp, _player.MaxHp, _player.CurrentMp, _player.MaxMp, isNight);
    }

    private static bool IsConditionalStatEffect(ItemEffect e)
    {
        if (e == null) return false;

        if (e.ConditionExpr == EConditionExpr.None) return false;
        if (e.ConditionValuesByLevel == null || e.ConditionValuesByLevel.Length == 0)
            return false;

        if (string.Equals(e.ActionTag, "StatAddFlat", StringComparison.OrdinalIgnoreCase)) return true;
        if (string.Equals(e.ActionTag, "StatAddPercent", StringComparison.OrdinalIgnoreCase)) return true;
        if (!string.IsNullOrWhiteSpace(e.StatId) &&
            (string.Equals(e.StatOp, "AddFlat", StringComparison.OrdinalIgnoreCase) ||
             string.Equals(e.StatOp, "AddPercent", StringComparison.OrdinalIgnoreCase)))
            return true;

        return false;
    }

    private static long MakeKey(string itemId, int effectIndex)
    {
        int h = StringComparer.OrdinalIgnoreCase.GetHashCode(itemId ?? "");
        return ((long)h << 32) ^ (uint)effectIndex;
    }

    public bool IsActive(string itemId, int effectIndex)
    {
        return _active.Contains(MakeKey(itemId, effectIndex));
    }
    
}
