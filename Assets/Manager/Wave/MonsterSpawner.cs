using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public GameObject knightPrefab;
    public GameObject archerPrefab;
    public GameObject spearManPrefab;
    public GameObject magePrefab;

    public float minDistance = 10f;
    public int rejectionSamples = 30;

    public float cameraAreaMultiplier = 1.2f;
    public float centerExclusionRadius = 3f;

    [Range(0f, 1f)] public float outerMinRatio = 0.75f;

    public Vector2 spawnIntervalRange = new Vector2(0.1f, 0.4f);
    public Vector2Int spawnBatchRange = new Vector2Int(1, 3);

    public int enemyThreshold = 50;

    private Camera _cam;


    public static MonsterSpawner Instance;


    void Awake()
    {
        Instance = this;
        _cam = Camera.main;
    }
    public void SpawnWaveFallback(WaveStaticData.MonsterAmountInfo pendingWaveAmountInfo, float areaScaleFactor = 1)
    {
        Rect spawnRect = GetSpawnAreaRect(areaScaleFactor);

        List<Vector2> points = new();
        Queue<string> spawnQueue = new();

        for (int i = 0; i < pendingWaveAmountInfo.GetTotalAmount(); i++)
        {
            Vector2 p = new(
                Random.Range(spawnRect.xMin, spawnRect.xMax),
                Random.Range(spawnRect.yMin, spawnRect.yMax)
            );

            if (Vector2.Distance(p, spawnRect.center) >= centerExclusionRadius)
                points.Add(p);
        }

        List<string> shuffledWaveMonsterList = pendingWaveAmountInfo
            .ToDictionary()
            .SelectMany(x => Enumerable.Repeat(x.Key, x.Value))
            .ToList()
            .Shuffle();

        foreach (var id in shuffledWaveMonsterList)
        {
            spawnQueue.Enqueue(id);
        }

        ImmediatelySpawnFromPoints(spawnQueue, points, spawnRect.center);

    }
    public void SpawnWave(WaveStaticData.MonsterAmountInfo info, float scaleFactor = 1)
    {
        Rect spawnRect = GetSpawnAreaRect(scaleFactor);
        bool areaExpended = false;

        /*
        while (CountEnemiesInRect(spawnRect) >= enemyThreshold)
        {
            spawnRect = GetExpandedOuterRect(spawnRect, 1.8f);
            areaExpended = true;
        } */

        List<Vector2> points = RectAreaPoissonDiskSampler.GeneratePoints(
            minDistance,
            spawnRect,
            rejectionSamples,
            spawnRect.center,
            areaExpended ? Mathf.Min(spawnRect.width, spawnRect.height) * 0.5f : centerExclusionRadius,
            info.GetTotalAmount() > 30 ? info.GetTotalAmount() : 30
        );

        Queue<string> spawnQueue = new Queue<string>();

        List<string> shuffledWaveMonsterList = info
            .ToDictionary()
            .SelectMany(x => Enumerable.Repeat(x.Key, x.Value))
            .ToList()
            .Shuffle();

        foreach (var id in shuffledWaveMonsterList)
        {
            spawnQueue.Enqueue(id);
        }

        StartCoroutine(SequentiallySpawnFromPoints(spawnQueue, points, spawnRect.center));
    }

    void ImmediatelySpawnFromPoints(
        Queue<string> spawnQueue,
        List<Vector2> points,
        Vector2 center
    )
    {
        int index = 0;
        while (spawnQueue.Count > 0 && index < points.Count)
        {
            int batch = Random.Range(spawnBatchRange.x, spawnBatchRange.y + 1);

            for (int i = 0; i < batch && spawnQueue.Count > 0; i++)
            {
                string id = spawnQueue.Dequeue();

                Vector2 pos;
                int guard = 0;

                do
                {
                    pos = points[index++];
                    guard++;
                }
                while (
                    id == "Archer" &&
                    !IsOuterArea(pos, center) &&
                    index < points.Count
                // guard < 50
                );
                // TODO: 오브젝트 풀링
                Instantiate(GetPrefab(id), pos, Quaternion.identity);
            }
        }
    }

    IEnumerator SequentiallySpawnFromPoints(
        Queue<string> spawnQueue,
        List<Vector2> points,
        Vector2 center
    )
    {
        int index = 0;
        while (spawnQueue.Count > 0 && index < points.Count)
        {
            int batch = Random.Range(spawnBatchRange.x, spawnBatchRange.y + 1);

            for (int i = 0; i < batch && spawnQueue.Count > 0; i++)
            {
                string id = spawnQueue.Dequeue();

                Vector2 pos;
                int guard = 0;

                do
                {
                    pos = points[index++];
                    guard++;
                }
                while (
                    id == "Archer" &&
                    !IsOuterArea(pos, center) &&
                    index < points.Count &&
                    guard < 50
                );
                // TODO: 오브젝트 풀링
                Instantiate(GetPrefab(id), pos, Quaternion.identity);
            }

            float wait = Random.Range(spawnIntervalRange.x, spawnIntervalRange.y);
            yield return new WaitForSeconds(wait);
        }
    }

    GameObject GetPrefab(string id)
    {
        return id switch
        {
            "Knight" => knightPrefab,
            "Archer" => archerPrefab,
            "Mage" => magePrefab,
            "SpearMan" => spearManPrefab,
            _ => throw new System.Exception("올바르지 않은 몬스터 ID")
        };
    }

    bool IsOuterArea(Vector2 pos, Vector2 center)
    {
        float maxRadius = GetMaxRadius();
        float dist = Vector2.Distance(pos, center);
        return dist >= maxRadius * outerMinRatio;
    }

    float GetMaxRadius()
    {
        Vector3 bl = _cam.ViewportToWorldPoint(new Vector3(0, 0));
        Vector3 tr = _cam.ViewportToWorldPoint(new Vector3(1, 1));
        return Vector2.Distance(bl, tr) * 0.5f * cameraAreaMultiplier;
    }

    Rect GetSpawnAreaRect(float scaleFactor = 1)
    {
        Vector3 bl = _cam.ViewportToWorldPoint(new Vector3(0, 0));
        Vector3 tr = _cam.ViewportToWorldPoint(new Vector3(1, 1));

        Vector2 size = (tr - bl) * cameraAreaMultiplier * scaleFactor;
        Vector2 center = (bl + tr) * 0.5f;

        return new Rect(center - size * 0.5f, size);
    }
}