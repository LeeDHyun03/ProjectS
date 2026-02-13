using System.Collections;
using UnityEngine;

public class BossPinpointPattern : MonoBehaviour
{
    [SerializeField] private BossAnimatorController controller;
    [SerializeField] private string circleIndicatorPoolKey = "AttackCircleIndicator"; // ObjectPooler 키
    [SerializeField] private float minPreDelay = 1f;
    [SerializeField] private float maxPreDelay = 6f;
    [SerializeField] private float spawnInterval = 0.4f;
    [SerializeField] private int count = 7;

    [SerializeField] private float radiusTiles = 6f;
    [SerializeField] private float aoeRadius = 1.5f; // 3x3 원형 느낌
    [SerializeField] private float damage = 14f;
    [SerializeField] private LayerMask targetMask;

    private ContactFilter2D targetFilter;
    private readonly Collider2D[] hitBuffer = new Collider2D[16];

    private void Awake()
    {
        targetFilter = new ContactFilter2D { useLayerMask = true, useTriggers = true };
        targetFilter.SetLayerMask(targetMask);
    }

    public IEnumerator Execute(Transform target)
    {
        if (target == null) yield break;

        controller.Pinpoint();

        for (int i = 0; i < count; i++)
        {
            Vector2 pos = (Vector2)target.position + Random.insideUnitCircle * radiusTiles;

            GameObject meteoObject = ObjectPooler.Instance.SpawnFromPool("Meteo", pos, Quaternion.identity);
            if (meteoObject == null) yield break;
            
            meteoObject.TryGetComponent<Meteo>(out Meteo meteo);
            if(meteoObject == null) yield break;

            float preDelay = Random.Range(minPreDelay, maxPreDelay);

            meteo.SetDefaultValue(pos, damage, aoeRadius, false, preDelay);
            meteo.StartFall();

            // 원형 인디케이터 Spawn
            GameObject go = ObjectPooler.Instance.SpawnFromPool(circleIndicatorPoolKey, pos, Quaternion.identity);
            go.TryGetComponent<IAttackIndicator>(out IAttackIndicator indicator);
            if(indicator == null) yield break;

            // size는 “지름” 기준으로 주는 게 가장 직관적
            float diameter = aoeRadius * 2f;
            indicator.StartIndicator(new Vector2(diameter, diameter), preDelay);

            indicator.OnIndicatorComplete += () =>
            {
                // 인디케이터 완료 시점 = 낙하 판정 시점
                GameObject meteoObject = ObjectPooler.Instance.SpawnFromPool("ExplosionEffect", pos, Quaternion.identity);
                ApplyCircleDamage(pos, aoeRadius);
            };

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void ApplyCircleDamage(Vector2 pos, float radius)
    {
        int n = Physics2D.OverlapCircle(pos, radius, targetFilter, hitBuffer);
        for (int i = 0; i < n; i++)
        {
            var col = hitBuffer[i];
            if (col == null) continue;

            col.TryGetComponent<PlayerCharacter>(out PlayerCharacter character);
            if (character != null) character.TakeDamage(damage);
        }
    }
}