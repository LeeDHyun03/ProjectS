using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using WaveStaticData;

using Random = UnityEngine.Random;
public class MonsterManager : MonoBehaviour
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


    public static MonsterManager instance;

    private int aliveMonsterCount = 0;

    public bool HasAliveMonster() => aliveMonsterCount > 0;

    // 테스트용
    private List<GameObject> mobs = new();

    // 몬스터 수 변화 추적용 이벤트 (인자는 업데이트 후 전체 몬스터 수)
    public event Action<int> OnChangedAliveMonsterCount;
    public event Action OnDeathMonster;


    public static MonsterManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameObject().AddComponent<MonsterManager>();
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
    public void SpawnWave(
        SpawnMethod spawnMethod,
        WaveStaticData.MonsterAmountInfo info,
        Vector2 center,
        float scaleFactor = 1
    )
    {
        Rect spawnRect = GetSpawnAreaRect(center, scaleFactor);

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
            StartCoroutine(SequentiallySpawnFromPoints(spawnQueue, points, center));
        }
        else ImmediatelySpawnFromPoints(spawnQueue, points, center);
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
            OnMonsterSuccessfullySpawned();
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
                if (index >= points.Count) break;

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
                OnMonsterSuccessfullySpawned();
            }
            if (spawnQueue.Count <= 0 || index >= points.Count) yield break;


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

    public Vector2 GetCurrentSpawnAreaCenter()
    {
        return GetSpawnAreaCenter(GetSpawnAreaWorldPoints());
    }

    (Vector3, Vector3) GetSpawnAreaWorldPoints()
    {
        return (
            Camera.main.ViewportToWorldPoint(new Vector3(0, 0)),
            Camera.main.ViewportToWorldPoint(new Vector3(1, 1))
        );
    }
    Vector2 GetSpawnAreaCenter((Vector3, Vector3) points)
    {
        return (points.Item1 + points.Item2) * 0.5f;
    }
    float GetMaxRadius()
    {
        (Vector3, Vector3) points = GetSpawnAreaWorldPoints();
        return Vector2.Distance(points.Item1, points.Item2) * 0.5f * cameraAreaMultiplier;
    }
    Rect GetSpawnAreaRect(Vector2 center, float scaleFactor = 1)
    {
        (Vector3, Vector3) points = GetSpawnAreaWorldPoints();

        Vector2 size = cameraAreaMultiplier * scaleFactor * (points.Item2 - points.Item1);

        return new Rect(center - size * 0.5f, size);
    }
    // 성공적으로 초기화된 몹의 경우에만 카운트함
    public void OnMonsterSuccessfullySpawned()
    {
        aliveMonsterCount++;
        OnChangedAliveMonsterCount?.Invoke(aliveMonsterCount);
    }
    public void OnMonsterDespawned()
    {
        aliveMonsterCount--;
        OnChangedAliveMonsterCount?.Invoke(aliveMonsterCount);
        OnDeathMonster?.Invoke();
    }
}