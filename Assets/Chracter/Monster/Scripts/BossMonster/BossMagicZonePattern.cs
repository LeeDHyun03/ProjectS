using System.Collections;
using UnityEngine;

public class BossMagicZonePattern : MonoBehaviour
{
    [SerializeField] private BossAnimatorController controller;
    [SerializeField] private string circleIndicatorPoolKey = "AttackCircleIndicator";
    [SerializeField] private float preDelay = 0.8f;

    [SerializeField] private int zoneCount = 3;
    [SerializeField] private float spawnRadiusTiles = 8f;

    [SerializeField] private float zoneRadius = 2.0f;
    [SerializeField] private float duration = 5.0f;
    [SerializeField] private float tickInterval = 0.8f;
    [SerializeField] private float tickDamage = 6f;
    [SerializeField] private LayerMask targetMask;

    public IEnumerator Execute(Transform target)
    {
        if (target == null) yield break;

        controller.MagicZone();

        for (int i = 0; i < zoneCount; i++)
        {
            Vector2 pos = (Vector2)target.position + Random.insideUnitCircle * spawnRadiusTiles;

            // 1) 전조 인디케이터
            GameObject igo = ObjectPooler.Instance.SpawnFromPool(circleIndicatorPoolKey, pos, Quaternion.identity);
            igo.TryGetComponent<AttackCircleIndicator>(out AttackCircleIndicator indicator);
            if (indicator == null) yield break;

            float diameter = zoneRadius * 2f;
            indicator.StartIndicator(new Vector2(diameter, diameter), preDelay);

            // 2) 인디케이터 완료 시 실제 장판 생성/활성
            indicator.OnIndicatorComplete += () =>
            {
                ObjectPooler.Instance.SpawnFromPool("MagicZoneArea", pos, Quaternion.identity).TryGetComponent<MagicZoneArea>(out MagicZoneArea zone);
                if(zone != null)
                    zone.Activate(zoneRadius, duration, tickInterval, tickDamage, targetMask, gameObject);
                SfxManager.Instance.Play("Boss_Magic");
            };
        }
    }
}