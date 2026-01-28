using System;
using UnityEngine;

[Serializable]
public class ObjectRule
{
    public string name;
    public GameObject prefab;

    [Header("수/조절")]
    [Range(0f, 1f)] public float density = 1f;
    public int maxCount = 0;

    [Header("최소거리")]
    public float minDistance = 0f;
}
