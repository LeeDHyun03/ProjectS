using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using WaveStaticData;

public class MonsterSpawner : MonoBehaviour
{

    public enum SpawnMethod
    {
        RandomFallback,
        PoissonDiscSampling
    }
    // 주의: minDistance가 지나치게 크면 뽑히는 샘플 포인트 수가 크게 적어져 문제 생길 수 있음
    // minDistance = 2일 때 평균 샘플 수 50
    // minDistance = 10일 때 평균 샘플 수 3
    public float minDistance = 2.5f;
    public int rejectionSamples = 30;

    public float cameraAreaMultiplier = 1.2f;
    public float centerExclusionRadius = 1.5f;

    [Range(0f, 1f)] public float outerMinRatio = 0.75f;

    public Vector2 spawnIntervalRange = new Vector2(0.2f, 0.75f);
    public Vector2Int spawnBatchRange = new Vector2Int(1, 3);

    public int enemyThreshold = 50;

    private Camera _cam;


    public static MonsterSpawner instance;

    // 테스트용
    private List<GameObject> mobs = new();


    void Awake()
    {
        _cam = Camera.main;
    }
    public static MonsterSpawner Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameObject().AddComponent<MonsterSpawner>();
            }
            return instance;
        }
    }
    public void Test_ClearMobs()
    {
        foreach (var obj in mobs)
        {
            ObjectPooler.ReturnToPool(obj);
        }
    }
    public void SpawnWaveFallbackLegacy(WaveStaticData.MonsterAmountInfo pendingWaveAmountInfo, float areaScaleFactor = 1)
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
    public void SpawnWave(
        SpawnMethod spawnMethod,
        WaveStaticData.MonsterAmountInfo info,
        float scaleFactor = 1
    )
    {
        Rect spawnRect = GetSpawnAreaRect(scaleFactor);

        /*
        while (CountEnemiesInRect(spawnRect) >= enemyThreshold)
        {
            spawnRect = GetExpandedOuterRect(spawnRect, 1.8f);
            areaExpended = true;
        } */

        List<Vector2> points;

        if (spawnMethod == SpawnMethod.PoissonDiscSampling)
        {
            points = RectAreaPoissonDiskSampler.GeneratePoints(
                minDistance,
                spawnRect,
                rejectionSamples,
                spawnRect.center,
                centerExclusionRadius,
                info.GetTotalAmount() > 30 ? info.GetTotalAmount() : 30
            );
        }
        else
        {
            float sqrRadius = centerExclusionRadius * centerExclusionRadius;
            Vector2 center = spawnRect.center;

            points = Enumerable.Range(0, info.GetTotalAmount())
                .Select(_ => new Vector2(
                     Random.Range(spawnRect.xMin, spawnRect.xMax),
                    Random.Range(spawnRect.yMin, spawnRect.yMax)
                ))
                .Where(p => (p - center).sqrMagnitude >= sqrRadius)
                .ToList();
        }
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

        if (spawnMethod == SpawnMethod.PoissonDiscSampling)
        {
            StartCoroutine(SequentiallySpawnFromPoints(spawnQueue, points, spawnRect.center));
        }
        else ImmediatelySpawnFromPoints(spawnQueue, points, spawnRect.center);
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
            mobs.Add(ObjectPooler.Instance.SpawnFromPool(id, pos, Quaternion.identity));
            // Instantiate(GetPrefab(id), pos, Quaternion.identity);
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
                mobs.Add(ObjectPooler.Instance.SpawnFromPool(id, pos, Quaternion.identity));
                // Instantiate(GetPrefab(id), pos, Quaternion.identity);
            }

            float wait = Random.Range(spawnIntervalRange.x, spawnIntervalRange.y);
            yield return new WaitForSeconds(wait);
        }
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