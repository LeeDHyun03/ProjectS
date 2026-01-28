using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PoissonObjectManager : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private SpawnObject spawnObject;
    [SerializeField] private PaintTilemap paint;
    [SerializeField] private Tilemap tilemap;

    [Header("이미 배치한 오브젝트")]
    [SerializeField] private Transform prePlacedRoot;

    [Header("오브젝트 간격")]
    public float minInterval = 1f;
    public int rejectionSamples = 30;

    private readonly List<Vector2> _placedObj = new();
    private readonly Dictionary<int, List<Vector2>> _placedSameObj = new();

    private void Start()
    {
        StartCoroutine(DelayPlace());
    }

    private IEnumerator DelayPlace()
    {
        yield return null;
        PlaceObjects();
    }

    private void PlaceObjects()
    {
        if (spawnObject == null)
        {
            Debug.LogError("SpawnObject 참조 실패");
            return;
        }

        var rules = spawnObject.ObjectRules;
        if (rules == null || rules.Count == 0)
        {
            Debug.LogError("SpawnObject에 ObjectRules가 비어있음");
            return;
        }

        int w = paint.width;
        int h = paint.height;

        Vector3 cellSize = tilemap.layoutGrid.cellSize;
        Vector2 regionSize = new Vector2(w * cellSize.x, h * cellSize.y);

        Vector3Int offset = paint.CentorOffset;
        Vector3 originWorld = tilemap.CellToWorld(offset);
        Vector2 center = (Vector2)originWorld + regionSize * 0.5f;

        var points = PoissonDiscSampler.GeneratePoints(Mathf.Max(0.0001f, minInterval), regionSize, rejectionSamples, center);

        int failDensity = 0, failGlobalDist = 0, placedTotal = 0;

        _placedObj.Clear();
        _placedSameObj.Clear();
        prePlaced();

        for (int ri = 0; ri < rules.Count; ri++)
        {
            var rule = rules[ri];
            if (rule == null || rule.prefab == null)
                continue;

            if (rule.maxCount <= 0)
                continue;


            var shuffled = new List<Vector2>(points);
            Shuffle(shuffled);

            int placed = 0;
            float minSqAll = minInterval * minInterval;

            for (int i = 0; i < shuffled.Count && placed < rule.maxCount; i++)
            {
                if (Random.value > rule.density)
                {
                    failDensity++;
                    continue;
                }

                // 일반 푸아송
                Vector2 p = shuffled[i];

                // 타일맵 격자 푸아송
                Vector3Int cell = tilemap.WorldToCell(p);
                p = (Vector2)tilemap.GetCellCenterWorld(cell);

                if (!EnoughDist(_placedObj, p, minSqAll))
                {
                    failGlobalDist++;
                    continue;
                }

                // SpawnObject
                spawnObject.SpawnObjects(rule, p);

                _placedObj.Add(p);
                placed++;
                placedTotal++;

            }
        }

    }

    private void prePlaced()
    {
        if (prePlacedRoot == null)
            return;

        var trans = prePlacedRoot.GetComponentsInChildren<Transform>();


        for (int i = 0;i < trans.Length; i++)
        {
            var t= trans[i];
            if (t != null)
                continue;
            if (t == prePlacedRoot)
                continue;

            Vector2 p = t.position;

            Vector3Int cell = tilemap.WorldToCell(p);
            p = (Vector2)tilemap.GetCellCenterWorld(cell);
            
            _placedObj.Add(p);
        }
    }

    private static bool EnoughDist(List<Vector2> placed, Vector2 candidate, float minSq)
    {
        for (int i = 0; i < placed.Count; i++)
        {
            Vector2 dist = candidate - placed[i];
            if (dist.sqrMagnitude < minSq)
                return false;
        }
        return true;
    }

    private static void Shuffle<T>(IList<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
