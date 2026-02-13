using System;
using System.Collections.Generic;
using System.Linq;
using Roguelike.Items;
using UnityEngine;

public class RarityWeight
{
    public EItemRarity rarity;
    public float weight;
}

public class ItemRandomSelector
{
    private static readonly Dictionary<EItemRarity, float> rarityChanceWeightData = new()
    {
        { EItemRarity.Common, 50 },
        { EItemRarity.Rare, 30 },
        { EItemRarity.Epic, 10 },
        { EItemRarity.Legendary, 5 }
    };
    private static readonly int resultItemCount = 3;
    private static float ApplyBias(EItemRarity rarity, float baseWeight, float rarityBias)
    {
        int rarityIndex = rarity switch
        {
            EItemRarity.Common => 0,
            EItemRarity.Rare => 1,
            EItemRarity.Epic => 2,
            EItemRarity.Legendary => 3,
            _ => 0
        };

        float multiplier = 1f + rarityBias * rarityIndex;
        return baseWeight * multiplier;
    }
    public static List<ItemData> PickItemsFromID(List<string> idList, int puzzleDifficulty = 0)
    {
        List<ItemData> candidates = new();

        foreach (string id in idList)
        {
            bool itemFound = ItemDataManager.Instance.TryGetItem(id, out var item);
            if (itemFound) candidates.Add(item);
        }

        return PickItems(candidates, puzzleDifficulty);
    }
    public static List<ItemData> PickItems(List<ItemData> candidates, int puzzleDifficulty = 0)
    {
        List<ItemData> list = new(candidates);
        List<ItemData> result = new();

        for (int i = 0; i < resultItemCount && list.Count > 0; i++)
        {
            ItemData selected = PickItem(list, puzzleDifficulty);
            result.Add(selected);
            list.Remove(selected);
        }

        return result;
    }
    private static ItemData PickItem(List<ItemData> list, int puzzleDifficulty)
    {

        float bias = puzzleDifficulty * 0.4f; // 0.4는 조절용
        // 현재 기준 어려움 퍼즐일 때 초월 획득 확률 5% -> 11%

        float totalWeight = 0f;

        foreach (ItemData item in list)
        {
            if (rarityChanceWeightData.TryGetValue(item.Rarity, out float baseWeight))
            {
                float finalWeight = ApplyBias(item.Rarity, baseWeight, bias);
                totalWeight += finalWeight;
            }
        }

        float randomValue = UnityEngine.Random.value * totalWeight;
        float cumulative = 0f;

        foreach (ItemData item in list)
        {
            if (!rarityChanceWeightData.TryGetValue(item.Rarity, out float baseWeight))
                continue;

            float finalWeight = ApplyBias(item.Rarity, baseWeight, bias);
            cumulative += finalWeight;

            if (randomValue <= cumulative)
                return item;
        }

        return list.Last();
    }
}
