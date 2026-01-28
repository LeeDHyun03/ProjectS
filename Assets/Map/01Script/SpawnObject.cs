using System;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObject : MonoBehaviour
{
    [SerializeField] private Transform parent;

    [SerializeField] private List<ObjectRule> objectRules = new();
    public IReadOnlyList<ObjectRule> ObjectRules => objectRules;

    [Serializable]
    public class ObjectRule
    {
        public GameObject prefab;

        [Range(0f, 1f)]
        public float density = 1f;

        public int maxCount = 0;

        public float minDistance = 0f;
    }

    public void SpawnObjects(ObjectRule rule, Vector2 pos)
    {
        if (rule == null || rule.prefab == null)
            return;

        Vector3 spawnPos = new Vector3(pos.x, pos.y, 0);
        Instantiate(rule.prefab, spawnPos, Quaternion.identity, parent);
    }
}
