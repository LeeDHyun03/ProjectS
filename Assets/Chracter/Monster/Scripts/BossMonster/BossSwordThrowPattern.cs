using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 2.6 검 날리기
/// - 보스 주위에 마법 검 6개를 한 번에 소환
/// - 2초 동안 환형으로 1바퀴 회전(반시계), 이때 공격 판정 O
/// - 회전 후 검들이 동시에 플레이어를 바라보며 조준(발사 전까지 추적), 이때 공격 판정 X
/// - 3초 후 0.5초마다 1개씩 발사 (12시 먼저, 반시계 순)
/// - 공격 후 딜레이는 6번째 발사 후 1번만(0.8s)
/// </summary>
public class BossSwordThrowPattern : MonoBehaviour
{
    [Header("Ref")]
    [SerializeField] private BossAnimatorController controller;

    [Header("Timing (from doc)")]
    [SerializeField] private float orbitTime = 2.0f;
    [SerializeField] private float aimHoldTime = 3.0f;
    [SerializeField] private float fireInterval = 0.5f;
    [SerializeField] private float postDelayAfterLastFire = 0.8f;

    [Header("Swords")]
    [SerializeField] private int swordCount = 6;

    [Tooltip("보스 중심으로부터 검의 공전 반지름(타일=유닛 기준)")]
    [SerializeField] private float orbitRadius = 2.2f;

    [Tooltip("2초에 1바퀴(360도)")]
    [SerializeField] private float orbitDegreesPerSecond = 180f; // 360/2=180

    [Header("Projectile")]
    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] private float projectileLifeTime = 6f;
    [SerializeField] private float projectileDamage = 12f;
    [SerializeField] private LayerMask targetMask; // Player 레이어

    // 내부 상태
    private readonly List<MagicSword> swords = new();

    public IEnumerator Execute(Transform target)
    {
        if (target == null)
            yield break;

        controller.SwordThrow();

        // 1) 6개 소환 + 초기 배치(12시부터 반시계 순서로 정렬되게 배치)
        SpawnSwords();

        float orbitElapsed = 0f;
        while (orbitElapsed < orbitTime)
        {
            float dt = Time.deltaTime;
            orbitElapsed += dt;

            // 360도를 orbitTime에 1바퀴 = orbitDegreesPerSecond 사용
            float deltaDeg = orbitDegreesPerSecond * dt;

            for (int i = 0; i < swords.Count; i++)
            {
                if (swords[i] == null) continue;
                swords[i].OrbitAround(transform.position, orbitRadius, deltaDeg); // CCW
            }

            yield return null;
        }

        for (int i = 0; i < swords.Count; i++)
        {
            if (swords[i] == null) continue;
            swords[i].DisableHit();
        }

        float aimElapsed = 0f;
        while (aimElapsed < aimHoldTime)
        {
            aimElapsed += Time.deltaTime;

            for (int i = 0; i < swords.Count; i++)
            {
                if (swords[i] == null) continue;
                swords[i].AimAt(target.position);
            }

            yield return null;
        }

        SortSwordsByCCWFrom12();

        for (int i = 0; i < swords.Count; i++)
        {
            MagicSword s = swords[i];
            if (s == null) continue;

            s.AimAt(target.position);
            s.LaunchTowards(target.position, projectileSpeed, projectileLifeTime, projectileDamage, targetMask, instigator: gameObject);

            yield return new WaitForSeconds(fireInterval);
        }

        if (postDelayAfterLastFire > 0f)
            yield return new WaitForSeconds(postDelayAfterLastFire);

        CleanupRemaining();
    }

    private void SpawnSwords()
    {
        CleanupRemaining();
        swords.Clear();

        // 12시(90도)에서 시작해서 반시계(CCW)로 6등분 배치
        for (int i = 0; i < swordCount; i++)
        {
            float angleDeg = 90f + (360f / swordCount) * i; // 90, 150, 210...
            Vector2 offset = AngleToVector(angleDeg) * orbitRadius;
            Vector2 pos = (Vector2)transform.position + offset;

            ObjectPooler.Instance.SpawnFromPool("MagicSword", pos, Quaternion.identity).TryGetComponent<MagicSword>(out MagicSword sword);
            if (sword == null) continue;

            sword.SetOwner(this);
            sword.SetInitialAngle(angleDeg);
            sword.DisableHit();

            swords.Add(sword);
        }
    }

    private void SortSwordsByCCWFrom12()
    {
        Vector2 center = transform.position;

        swords.Sort((a, b) =>
        {
            if (a == null && b == null) return 0;
            if (a == null) return 1;
            if (b == null) return -1;

            float aa = GetAngleFrom12CCW(center, (Vector2)a.transform.position);
            float bb = GetAngleFrom12CCW(center, (Vector2)b.transform.position);
            return aa.CompareTo(bb);
        });
    }

    private float GetAngleFrom12CCW(Vector2 center, Vector2 pos)
    {
        Vector2 v = (pos - center).normalized;
        
        float angleFromX = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
        float angle360 = (angleFromX + 360f) % 360f;            

        float from12 = (angle360 - 90f + 360f) % 360f;

        return from12;
    }

    private Vector2 AngleToVector(float deg)
    {
        float r = deg * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(r), Mathf.Sin(r));
    }

    private void CleanupRemaining()
    {
        for (int i = 0; i < swords.Count; i++)
        {
            if (swords[i] != null)
                ObjectPooler.ReturnToPool(swords[i].gameObject);
        }
        swords.Clear();
    }
}
