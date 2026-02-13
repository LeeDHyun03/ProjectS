using System.Collections.Generic;
using UnityEngine;

public sealed class DevItemStatTest : MonoBehaviour
{
    [SerializeField] private PlayerCharacter player;
    [SerializeField] private List<string> testItemIds;
    [SerializeField] private float hitDamage = 20f;

    private PlayerItemStatController _itemStats;

    private void Start()
    {
        if (!player) player = FindFirstObjectByType<PlayerCharacter>();

        _itemStats = player ? player.GetComponent<PlayerItemStatController>() : null;

        Invoke("StartLoad", 3);


    }

    private void StartLoad()
    {
        Debug.Log($"[DevItemStatTest] player={player}, itemStats={_itemStats}");
        for (int i = 0; i < testItemIds.Count; i++)
        {
            if (_itemStats != null && !string.IsNullOrWhiteSpace(testItemIds[i]))
            {
                _itemStats.AddItem(testItemIds[i], 1);
                Debug.Log($"[DevItemStatTest] Added item: {testItemIds[i]}");
            }
        }
        DumpStats("After AddItem");
    }

    private void Update()
    {
        if (!player) return;

        if (Input.GetKeyDown(KeyCode.K))
        {
            player.TakeDamage(hitDamage);
            DumpStats($"After TakeDamage({hitDamage})");
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            player.CureHp(20);
            Debug.Log("[DevItemStatTest] R pressed.");
            DumpStats("Manual Dump");
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            DumpStats("Manual Dump");
        }
    }

    private void DumpStats(string tag)
    {
        if (!player) return;

        Debug.Log(
            $"[DevItemStatTest] {tag}\n" +
            $"HP {player.CurrentHp}/{player.MaxHp} ({player.CurrentHp / player.MaxHp * 100f:0.0}%)\n" +
            $"Atk={player.DebugAttackDamage}, Spd={player.DebugMoveSpeed}, AS={player.DebugAttackSpeed}" +
            $"CritChance={player.CritChance}"
        );
        Debug.Log($"CritChance{player.CritChance}");
    }
}
