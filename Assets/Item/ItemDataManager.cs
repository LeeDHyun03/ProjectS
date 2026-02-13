using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace Roguelike.Items
{
    public sealed class ItemDataManager : MonoBehaviour
    {
        public static ItemDataManager Instance { get; private set; }

        [SerializeField] private string masterFileName = "ItemMaster.json";
        [SerializeField] private string effectsFileName = "ItemEffects.json";

        public bool IsLoaded { get; private set; }
        public string LastError { get; private set; }

        private readonly Dictionary<string, ItemData> _itemsById =
            new Dictionary<string, ItemData>(StringComparer.OrdinalIgnoreCase);

        public IReadOnlyDictionary<string, ItemData> ItemsById => _itemsById;

        private readonly Dictionary<string, Sprite> _iconCache = new(StringComparer.Ordinal);
        private readonly Dictionary<string, Sprite> _iconFrameCache = new();
        private readonly HashSet<string> _iconMissCache = new(StringComparer.Ordinal);
        private readonly HashSet<string> _iconFrameMissCache = new(StringComparer.Ordinal);
        private const string DefaultIconKey = "Icons/Items/Default";
        private const string DefaultIconFrameKey = "Icons/Frames/Frame_Default";
        private const string CommonIconFrameKey = "Icons/Frames/Frame_Common";
        private const string RareIconFrameKey = "Icons/Frames/Frame_Rare";
        private const string EpicIconFrameKey = "Icons/Frames/Frame_Epic";
        private const string LegendaryIconFrameKey = "Icons/Frames/Frame_Legendary";

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public IEnumerator LoadAsync()
        {
            if (IsLoaded) yield break;

            LastError = "";
            _itemsById.Clear();

            string masterJson = null;
            yield return LoadTextFromStreamingAssets(masterFileName, s => masterJson = s);
            if (!string.IsNullOrEmpty(LastError)) yield break;
            if (!BuildItems(masterJson)) yield break;

            string effectsJson = null;
            yield return LoadTextFromStreamingAssets(effectsFileName, s => effectsJson = s);
            if (!string.IsNullOrEmpty(LastError)) yield break;
            if (!AttachEffects(effectsJson)) yield break;

            foreach (var kv in _itemsById)
                kv.Value.Effects.Sort((a, b) => a.EffectIndex.CompareTo(b.EffectIndex));

            IsLoaded = true;
            Debug.Log($"[ItemDataManager] Loaded items={_itemsById.Count}");
        }

        public bool TryGetItem(string itemId, out ItemData item)
        {
            itemId = (itemId ?? "").Trim();
            return _itemsById.TryGetValue(itemId, out item);
        }

        public void ResetDatabase()
        {
            IsLoaded = false;
            LastError = "";
            _itemsById.Clear();
            _iconCache.Clear();
            _iconMissCache.Clear();
            _iconFrameCache.Clear();
            _iconFrameMissCache.Clear();
        }

        public Sprite LoadIcon(ItemData item)
        {
            if (item == null) return LoadIconByKey(DefaultIconKey);
            return LoadIconByKey(item.IconKey);
        }

        public Sprite LoadItemFrame(ItemData item)
        {
            if (item == null) return LoadIconFrameByKey(EItemRarity.Unknown);
            return LoadIconFrameByKey(item.Rarity);
        }

        public Sprite LoadIconByKey(string iconKey)
        {
            if (string.IsNullOrWhiteSpace(iconKey))
                iconKey = DefaultIconKey;

            if (_iconCache.TryGetValue(iconKey, out var cached) && cached != null)
                return cached;

            if (_iconMissCache.Contains(iconKey))
                return _iconCache.TryGetValue(DefaultIconKey, out var d) ? d : null;

            var sprite = Resources.Load<Sprite>(iconKey);
            if (sprite == null)
            {
                _iconMissCache.Add(iconKey);

                if (iconKey != DefaultIconKey)
                    return LoadIconByKey(DefaultIconKey);

                return null;
            }

            _iconCache[iconKey] = sprite;
            return sprite;
        }

        public Sprite LoadIconFrameByKey(EItemRarity rarityKey)
        {
            string iconFrameKey = DefaultIconFrameKey;
            switch (rarityKey)
            {
                case EItemRarity.Common:
                    iconFrameKey = CommonIconFrameKey;
                    break;
                case EItemRarity.Rare:
                    iconFrameKey = RareIconFrameKey;
                    break;
                case EItemRarity.Epic:
                    iconFrameKey = EpicIconFrameKey;
                    break;
                case EItemRarity.Legendary:
                    iconFrameKey = LegendaryIconFrameKey;
                    break;
                case EItemRarity.Unknown:
                    iconFrameKey = DefaultIconFrameKey;
                    break;
                default:
                    iconFrameKey = DefaultIconFrameKey;
                    break;
            }

            if (_iconFrameCache.TryGetValue(iconFrameKey, out Sprite frame))
                return frame;

            if (_iconFrameMissCache.Contains(iconFrameKey))
                return _iconFrameCache.TryGetValue(DefaultIconKey, out Sprite d) ? d : null;

            var sprite = Resources.Load<Sprite>(iconFrameKey);
            if (sprite == null)
            {
                _iconFrameMissCache.Add(iconFrameKey);

                if (iconFrameKey != DefaultIconFrameKey)
                    return LoadIconFrameByKey(EItemRarity.Unknown);

                return null;
            }

            _iconFrameCache[iconFrameKey] = sprite;
            return sprite;
        }

        private IEnumerator LoadTextFromStreamingAssets(string fileName, Action<string> onLoaded)
        {
            var path = Path.Combine(Application.streamingAssetsPath, fileName);

            using (var req = UnityWebRequest.Get(path))
            {
                yield return req.SendWebRequest();

                if (req.result != UnityWebRequest.Result.Success)
                {
                    LastError = $"StreamingAssets load failed: {path}\n{req.error}";
                    Debug.LogError($"[ItemDataManager] {LastError}");
                    yield break;
                }

                var text = req.downloadHandler.text;
                if (string.IsNullOrWhiteSpace(text))
                {
                    LastError = $"{fileName} is empty: {path}";
                    Debug.LogError($"[ItemDataManager] {LastError}");
                    yield break;
                }

                onLoaded?.Invoke(text);
            }
        }

        private bool BuildItems(string json)
        {
            ItemMasterDbJson db;
            try
            {
                db = JsonUtility.FromJson<ItemMasterDbJson>(json);
            }
            catch (Exception e)
            {
                LastError = $"Master parse failed: {e.Message}";
                Debug.LogError($"[ItemDataManager] {LastError}");
                return false;
            }

            if (db == null || db.Items == null || db.Items.Count == 0)
            {
                LastError = "Master parsed but 'items' is missing/empty.";
                Debug.LogError($"[ItemDataManager] {LastError}");
                return false;
            }

            for (int i = 0; i < db.Items.Count; i++)
            {
                var m = db.Items[i];
                if (m == null) continue;

                var id = (m.ItemId ?? "").Trim();
                if (string.IsNullOrWhiteSpace(id))
                    continue;

                if (_itemsById.ContainsKey(id))
                    continue;

                var data = new ItemData(
                    itemId: id,
                    nameKr: (m.NameKr ?? "").Trim(),
                    desc: m.Description ?? "",
                    rarity: ItemDataParseUtil.ParseRarity(m.Rarity),
                    maxLevel: Mathf.Max(1, m.MaxLevel),
                    progressionType: m.ProgressionType ?? "",
                    iconKey: string.IsNullOrWhiteSpace(m.IconKey) ? $"Icons/Items/{id}" : m.IconKey.Trim()
                );

                _itemsById.Add(id, data);
            }

            if (_itemsById.Count == 0)
            {
                LastError = "Master loaded but no valid items.";
                Debug.LogError($"[ItemDataManager] {LastError}");
                return false;
            }

            return true;
        }

        private bool AttachEffects(string json)
        {
            ItemEffectsDbJson db;
            try
            {
                db = JsonUtility.FromJson<ItemEffectsDbJson>(json);
            }
            catch (Exception e)
            {
                LastError = $"Effects parse failed: {e.Message}";
                Debug.LogError($"[ItemDataManager] {LastError}");
                return false;
            }

            if (db == null || db.Effects == null)
            {
                LastError = "Effects parsed but 'effects' is missing.";
                Debug.LogError($"[ItemDataManager] {LastError}");
                return false;
            }
            foreach (var src in db.Effects)
            {
                if (src == null) continue;
                var id = (src.ItemId ?? "").Trim();
                if (string.IsNullOrWhiteSpace(id)) continue;
                if (!_itemsById.TryGetValue(id, out var item))
                    continue;

                TryAttachEffect(item, src);
            }

            return true;
        }

        private void TryAttachEffect(ItemData item, ItemEffectJson src)
        {
            if (item == null || src == null) return;
            if (string.IsNullOrWhiteSpace(src.ActionTag)) return;


            var trigger = ItemDataParseUtil.NormalizeTriggerTag(src.TriggerTag);

            var e = new ItemEffect
            {
                ItemId = item.ItemId,
                EffectIndex = src.EffectIndex,

                TriggerTag = string.IsNullOrWhiteSpace(trigger) ? "Passive" : trigger,
                TriggerParam = src.TriggerParam ?? "",
                ActionTag = src.ActionTag ?? "",
                TargetTag = string.IsNullOrWhiteSpace(src.TargetTag) ? "TargetSelf" : src.TargetTag,
                TimingTag = src.TimingTag ?? "",

                TargetAnchor = src.TargetAnchor ?? "",
                TargetSelector = src.TargetSelector ?? "",
                TargetFilter = src.TargetFilter ?? "",

                ConditionExpr = ItemDataParseUtil.ParseConditionExpr(src.ConditionExpr),
                ConditionArg = ItemDataParseUtil.ExtractConditionTypeArg(src.ConditionExpr),
                ConditionValuesByLevel = ItemDataParseUtil.ParseLevelArray(src.ConditionValueByLevel, item.MaxLevel),

                LevelValues = ItemDataParseUtil.ParseLevelArray(src.LevelValues, item.MaxLevel),
                ProcChanceByLevel = ItemDataParseUtil.ParseLevelArray(src.ProcChanceByLevel, item.MaxLevel),

                DurationSec = src.DurationSec,
                IntervalSec = src.IntervalSec,

                DamageSourceType = ItemDataParseUtil.ParseDamageSourceType(src.DamageSourceType),
                KillSourceType = ItemDataParseUtil.ParseDamageSourceType(src.KillSourceType),

                StatId = src.StatId ?? "",
                StatOp = src.StatOp ?? "",
                KeywordId = src.KeywordId ?? "",

                Notes = src.Notes ?? "",
                FormulaId = src.FormulaId ?? "",
                FormulaParamsJson = src.FormulaParamsJson ?? ""
            };

            e.DamagePhase = ItemDataParseUtil.ResolvePhase(e.TriggerTag);

            item.Effects.Add(e);
        }
    }
}
