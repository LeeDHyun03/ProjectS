using System;
using System.Collections.Generic;
using UnityEngine;

public class PortalManager : MonoBehaviour
{
    [Serializable]
    public class PuzzleCatalogEntry
    {
        public SceneFlowManager.GameScene scene;
        public string displayName;
        public string difficulty;
    }

    [Header("Portals (size=10)")]
    [SerializeField] private List<DungeonPortal> portals = new();

    [Header("Active Count")]
    [SerializeField] private int activeCount = 5;

    [Header("Puzzle Catalog")]
    [SerializeField] private List<PuzzleCatalogEntry> catalog = new();

    private System.Random rng;

    // 현재 활성 포탈 id 집합
    private readonly HashSet<int> activePortalIds = new();

    private void Awake()
    {
        rng = new System.Random(Environment.TickCount);
    }

    private void Start()
    {
        InitializeRandomActivePortals();
    }
    /// <summary>
    /// 게임 시작/밤 시작 시 초기 5개 활성화가 필요하면 호출
    /// </summary>
    public void InitializeRandomActivePortals()
    {
        activePortalIds.Clear();

        // 전부 끄기
        foreach (var p in portals)
            if (p != null) p.SetActivePortal(false);

        int count = Mathf.Clamp(activeCount, 0, portals.Count);
        var chosen = PickUniqueIndices(portals.Count, count);

        foreach (int idx in chosen)
        {
            var p = portals[idx];
            if (p == null) continue;

            AssignNewPuzzleInfo(p);
            p.SetActivePortal(true);
            activePortalIds.Add(p.PortalId);
        }
    }

    /// <summary>
    /// 퍼즐을 클리어했을 때 호출.
    /// - 방금 사용한 포탈 비활성화
    /// - 비활성 포탈 중 1개 랜덤 활성화 + 정보 덮어쓰기
    /// </summary>
    public void OnPuzzleCleared(int clearedPortalId)
    {
        // 1) 방금 사용한 포탈 OFF
        var cleared = FindPortalById(clearedPortalId);
        if (cleared != null)
        {
            cleared.SetActivePortal(false);
            activePortalIds.Remove(clearedPortalId);
        }

        // 2) 비활성 포탈들 수집
        var inactive = new List<DungeonPortal>();
        foreach (var p in portals)
        {
            if (p == null) continue;
            if (!activePortalIds.Contains(p.PortalId))
                inactive.Add(p);
        }

        if (inactive.Count == 0)
        {
            // 이미 전부 활성 같은 이상 상태(설정 오류)
            Debug.LogWarning("OnPuzzleCleared: no inactive portal to activate.");
            return;
        }

        // 3) 비활성 중 1개 랜덤 선택하여 ON
        var newPortal = inactive[rng.Next(0, inactive.Count)];
        AssignNewPuzzleInfo(newPortal);
        newPortal.SetActivePortal(true);
        activePortalIds.Add(newPortal.PortalId);

        // 4) 안전장치: 활성 5개 유지(혹시 activeCount 변경/오류 대비)
        TrimOrFillToActiveCount();
    }

    private void TrimOrFillToActiveCount()
    {
        int desired = Mathf.Clamp(activeCount, 0, portals.Count);

        // 부족하면 채우기
        while (activePortalIds.Count < desired)
        {
            var inactive = new List<DungeonPortal>();
            foreach (var p in portals)
                if (p != null && !activePortalIds.Contains(p.PortalId))
                    inactive.Add(p);

            if (inactive.Count == 0) break;

            var p2 = inactive[rng.Next(0, inactive.Count)];
            AssignNewPuzzleInfo(p2);
            p2.SetActivePortal(true);
            activePortalIds.Add(p2.PortalId);
        }

        // 초과하면 줄이기
        while (activePortalIds.Count > desired)
        {
            // 임의로 하나 끄기
            int pickId = First(activePortalIds);
            var p = FindPortalById(pickId);
            if (p != null) p.SetActivePortal(false);
            activePortalIds.Remove(pickId);
        }
    }

    private DungeonPortal FindPortalById(int portalId)
    {
        foreach (var p in portals)
            if (p != null && p.PortalId == portalId)
                return p;
        return null;
    }

    private void AssignNewPuzzleInfo(DungeonPortal portal)
    {
        if (portal == null) return;
        if (catalog == null || catalog.Count == 0)
        {
            Debug.LogWarning("PortalManager: catalog empty.");
            return;
        }

        // 퍼즐 선택
        var entry = catalog[rng.Next(0, catalog.Count)];


        portal.SetPuzzleInfo(entry.scene, entry.displayName, entry.difficulty);
    }

    private List<int> PickUniqueIndices(int range, int count)
    {
        var list = new List<int>(range);
        for (int i = 0; i < range; i++) list.Add(i);

        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = rng.Next(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }

        if (count < list.Count)
            list.RemoveRange(count, list.Count - count);

        return list;
    }

    private static int First(HashSet<int> set)
    {
        foreach (var v in set) return v;
        return -1;
    }
}