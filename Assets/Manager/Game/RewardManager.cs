using NUnit.Framework;
using Roguelike.Items;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RewardManager : MonoBehaviour
{
    public static RewardManager Instance;

    private RewardScreen rewardScreen;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }
    }

    private static readonly Dictionary<EItemRarity, float> rarityChanceWeightData = new()
    {
        { EItemRarity.Common, 50 },
        { EItemRarity.Rare, 30 },
        { EItemRarity.Epic, 10 },
        { EItemRarity.Legendary, 5 }
    };

    public static List<ItemData> PickItemsFromID(List<string> idList, int count = 3)
    {
        List<ItemData> candidates = new();

        foreach (string id in idList)
        {
            bool itemFound = ItemDataManager.Instance.TryGetItem(id, out var item);
            Dbg.L("찾음?", itemFound);
            if (itemFound) candidates.Add(item);
        }

        return PickItems(candidates, count);
    }

    public static List<ItemData> PickItems(List<ItemData> candidates, int count = 3)
    {
        List<ItemData> list = new(candidates);
        List<ItemData> result = new();

        for (int i = 0; i < count && list.Count > 0; i++)
        {
            ItemData selected = PickItem(list);
            result.Add(selected);
            list.Remove(selected);
        }

        return result;
    }

    private static ItemData PickItem(List<ItemData> list)
    {
        float totalWeight = 0f;

        foreach (ItemData item in list)
        {
            if (rarityChanceWeightData.TryGetValue(item.Rarity, out float weight)) totalWeight += weight;
        }

        float randomValue = UnityEngine.Random.value * totalWeight;
        float cumulative = 0f;

        foreach (ItemData item in list)
        {
            if (!rarityChanceWeightData.TryGetValue(item.Rarity, out float weight)) continue;

            cumulative += weight;

            if (randomValue <= cumulative) return item;
        }

        return list.Last();
    }
}
