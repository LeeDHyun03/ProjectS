using System;
using System.Collections;
using UnityEngine;

public class BossDashPatternRunner : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private BossMovement movement;
    [SerializeField] private BossDashIndicator dashIndicator;
    [SerializeField] private BossDashHitbox dashHitbox;
    [SerializeField] private Transform target;
    [SerializeField] private BossAnimatorController controller;

    [Header("Follow-up (after Dash3)")]
    [SerializeField] private BossSwordSwingPattern swordSwing;
    [SerializeField] private BossSwordWavePattern swordWave;

    [Header("Wall Clamp (visual too)")]
    [SerializeField] private LayerMask wallMask; // Wall만

    [Header("Dash Spec (3-hit)")]
    [SerializeField] private float pre1 = 0.8f;
    [SerializeField] private float post1 = 1.3f;
    [SerializeField] private float pre2 = 0.8f;
    [SerializeField] private float post2 = 1.3f;
    [SerializeField] private float pre3 = 0.6f;
    [SerializeField] private float post3 = 1.0f;

    [Header("Damage (per hit)")]
    [SerializeField] private float dmg1 = 10f;
    [SerializeField] private float dmg2 = 10f;
    [SerializeField] private float dmg3 = 12f;

    [Header("Indicator")]
    [SerializeField] private float fixedDashLength = 6f;
    [SerializeField] private float indicatorWidth = 1.2f;

    [Header("ToTarget Clamp (Indicator)")]
    [SerializeField] private float minLen = 0.5f;
    [SerializeField] private float maxLen = 12f;

    public void SetTarget(Transform newTarget) => target = newTarget;

    private void Awake()
    {
        if (movement != null && dashIndicator != null)
        {
            movement.OnAimUpdated += dashIndicator.UpdateAim;
            movement.OnDashCommitted += dashIndicator.Commit;
        }

        if (dashIndicator != null)
            dashIndicator.SetWidth(indicatorWidth);
    }

    private void OnDestroy()
    {
        if (movement != null && dashIndicator != null)
        {
            movement.OnAimUpdated -= dashIndicator.UpdateAim;
            movement.OnDashCommitted -= dashIndicator.Commit;
        }
    }

    public IEnumerator RunDashCombo()
    {
        yield return RunDash3();

        // 50/50 연계(돌진 연계 공격)
        if (UnityEngine.Random.value < 0.5f)
        {
            if (swordSwing != null) yield return swordSwing.Execute(target);
        }
        else
        {
            if (swordWave != null) yield return swordWave.Execute(target);
        }
    }

    public IEnumerator RunDash3()
    {
        // 1~2타: 고정 거리
        yield return DashOnce(pre1, post1, BossMovement.DashDistanceMode.Fixed, dynamicLen: false, damage: dmg1);
        yield return DashOnce(pre2, post2, BossMovement.DashDistanceMode.Fixed, dynamicLen: false, damage: dmg2);

        // 3타: 커밋 시점 플레이어까지(가변 길이)
        yield return DashOnce(pre3, post3, BossMovement.DashDistanceMode.ToTarget, dynamicLen: true, damage: dmg3);
    }

    private IEnumerator DashOnce(float preDelay, float postDelay, BossMovement.DashDistanceMode mode, bool dynamicLen, float damage)
    {
        if (movement == null || dashIndicator == null || target == null)
            yield break;

        if (dashHitbox != null)
            dashHitbox.SetDamage(damage);

        controller.DashReady();

        dashIndicator.SetVisible(true);

        if (!dynamicLen)
        {
            float visualLen = ClampByWall(fixedDashLength);
            dashIndicator.SetLength(visualLen);
            dashIndicator.SetFill01(0f);
        }

        Coroutine telegraphRoutine = StartCoroutine(CoTelegraphVisual(preDelay, dynamicLen));
        yield return movement.DashToTargetLocked(target, preDelay, postDelay, mode);

        if (telegraphRoutine != null)
            StopCoroutine(telegraphRoutine);

    }

    private IEnumerator CoTelegraphVisual(float duration, bool dynamicLen)
    {
        float t = 0f;
        while (t < duration)
        {
            if (target == null) yield break;

            float fill01 = (duration <= 0.0001f) ? 1f : (t / duration);
            dashIndicator.SetFill01(fill01);

            if (dynamicLen)
            {
                float dist = Vector2.Distance(transform.position, target.position);
                dist = Mathf.Clamp(dist, minLen, maxLen);
                dist = ClampByWall(dist);
                dashIndicator.SetLength(dist);
            }

            t += Time.deltaTime;
            yield return null;
        }

        dashIndicator.SetFill01(1f);
        dashIndicator.SetVisible(false);
    }

    private float ClampByWall(float desiredDist)
    {
        Vector2 start = transform.position;

        Vector2 dir = dashIndicator.transform.forward;

        RaycastHit2D hit = Physics2D.Raycast(start, dir, desiredDist, wallMask);
        if (hit.collider != null)
        {
            float hitDist = Vector2.Distance(start, hit.point) - 0.05f;
            return Mathf.Max(0.1f, hitDist);
        }

        return desiredDist;
    }
}
