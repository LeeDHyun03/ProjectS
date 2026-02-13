using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class MagicZoneArea : MonoBehaviour
{
    [Header("Visual (optional)")]
    [SerializeField] private SpriteRenderer outlineSr; // optional
    [SerializeField] private SpriteRenderer fillSr;    // optional

    private CircleCollider2D trigger;

    private float radius;
    private float activeTime;
    private float tickInterval;
    private float tickDamage;
    private LayerMask targetMask;
    private GameObject instigator;

    // 텔레그래프는 AttackCircleIndicator로 빼는 게 구조상 더 깔끔하므로,
    // 이 클래스는 "활성화(판정 ON) 이후" 역할만 담당하도록 수정.
    private Coroutine lifeRoutine;

    // "지속적으로 접촉 시 tickInterval마다" → 대상별 누적 시간
    private readonly Dictionary<int, float> stayTimeByTarget = new();

    private void Awake()
    {
        trigger = GetComponent<CircleCollider2D>();
        trigger.isTrigger = true;
        trigger.enabled = false;
    }

    private void OnDisable()
    {
        if (lifeRoutine != null)
        {
            StopCoroutine(lifeRoutine);
            lifeRoutine = null;
        }
        stayTimeByTarget.Clear();
        trigger.enabled = false;
    }

    /// <summary>
    /// 인디케이터(AttackCircleIndicator) 완료 후 호출:
    /// - 표시 제거 상태로 시작(문서: 판정 시작 시 표시 제거)
    /// - 트리거 ON
    /// - activeTime 동안 틱 데미지
    /// - 종료 시 Destroy 또는 Pool 반환
    /// </summary>
    public void Activate(
        float radius,
        float activeTime,
        float tickInterval,
        float tickDamage,
        LayerMask targetMask,
        GameObject instigator)
    {
        this.radius = Mathf.Max(0.05f, radius);
        this.activeTime = Mathf.Max(0f, activeTime);
        this.tickInterval = Mathf.Max(0.01f, tickInterval);
        this.tickDamage = tickDamage;
        this.targetMask = targetMask;
        this.instigator = instigator;

        // 콜라이더 반지름 세팅
        trigger.radius = this.radius;

        stayTimeByTarget.Clear();
        trigger.enabled = true;

        if (lifeRoutine != null) StopCoroutine(lifeRoutine);
        lifeRoutine = StartCoroutine(CoActiveLifetime());
    }

    private IEnumerator CoActiveLifetime()
    {
        float alive = 0f;
        while (alive < activeTime)
        {
            alive += Time.deltaTime;
            yield return null;
        }

        lifeRoutine = null;

        trigger.enabled = false;
        stayTimeByTarget.Clear();

        ObjectPooler.ReturnToPool(gameObject);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // 레이어 필터
        if (((1 << other.gameObject.layer) & targetMask.value) == 0)
            return;

        int key = other.transform.root.GetInstanceID();

        if (!stayTimeByTarget.TryGetValue(key, out float acc))
            acc = 0f;

        acc += Time.deltaTime;

        while (acc >= tickInterval)
        {
            acc -= tickInterval;
            ApplyDamage(other, tickDamage);
        }

        stayTimeByTarget[key] = acc;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        int key = other.transform.root.GetInstanceID();
        stayTimeByTarget.Remove(key);
    }

    private void ApplyDamage(Collider2D other, float dmg)
    {
        var dmgable = other.GetComponentInParent<IDamageable>();
        if (dmgable != null)
        {
            dmgable.ApplyDamage(dmg, instigator != null ? instigator : gameObject);
            return;
        }

        var ch = other.GetComponentInParent<Character>();
        if (ch != null)
            ch.TakeDamage(dmg);
    }
}