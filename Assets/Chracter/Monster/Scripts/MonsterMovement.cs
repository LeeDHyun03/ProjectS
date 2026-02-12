using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MonsterMovement : MonoBehaviour
{
    private List<Vector3> path = new();
    private List<Vector3> currentPath = new();

    private Character Target;

    private float moveSpeed = 3f;

    private Vector3 direction = Vector3.zero;

    public event Action OnArrivedPath;

    private bool isWaiting = true;
    public void SetWaiting(bool waiting) => isWaiting = waiting;

    public bool GetWaiting() => isWaiting;

    public void SetupPath(List<Vector3> newPath)
    {
        path = newPath;
        currentPath.Clear();
    }
    public void SetupMovement(float newMoveSpeed)
    {
        moveSpeed = newMoveSpeed;
    }

    public void SetTarget(Character newTarget)
    {
        Target = newTarget;
    }

    public void ClearTarget()
    {
        Target = null;
    }

    private void MoveToTarget()
    {
        direction = (Target.transform.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    private void MoveToPath()
    {
        if (currentPath.Count == 0)
        {
            currentPath = path;
        }
        Vector3 nextPoint = currentPath[0];
        direction = (nextPoint - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
        if (Vector3.Distance(transform.position, nextPoint) < 0.1f)
        {
            currentPath.RemoveAt(0);
            OnArrivedPath?.Invoke();
        }
    }

    void Update()
    {
        if (isWaiting) return;

        if (Target != null)
        {
            MoveToTarget();
        }
        else if (path.Count > 0)
        {
            MoveToPath();
        }
    }
}
