using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Roguelike.Items.EditorTools
{
    /// <summary>
    /// itemData.json(Effects-only) -> ItemMaster.json 생성기
    ///
    /// 사용:
    /// 1) Assets/StreamingAssets/itemData.json  (effects-only) 준비
    /// 2) Unity 메뉴: Tools/Roguelike/Items/Generate ItemMaster.json
    ///
    /// 출력:
    /// - Assets/StreamingAssets/ItemMaster.json
    ///
    /// 규칙:
    /// - itemId 기준 그룹핑
    /// - nameKr: 그룹 내 첫 유효값
    /// - description: 그룹 내 가장 긴 문자열 1개(정보 손실 최소화)
    /// - maxLevel: 그룹 내 MaxLevel 최대값(없으면 1)
    /// - progressionType: maxLevel==1 -> None, 그 외 -> LevelUp (추후 확장 가능)
    /// - rarity: maxLevel 기반 임시 규칙 (1 Common, 2 Rare, 3 Epic, >=4 Legendary)
    /// - iconKey: "Icons/Items/{itemId}"
    /// </summary>
    public static class ItemMasterGeneratorEditor
    {
        private const string DefaultSourceFileName = "itemData.json";
        private const string OutputFileName = "ItemMaster.json";

        [MenuItem("Tools/Roguelike/Items/Generate ItemMaster.json")]
        public static void Generate()
        {
            try
            {
                var streaming = Application.streamingAssetsPath;

                var srcPath = Path.Combine(streaming, DefaultSourceFileName);
                if (!File.Exists(srcPath))
                {
                    EditorUtility.DisplayDialog(
                        "ItemMaster Generator",
                        $"Source not found:\n{srcPath}\n\nPut effects-only json at StreamingAssets/{DefaultSourceFileName}",
                        "OK");
                    return;
                }

                var json = File.ReadAllText(srcPath);
                if (string.IsNullOrWhiteSpace(json))
                {
                    EditorUtility.DisplayDialog("ItemMaster Generator", "Source json is empty.", "OK");
                    return;
                }

                // itemData.json은 보통 "루트 배열"일 확률이 높음
                // JsonUtility는 루트 배열을 직접 못 읽으므로 래핑
                var wrapped = json.TrimStart().StartsWith("[")
                    ? "{ \"effects\": " + json + " }"
                    : json;

                var effectsDb = JsonUtility.FromJson<EffectsDbJson>(wrapped);
                if (effectsDb == null || effectsDb.effects == null || effectsDb.effects.Count == 0)
                {
                    EditorUtility.DisplayDialog(
                        "ItemMaster Generator",
                        "Parsed effects is null/empty.\nCheck JSON keys and structure.",
                        "OK");
                    return;
                }

                // itemId 그룹핑
                var groups = effectsDb.effects
                    .Where(e => e != null && !string.IsNullOrWhiteSpace(e.ItemId))
                    .GroupBy(e => e.ItemId.Trim(), StringComparer.OrdinalIgnoreCase)
                    .OrderBy(g => g.Key, StringComparer.OrdinalIgnoreCase)
                    .ToList();

                var master = new ItemMasterDbJson { Items = new List<ItemMasterRowJson>(groups.Count) };

                foreach (var g in groups)
                {
                    var itemId = g.Key.Trim();

                    // nameKr: 첫 유효값
                    var nameKr = g.Select(x => (x.NameKr ?? "").Trim())
                                  .FirstOrDefault(s => !string.IsNullOrWhiteSpace(s)) ?? "";

                    // description: 가장 긴 문자열(없으면 빈 문자열)
                    var desc = g.Select(x => (x.Description ?? "").Trim())
                                .OrderByDescending(s => s.Length)
                                .FirstOrDefault() ?? "";

                    // maxLevel: 그룹 내 최대 (없으면 1)
                    int maxLevel = 1;
                    foreach (var e in g)
                    {
                        if (e.MaxLevel > maxLevel) maxLevel = e.MaxLevel;
                    }
                    maxLevel = Mathf.Max(1, maxLevel);

                    // progressionType: 현재는 maxLevel로만 판별(추후 Stack 판별 규칙 확장 가능)
                    var progressionType = (maxLevel <= 1) ? "None" : "LevelUp";

                    // rarity: 임시 규칙
                    var rarity = MaxLevelToRarity(maxLevel);

                    // iconKey: Resources 경로 규칙
                    var iconKey = $"Icons/Items/{itemId}";

                    master.Items.Add(new ItemMasterRowJson
                    {
                        ItemId = itemId,
                        NameKr = nameKr,
                        Description = desc,
                        Rarity = rarity,
                        MaxLevel = maxLevel,
                        ProgressionType = progressionType,
                        IconKey = iconKey
                    });
                }

                // 출력
                var outPath = Path.Combine(streaming, OutputFileName);
                var outJson = JsonUtility.ToJson(master, true);
                File.WriteAllText(outPath, outJson);

                AssetDatabase.Refresh();

                EditorUtility.DisplayDialog(
                    "ItemMaster Generator",
                    $"Done!\n\nSource: {srcPath}\nOutput: {outPath}\nItems: {master.Items.Count}",
                    "OK");
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                EditorUtility.DisplayDialog("ItemMaster Generator", $"Failed:\n{ex.Message}", "OK");
            }
        }

        private static string MaxLevelToRarity(int maxLevel)
        {
            if (maxLevel <= 1) return "Common";
            if (maxLevel == 2) return "Rare";
            if (maxLevel == 3) return "Epic";
            return "Legendary";
        }

        #region Local DTO (Editor-only)

        [Serializable]
        private sealed class EffectsDbJson
        {
            public List<ItemEffectRowJson> effects;
        }

        [Serializable]
        private sealed class ItemEffectRowJson
        {
            // itemData.json 키와 반드시 일치해야 함 (camelCase 기준)
            public string ItemId;
            public string NameKr;
            public string Description;

            public int MaxLevel;
        }

        [Serializable]
        private sealed class ItemMasterDbJson
        {
            public List<ItemMasterRowJson> Items;
        }

        [Serializable]
        private sealed class ItemMasterRowJson
        {
            public string ItemId;
            public string NameKr;
            public string Description;

            public string Rarity;
            public int MaxLevel;

            public string ProgressionType;
            public string IconKey;
        }

        #endregion
    }
}
