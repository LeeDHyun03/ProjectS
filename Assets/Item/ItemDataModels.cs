using System;
using System.Collections.Generic;
using UnityEngine;

namespace Roguelike.Items
{
    [Serializable]
    public sealed class ItemMasterDbJson
    {
        public List<ItemMasterJson> Items;
    }

    [Serializable]
    public sealed class ItemEffectsDbJson
    {
        public List<ItemEffectJson> Effects;
    }

    [Serializable]
    public sealed class ItemMasterJson
    {
        public string ItemId;
        public string NameKr;
        public string Description;

        public string Rarity;
        public int MaxLevel = 1;

        public string ProgressionType;
        public string IconKey;
    }

    [Serializable]
    public sealed class ItemEffectJson
    {
        public string ItemId;
        public int EffectIndex;

        public string TriggerTag;
        public string TriggerParam;
        public string ActionTag;
        public string TargetTag;
        public string TimingTag;

        public string TargetAnchor;
        public string TargetSelector;
        public string TargetFilter;

        public string ConditionExpr;
        public string ConditionMode;
        public string ConditionValueByLevel;

        public string LevelValues;
        public string ProcChanceByLevel;

        public float DurationSec;
        public float IntervalSec;

        public string DamageSourceType;
        public string KillSourceType;

        public string StatId;
        public string StatOp;
        public string KeywordId;

        public string Notes;
        public string FormulaId;
        public string FormulaParamsJson;
    }

    public enum EItemRarity { Common, Rare, Epic, Legendary, Unknown }
    public enum EConditionExpr { None, NotBoss, KeywordAtMax, HPAboveByLevel, MPAboveByLevel, NotHasSuperArmor, NightOnly }
    public enum EDamageSourceType { None, Normal, Ignition, Transfer, Mark }
    public enum EDamagePhase { None, Direct, Tick, Proc, Expire }

    public sealed class ItemData
    {
        public string ItemId { get; }
        public string NameKr { get; }
        public string Description { get; }
        public EItemRarity Rarity { get; }
        public int MaxLevel { get; }
        public string ProgressionType { get; }
        public string IconKey { get; }

        public readonly List<ItemEffect> Effects = new();

        public ItemData(string itemId, string nameKr, string desc, EItemRarity rarity, int maxLevel, string progressionType, string iconKey)
        {
            ItemId = itemId ?? "";
            NameKr = nameKr ?? "";
            Description = desc ?? "";
            Rarity = rarity;
            MaxLevel = Mathf.Max(1, maxLevel);
            ProgressionType = progressionType ?? "";
            IconKey = iconKey ?? "";
        }
    }

    public sealed class ItemEffect
    {
        public string ItemId;
        public int EffectIndex;

        public string TriggerTag;
        public string TriggerParam;
        public string ActionTag;
        public string TargetTag;
        public string TimingTag;

        public string TargetAnchor;
        public string TargetSelector;
        public string TargetFilter;

        public EConditionExpr ConditionExpr;
        public string ConditionArg;
        public float[] ConditionValuesByLevel;

        public float[] LevelValues;
        public float[] ProcChanceByLevel;

        public float DurationSec;
        public float IntervalSec;

        public EDamageSourceType DamageSourceType;
        public EDamageSourceType KillSourceType;

        public string StatId;
        public string StatOp;
        public string KeywordId;

        public string Notes;
        public string FormulaId;
        public string FormulaParamsJson;

        public EDamagePhase DamagePhase;
    }
}
