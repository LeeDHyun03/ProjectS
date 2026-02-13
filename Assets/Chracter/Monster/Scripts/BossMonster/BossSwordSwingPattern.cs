using System.Collections;
using UnityEngine;

public class BossSwordSwingPattern : MonoBehaviour
{
    [Header("Ref")]
    [SerializeField] private BossAnimatorController controller;


    [Header("Timing")]
    [SerializeField] private float preDelay = 0.7f;
    [SerializeField] private float postDelay = 1.0f;

    [Header("Damage")]
    [SerializeField] private float damage = 15f;
    [SerializeField] private LayerMask targetMask; // Player 레이어

    [Header("Area")]
    [SerializeField] private float outerRadius = 2.8f;

    [Header("Safe Sector (1/4)")]
    [SerializeField] private float safeSectorAngle = 90f; // 1/4 = 90도

    [Header("Indicator")]
    [SerializeField] private MonoBehaviour circleIndicatorBehaviour; 
    [SerializeField] private Transform safeMarker;                   
    [SerializeField] private float safeLocalAngle = 45f;

    private IAttackIndicator circleIndicator;

    private readonly Collider2D[] buffer = new Collider2D[16];
    private ContactFilter2D targetFilter;

    private void Awake()
    {
        circleIndicator = circleIndicatorBehaviour as IAttackIndicator;

        targetFilter = new ContactFilter2D { useLayerMask = true, useTriggers = true };
        targetFilter.SetLayerMask(targetMask);
    }

    public IEnumerator Execute(Transform target)
    {
        if (target == null) yield break;

        Vector2 bossPos = transform.position;
        Vector2 committedSafeDir = ComputeSafeDir(bossPos, target.position);
        
        UpdateSafeMarker(bossPos, committedSafeDir);

        if (circleIndicator != null)
        {
            if (circleIndicator is MonoBehaviour mono)
            {
                mono.transform.position = bossPos;

                float angle = Mathf.Atan2(committedSafeDir.y, committedSafeDir.x) * Mathf.Rad2Deg;
                mono.transform.rotation = Quaternion.Euler(0f, 0f, angle - safeLocalAngle);

                mono.gameObject.SetActive(true); 
            }
            Vector2 indicatorSize = circleIndicator.GetBaseSize();

            this.outerRadius = indicatorSize.x * 0.5f;

            float diameter = outerRadius * 2f;
            circleIndicator.StartIndicator(new Vector2(diameter, diameter), preDelay);
        }

        controller.SwingReady();

        float t = 0f;

        while (t < preDelay)
        {
            t += Time.deltaTime;
            yield return null;
        }

        ApplySwingHit(committedSafeDir);

        if (postDelay > 0f)
            yield return new WaitForSeconds(postDelay);

        if (safeMarker != null) safeMarker.gameObject.SetActive(false);
    }

    private void ApplySwingHit(Vector2 safeDir)
    {

        controller.Swing();
        Vector2 origin = transform.position;

        int count = Physics2D.OverlapCircle(origin, outerRadius, targetFilter, buffer);

        float safeHalf = safeSectorAngle * 0.5f;

        for (int i = 0; i < count; i++)
        {
            Collider2D col = buffer[i];
            if (col == null) continue;

            Vector2 pos = col.bounds.center;
            Vector2 to = pos - origin;

            float dist = to.magnitude;
            if (dist > outerRadius) continue;

            Vector2 toN = (dist > 0.0001f) ? (to / dist) : Vector2.zero;

            if (Vector2.Angle(safeDir, toN) <= safeHalf)
                continue;

            col.TryGetComponent<PlayerCharacter>(out PlayerCharacter playerCharacter);
            if (playerCharacter != null)
                playerCharacter.TakeDamage(damage);
        }
    }

    private static Vector2 ComputeSafeDir(Vector2 bossPos, Vector2 playerPos)
    {
        Vector2 toPlayer = playerPos - bossPos;
        if (toPlayer.sqrMagnitude < 0.0001f)
            return Vector2.up;

        toPlayer.Normalize();

        Vector2 safe = (-toPlayer + Vector2.up);
        if (safe.sqrMagnitude < 0.0001f)
            safe = Vector2.up;

        return safe.normalized;
    }

    private void UpdateSafeMarker(Vector2 bossPos, Vector2 safeDir)
    {
        if (safeMarker == null) return;

        safeMarker.gameObject.SetActive(true);
        safeMarker.position = bossPos;

        float angle = Mathf.Atan2(safeDir.y, safeDir.x) * Mathf.Rad2Deg;
        safeMarker.rotation = Quaternion.Euler(0f, 0f, angle);

        float diameter = outerRadius * 2f;
        safeMarker.localScale = new Vector3(diameter, diameter, 1f);
    }
}