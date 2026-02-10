using System.Collections.Generic;
using UnityEngine;

public static class RectAreaPoissonDiskSampler
{
    public static List<Vector2> GeneratePoints(
        float radius,
        Rect region,
        int rejectionSamples,
        Vector2 excludeCenter,
        float excludeRadius,
        int fallbackPointCount = 100
    )
    {
        float cellSize = radius / Mathf.Sqrt(2);

        int gridWidth = Mathf.CeilToInt(region.width / cellSize);
        int gridHeight = Mathf.CeilToInt(region.height / cellSize);
        int[,] grid = new int[gridWidth, gridHeight];

        List<Vector2> points = new();
        List<Vector2> spawnPoints = new();

        Vector2 start;
        int guard = 0;

        do
        {
            start = new Vector2(
                Random.Range(region.xMin, region.xMax),
                Random.Range(region.yMin, region.yMax)
            );
            guard++;
        }
        while (
            Vector2.Distance(start, excludeCenter) < excludeRadius &&
            guard < 100
        );

        if (guard >= 100)
            start = region.center;

        spawnPoints.Add(start);

        while (spawnPoints.Count > 0)
        {
            int spawnIndex = Random.Range(0, spawnPoints.Count);
            Vector2 spawnCenter = spawnPoints[spawnIndex];
            bool accepted = false;

            for (int i = 0; i < rejectionSamples; i++)
            {
                float angle = Random.value * Mathf.PI * 2f;
                Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                Vector2 candidate =
                    spawnCenter + dir * Random.Range(radius, radius * 2f);

                if (!region.Contains(candidate))
                    continue;

                if (Vector2.Distance(candidate, excludeCenter) < excludeRadius)
                    continue;

                if (IsValid(candidate))
                {
                    points.Add(candidate);
                    spawnPoints.Add(candidate);

                    int cx = GetCellX(candidate);
                    int cy = GetCellY(candidate);
                    grid[cx, cy] = points.Count;

                    accepted = true;
                    break;
                }
            }

            if (!accepted)
                spawnPoints.RemoveAt(spawnIndex);
        }

        points = SpatiallyMix(points, region.center);

        if (points.Count == 0)
        {
            Debug.LogWarning("샘플링 실패 (대신 무작위 배치 사용)");

            for (int i = 0; i < fallbackPointCount; i++)
            {
                Vector2 p = new(
                    Random.Range(region.xMin, region.xMax),
                    Random.Range(region.yMin, region.yMax)
                );

                if (Vector2.Distance(p, excludeCenter) >= excludeRadius)
                    points.Add(p);
            }
        }

        return points;

        bool IsValid(Vector2 candidate)
        {
            int cellX = GetCellX(candidate);
            int cellY = GetCellY(candidate);

            for (int x = Mathf.Max(0, cellX - 2); x <= Mathf.Min(cellX + 2, gridWidth - 1); x++)
                for (int y = Mathf.Max(0, cellY - 2); y <= Mathf.Min(cellY + 2, gridHeight - 1); y++)
                {
                    int index = grid[x, y] - 1;
                    if (index >= 0)
                    {
                        if (Vector2.Distance(candidate, points[index]) < radius)
                            return false;
                    }
                }
            return true;
        }

        int GetCellX(Vector2 p)
            => Mathf.FloorToInt((p.x - region.xMin) / cellSize);

        int GetCellY(Vector2 p)
            => Mathf.FloorToInt((p.y - region.yMin) / cellSize);
    }

    // 한 지점을 기준으로 너무 몰리지 않게끔 분산
    static List<Vector2> SpatiallyMix(List<Vector2> points, Vector2 center)
    {
        if (points.Count <= 2)
            return points;

        List<Vector2> inner = new();
        List<Vector2> middle = new();
        List<Vector2> outer = new();

        float maxDist = 0f;
        foreach (var p in points)
            maxDist = Mathf.Max(maxDist, Vector2.Distance(center, p));

        foreach (var p in points)
        {
            float d = Vector2.Distance(center, p);

            if (d < maxDist * 0.33f)
                inner.Add(p);
            else if (d < maxDist * 0.66f)
                middle.Add(p);
            else
                outer.Add(p);
        }

        List<Vector2> shuffledInner = inner.Shuffle();
        List<Vector2> shuffledMiddle = middle.Shuffle();
        List<Vector2> shuffledOuter = outer.Shuffle();


        List<Vector2> mixed = new();
        int max = Mathf.Max(shuffledInner.Count, shuffledMiddle.Count, shuffledOuter.Count);

        for (int i = 0; i < max; i++)
        {
            if (i < shuffledInner.Count) mixed.Add(shuffledInner[i]);
            if (i < shuffledMiddle.Count) mixed.Add(shuffledMiddle[i]);
            if (i < shuffledOuter.Count) mixed.Add(shuffledOuter[i]);
        }

        return mixed;
    }

}
