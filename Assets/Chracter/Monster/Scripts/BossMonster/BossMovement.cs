using System;
using System.Collections;
using UnityEngine;

public class BossMovement : MonoBehaviour
{
    public enum DashDistanceMode { Fixed, ToTarget }

    [Header("Refs")]
    [SerializeField] private BossAnimatorController controller;

    [Header("Dash Tuning")]
    [SerializeField] private float dashSpeed = 12f;
    [SerializeField] private float dashDistance = 6f;
    [SerializeField] private float minDashDistance = 0.5f;
    [SerializeField] private float maxDashDistance = 12f;

    [Header("Collision")]
    [SerializeField] private LayerMask wallMask;

    public bool IsDashing { get; private set; }

    public event Action<Vector2> OnAimUpdated;
    public event Action<Vector2> OnDashCommitted;
    public event Action OnDashStart;
    public event Action OnDashEnd;

    private Coroutine dashRoutine;

    public Coroutine DashToTargetLocked(Transform target, float preDelay, float postDelay, DashDistanceMode mode)
    {
        if (target == null) return null;

        if (dashRoutine != null) StopCoroutine(dashRoutine);
        dashRoutine = StartCoroutine(CoDash(target, preDelay, postDelay, mode));
        return dashRoutine;
    }

    private IEnumerator CoDash(Transform target, float preDelay, float postDelay, DashDistanceMode mode)
    {
        IsDashing = true;

        Vector2 dir = (target != null)
    ? ((Vector2)(target.position - transform.position)).normalized
    : Vector2.right;

        // 1) 전딜: 방향 추적
        float t = 0f;

        while (t < preDelay)
        {
            if (target != null)
            {
                Vector2 toTarget = (Vector2)(target.position - transform.position);
                if (toTarget.sqrMagnitude > 0.0001f)
                    dir = toTarget.normalized;

                OnAimUpdated?.Invoke(dir);
            }

            t += Time.deltaTime;
            yield return null;
        }

        // 전딜이 0이거나 루프가 안 돌았을 경우 대비
        if (target != null)
        {
            Vector2 toTarget = (Vector2)(target.position - transform.position);
            if (toTarget.sqrMagnitude > 0.0001f)
                dir = toTarget.normalized;
        }

        // 2) 커밋
        OnDashCommitted?.Invoke(dir);

        Vector2 start = transform.position;

        float desiredDist = dashDistance;
        if (mode == DashDistanceMode.ToTarget && target != null)
            desiredDist = Vector2.Distance(start, target.position);

        desiredDist = Mathf.Clamp(desiredDist, minDashDistance, maxDashDistance);

        Vector2 goal = start + dir * desiredDist;

        // 3) Wall 충돌 클램프
        RaycastHit2D hit = Physics2D.Raycast(start, dir, desiredDist, wallMask);
        if (hit.collider != null)
        {
            goal = hit.point - dir * 0.05f;
        }

        // 4) 이동 시작
        OnDashStart?.Invoke();
        controller?.Dash();

        float totalDist = Vector2.Distance(start, goal);
        float moveTime = (dashSpeed <= 0.0001f) ? 0f : totalDist / dashSpeed;

        float elapsed = 0f;

        while (elapsed < moveTime)
        {
            float alpha = (moveTime <= 0.0001f) ? 1f : (elapsed / moveTime);

            transform.position = Vector2.Lerp(start, goal, alpha);

            elapsed += Time.deltaTime * 2;
            yield return null;
        }

        transform.position = goal;

        OnDashEnd?.Invoke();

        // 5) 후딜
        if (postDelay > 0f)
            yield return new WaitForSeconds(postDelay);

        if (mode == DashDistanceMode.Fixed)
            controller.DashReady();
        else
            controller.DashEnded();


        IsDashing = false;
        dashRoutine = null;
    }
}