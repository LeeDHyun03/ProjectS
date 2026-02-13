using Roguelike.Items;
using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(PlayerCharacter))]
public sealed class PlayerItemStatController : MonoBehaviour
{
    [Serializable]
    public sealed class RunItem
    {
        public string itemId;
        public int level = 1;
        public int stacks = 1;
    }

    public struct PlayerStatBlock
    {
        public float maxHp;
        public float maxMp;

        public float attackDamage;

        public float normalAttackDamage;
        public float specialAttackDamage;

        public float normalAttackSpeed;
        public float specialAttackSpeed;

        public float moveSpeed;

        public float defense;

        public float critChance;
        public float critDamage;

        public float pride;
        public float anger;
        public float jealousy;

        public int rerollCount;
    }

    [SerializeField] private ItemIcon ItemIconPrefab;
    [SerializeField] private Transform playerUITransform;
    private PlayerCharacter _player;
    public static readonly Dictionary<string, RunItem> _inv = new(StringComparer.OrdinalIgnoreCase);
    private PlayerConditionalStatController cond;

    private void Awake()
    {
        _player = GetComponent<PlayerCharacter>();
    }

    private void Start()
    {
        cond = GetComponent<PlayerConditionalStatController>();
    }

    public void AddItem(string itemId, int amount = 1)
    {
        if (string.IsNullOrWhiteSpace(itemId)) return;
        if (!ItemDataManager.Instance || !ItemDataManager.Instance.IsLoaded) return;
        if (!ItemDataManager.Instance.TryGetItem(itemId, out var item)) return;

        amount = Mathf.Max(1, amount);

        if (!_inv.TryGetValue(itemId, out var inst))
        {
            inst = new RunItem { itemId = itemId, level = 1, stacks = 1 };
            _inv[itemId] = inst;
            amount -= 1;
            ItemIcon itemIcon = Instantiate(ItemIconPrefab, playerUITransform);
            Sprite iconSprite = ItemDataManager.Instance.LoadIcon(item);
            Sprite iconFrameSprite = ItemDataManager.Instance.LoadItemFrame(item);
            itemIcon.SetItemInfo(item.NameKr, item.Description, iconSprite, iconFrameSprite);
        }

        if (amount > 0)
        {
            if (string.Equals(item.ProgressionType, "LevelUp", StringComparison.OrdinalIgnoreCase))
            {
                inst.level = Mathf.Min(item.MaxLevel, inst.level + amount);
            }
            else
            {
                inst.stacks += amount;
            }
        }
        RebuildAndApply();
    }

    public void RebuildAndApply()
    {
        if (!_player) return;
        if (!ItemDataManager.Instance || !ItemDataManager.Instance.IsLoaded) return;

        var baseStats = _player.BuildBaseStatsFromDataManager();
        var finalStats = baseStats;

        foreach (var kv in _inv)
        {
            var inst = kv.Value;
            if (inst == null) continue;
            if (!ItemDataManager.Instance.TryGetItem(inst.itemId, out var item)) continue;

            foreach (var eff in item.Effects)
            {
                if (eff == null) continue;

                if (!IsStatEffect(eff)) continue;

                bool isConditional = eff.ConditionExpr != EConditionExpr.None;
                if (isConditional)
                {
                    if (cond == null || !cond.IsActive(item.ItemId, eff.EffectIndex)) continue;
                }

                float v = GetByLevel(eff.LevelValues, inst.level, item.MaxLevel);

                if (!string.Equals(item.ProgressionType, "LevelUp", StringComparison.OrdinalIgnoreCase))
                    v *= Mathf.Max(1, inst.stacks);

                ApplyStat(ref finalStats, eff, v);
            }
        }

        _player.ApplyFinalStats(finalStats);
    }

    public static bool FindItem(string itemId)
    {
        if (_inv.TryGetValue(itemId, out var inst))
        {
            return true;
        }
        return false;
    }

    public static int GetItemValueByLevel(string itemId, int effectIndex)
    {
        _inv.TryGetValue(itemId, out RunItem runItem);
        if (runItem == null) return 0;
        int value = 0;

        if (ItemDataManager.Instance.TryGetItem(itemId, out ItemData item))
        {
            ItemEffect effect = item.Effects[effectIndex];
            value = (int)effect.LevelValues[runItem.level];
        }
        return value;
    }

    public IEnumerable<(string ItemId, int Level, int Stacks)> EnumerateRunItems()
    {
        foreach (var kv in _inv)
        {
            var i = kv.Value;
            if (i == null) continue;
            yield return ((i.itemId ?? "").Trim(), i.level, i.stacks);
        }
    }

    private static bool IsStatEffect(ItemEffect e)
    {
        if (e == null) return false;

        if (string.Equals(e.ActionTag, "StatAddFlat", StringComparison.OrdinalIgnoreCase)) return true;
        if (string.Equals(e.ActionTag, "StatAddPercent", StringComparison.OrdinalIgnoreCase)) return true;

        if (!string.IsNullOrWhiteSpace(e.StatId) &&
            (string.Equals(e.StatOp, "AddFlat", StringComparison.OrdinalIgnoreCase) ||
             string.Equals(e.StatOp, "AddPercent", StringComparison.OrdinalIgnoreCase)))
            return true;

        return false;
    }

    private static float GetByLevel(float[] arr, int level, int maxLevel)
    {
        if (arr == null || arr.Length == 0) return 0f;
        int lv = Mathf.Clamp(level, 1, Mathf.Max(1, maxLevel));
        int idx = Mathf.Clamp(lv - 1, 0, arr.Length - 1);
        return arr[idx];
    }

    private static void ApplyStat(ref PlayerStatBlock s, ItemEffect eff, float value)
    {
        bool isPercent =
            string.Equals(eff.ActionTag, "StatAddPercent", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(eff.StatOp, "AddPercent", StringComparison.OrdinalIgnoreCase);

        string stat = (eff.StatId ?? "").Trim();

        if (!isPercent)
        {
            if (stat.Equals("MaxHp", StringComparison.OrdinalIgnoreCase)) s.maxHp += value;
            else if (stat.Equals("AttackDamage", StringComparison.OrdinalIgnoreCase)) s.attackDamage += value;
            else if (stat.Equals("MeleeDamageMult", StringComparison.OrdinalIgnoreCase)) s.normalAttackDamage += value;
            else if (stat.Equals("RangedDamageMult", StringComparison.OrdinalIgnoreCase)) s.specialAttackDamage += value;
            else if (stat.Equals("MeleeAttackSpeed", StringComparison.OrdinalIgnoreCase)) s.normalAttackSpeed += value;
            else if (stat.Equals("RangedAttackSpeed", StringComparison.OrdinalIgnoreCase)) s.specialAttackSpeed += value;
            else if (stat.Equals("MoveSpeed", StringComparison.OrdinalIgnoreCase)) s.moveSpeed += value;
            else if (stat.Equals("MaxMp", StringComparison.OrdinalIgnoreCase)) s.maxMp += value;
            else if (stat.Equals("Defense", StringComparison.OrdinalIgnoreCase)) s.defense += value;
            else if (stat.Equals("CritChance", StringComparison.OrdinalIgnoreCase)) s.critChance += value;
            else if (stat.Equals("CritDamage", StringComparison.OrdinalIgnoreCase)) s.critDamage += value;
            else if (stat.Equals("Wrath", StringComparison.OrdinalIgnoreCase)) s.anger += value;
            else if (stat.Equals("Envy", StringComparison.OrdinalIgnoreCase)) s.jealousy += value;
            else if (stat.Equals("Pride", StringComparison.OrdinalIgnoreCase)) s.anger += value;
            return;
        }

        float m = 1f + (value / 100f);

        if (stat.Equals("MaxHp", StringComparison.OrdinalIgnoreCase)) s.maxHp *= m;
        else if (stat.Equals("AttackDamage", StringComparison.OrdinalIgnoreCase)) s.attackDamage *= m;
        else if (stat.Equals("MeleeDamageMult", StringComparison.OrdinalIgnoreCase)) s.normalAttackDamage *= m;
        else if (stat.Equals("RangedDamageMult", StringComparison.OrdinalIgnoreCase)) s.specialAttackDamage *= m;
        else if (stat.Equals("MeleeAttackSpeed", StringComparison.OrdinalIgnoreCase)) s.normalAttackSpeed *= m;
        else if (stat.Equals("RangedAttackSpeed", StringComparison.OrdinalIgnoreCase)) s.specialAttackSpeed *= m;
        else if (stat.Equals("MoveSpeed", StringComparison.OrdinalIgnoreCase)) s.moveSpeed *= m;
        else if (stat.Equals("MaxMp", StringComparison.OrdinalIgnoreCase)) s.maxMp *= m;
        else if (stat.Equals("Defense", StringComparison.OrdinalIgnoreCase)) s.defense *= m;
        else if (stat.Equals("CritChance", StringComparison.OrdinalIgnoreCase)) s.critChance *= m;
        else if (stat.Equals("CritDamage", StringComparison.OrdinalIgnoreCase)) s.critDamage *= m;
        else if (stat.Equals("Wrath", StringComparison.OrdinalIgnoreCase)) s.anger *= value;
        else if (stat.Equals("Envy", StringComparison.OrdinalIgnoreCase)) s.anger *= value;
        else if (stat.Equals("Pride", StringComparison.OrdinalIgnoreCase)) s.anger *= value;
    }
}
