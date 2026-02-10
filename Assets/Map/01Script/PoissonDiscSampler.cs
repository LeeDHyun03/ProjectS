using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Random;

public class PoissonDiscSampler
{
    public static List<Vector2> GeneratePoints(float radius, Vector2 regionSize, int rejectionSamples = 30, Vector2 center = default)
    {
        if (radius <= 0f)
            throw new System.ArgumentException("범위는 0보다 작을 수 없습니다.");
        if (regionSize.x < 0f || regionSize.y < 0f)
            throw new System.ArgumentException("영역의 크기는 0보다 작을 수 없습니다.");
        if (rejectionSamples < 1)
            rejectionSamples = 1;

        float cellSize = radius / Mathf.Sqrt(2f);

        int gridx = Mathf.CeilToInt(regionSize.x / cellSize);
        int gridy = Mathf.CeilToInt(regionSize.y / cellSize);

        int[,] grid = new int[gridx, gridy];

        List<Vector2> points = new();
        List<Vector2> active = new();

        Vector2 half = regionSize * 0.5f;
        Vector2 origin = center - half;

        Vector2 first = origin + new Vector2(value * regionSize.x, value * regionSize.y);

        points.Add(first);
        active.Add(first);
        grid[ToGridX(first, origin, cellSize), ToGridY(first, origin, cellSize)] = points.Count;

        while (active.Count > 0)
        {
            int activeIndex = Random.Range(0, active.Count);
            Vector2 spawnCenter = active[activeIndex];
            bool accepted = false;

            for (int i = 0; i < rejectionSamples; i++)
            {
                float angle = value * Mathf.PI * 2f;
                float dist = radius * (1f + value);
                Vector2 candidate = spawnCenter + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * dist;

                if (!IsSamplingArea(candidate, origin, regionSize))
                    continue;

                if (IsValid(candidate, origin, regionSize, cellSize, radius, points, grid))
                {
                    points.Add(candidate);
                    active.Add(candidate);
                    grid[ToGridX(candidate, origin, cellSize), ToGridY(candidate, origin, cellSize)] = points.Count;
                    accepted = true;
                    break;
                }
            }
            if (!accepted)
                active.RemoveAt(activeIndex);
        }

        return points;
    }

    private static bool IsSamplingArea(Vector2 point, Vector2 origin, Vector2 regionSize)
    {
        return
            point.x >= origin.x && point.y >= origin.y && point.x < origin.x + regionSize.x && point.y < origin.y + regionSize.y;
    }

    private static int ToGridX(Vector2 point, Vector2 origin, float cellSize)
        => Mathf.FloorToInt((point.x - origin.x) / cellSize);

    private static int ToGridY(Vector2 point, Vector2 origin, float cellSize)
        => Mathf.FloorToInt((point.y - origin.y) / cellSize);

    private static bool IsValid(Vector2 candidate, Vector2 origin, Vector2 regionSize, float cellSize, float radius, List<Vector2> points, int[,] grid)
    {
        int gridx = grid.GetLength(0);
        int gridy = grid.GetLength(1);

        int cx = ToGridX(candidate, origin, cellSize);
        int cy = ToGridY(candidate, origin, cellSize);

        if (cx < 0 || cy < 0 || cx >= gridx || cy >= gridy)
            return false;

        int startX = Mathf.Max(0, cx - 2);
        int endX = Mathf.Min(gridx - 1, cx + 2);
        int startY = Mathf.Max(0, cy - 2);
        int endY = Mathf.Min(gridy - 1, cy + 2);

        float r2 = radius * radius;

        for (int x = startX; x <= endX; x++)
            for (int y = startY; y <= endY; y++)
            {
                int IndexPlus = grid[x, y];
                if (IndexPlus == 0)
                    continue;
                Vector2 point = points[IndexPlus - 1];
                float dx = candidate.x - point.x;
                float dy = candidate.y - point.y;

                if (dx * dx + dy * dy < r2)
                    return false;
            }

        return true;
    }
}
