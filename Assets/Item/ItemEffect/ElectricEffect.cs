using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricEffect : MonoBehaviour
{
    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void Draw(List<Vector3> points)
    {
        StopAllCoroutines();
        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());

        StartCoroutine(ElectricCleanRoutine());
    }

    private IEnumerator ElectricCleanRoutine()
    {
        yield return new WaitForSeconds(0.5f);

        int totalPositions = lineRenderer.positionCount;
        Vector3[] positions = new Vector3[totalPositions];
        lineRenderer.GetPositions(positions);

        for (int i = 0; i < totalPositions; i++)
        {
            yield return new WaitForSeconds(0.2f);

            int remainingCount = totalPositions - (i + 1);
            if (remainingCount > 0)
            {
                Vector3[] newPositions = new Vector3[remainingCount];
                System.Array.Copy(positions, i + 1, newPositions, 0, remainingCount);
                lineRenderer.positionCount = remainingCount;
                lineRenderer.SetPositions(newPositions);
            }
            else
            {
                lineRenderer.positionCount = 0;
            }
        }

        ObjectPooler.ReturnToPool(gameObject);
    }
}