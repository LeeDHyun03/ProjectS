using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerStateManager : MonoBehaviour
{
    public static PlayerStateManager Instance;

    [Serializable]
    public sealed class RunItemSnapshot
    {
        public string itemId;
        public int level;
        public int stacks;
    }

    [Serializable]
    public sealed class PlayerRunSnapshot
    {
        public float hp;
        public float maxHp;
        public List<RunItemSnapshot> items = new();
        public Vector2 position;
    }


    [Header("Auto Restore")]
    [SerializeField] private string fieldSceneName = "Field";
    [SerializeField] private bool autoRestoreOnFieldLoaded = true;

    private PlayerRunSnapshot snapshot;

    public bool HasSnapshot => snapshot != null;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void SaveFrom(PlayerCharacter player)
    {
        if (player == null) return;

        var itemCtrl = player.GetComponent<PlayerItemStatController>();

        snapshot = new PlayerRunSnapshot
        {
            hp = player.GetCurrentHp,
            maxHp = player.GetMaxHp,
            position = player.transform.position,
            items = new List<RunItemSnapshot>()
        };

        if (itemCtrl != null)
        {
            foreach (var (ItemId, Level, Stacks) in itemCtrl.EnumerateRunItems())
            {
                if (string.IsNullOrWhiteSpace(ItemId)) continue;

                snapshot.items.Add(new RunItemSnapshot
                {
                    itemId = ItemId,
                    level = Mathf.Max(1, Level),
                    stacks = Mathf.Max(1, Stacks)
                });
            }
        }
    }

    public void RestoreTo(PlayerCharacter player)
    {
        if (player == null) return;
        if (snapshot == null) return;

        player.SetHp(snapshot.hp, snapshot.maxHp);
        player.transform.position = snapshot.position;

        var itemCtrl = player.GetComponent<PlayerItemStatController>();
        if (itemCtrl != null)
        {
            var list = new List<PlayerItemStatController.RunItem>();
            foreach (var it in snapshot.items)
            {
                list.Add(new PlayerItemStatController.RunItem
                {
                    itemId = it.itemId,
                    level = it.level,
                    stacks = it.stacks
                });
            }

            itemCtrl.LoadRunItems(list);
        }
    }

    public void ClearSnapshot()
    {
        snapshot = null;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!autoRestoreOnFieldLoaded) return;
        if (scene.name != fieldSceneName) return;
        if (snapshot == null) return;

        var player = FindFirstObjectByType<PlayerCharacter>();
        if (player != null)
        {
            RestoreTo(player);
        }
    }
}