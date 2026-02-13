using System.Collections;
using UnityEngine;

public class BossSwordWavePattern : MonoBehaviour
{
    [Header("Ref")]
    [SerializeField] private BossAnimatorController controller;

    [Header("Timing")]
    [SerializeField] private float preDelay = 0.7f;
    [SerializeField] private float postDelay = 0.5f;

    [Header("Projectile")]
    [SerializeField] private float lifeTime = 6f;
    [SerializeField] private float speed = 8f;

    [Header("Growth")]
    [SerializeField] private float growthInterval = 0.2f;
    [SerializeField] private float growthStep = 0.4f;

    [Header("Damage")]
    [SerializeField] private float damage = 12f;

    [Header("Indicator (optional)")]
    [SerializeField] private BossArcIndicator indicator;

    public IEnumerator Execute(Transform target)
    {
        if (target == null) yield break;

        controller.SwingReady();

        indicator?.SetVisible(true);
        indicator?.SetFill01(0f);

        float t = 0f;
        Vector2 aimDir = Vector2.right;

        while (t < preDelay)
        {
            Vector2 toTarget = (Vector2)(target.position - transform.position);
            if (toTarget.sqrMagnitude > 0.0001f)
                aimDir = toTarget.normalized;

            indicator?.UpdateAim(aimDir);
            indicator?.SetFill01(preDelay <= 0.0001f ? 1f : (t / preDelay));

            t += Time.deltaTime;
            yield return null;
        }

        controller.Swing();

        Vector2 snapDir = SnapToCardinal(aimDir);
        indicator?.Commit(snapDir);
        indicator?.SetFill01(1f);

        ObjectPooler.Instance.SpawnFromPool("SwordWaveProjectile", transform.position, Quaternion.identity).TryGetComponent<SwordWaveProjectile>(out SwordWaveProjectile proj);
        SfxManager.Instance.Play("Release");
        if (proj == null) yield break;
        proj.Launch(snapDir, speed, damage, lifeTime, growthInterval, growthStep, gameObject);

        if (postDelay > 0f) yield return new WaitForSeconds(postDelay);

        indicator?.SetVisible(false);
    }

    private Vector2 SnapToCardinal(Vector2 dir)
    {
        if (Mathf.Abs(dir.x) >= Mathf.Abs(dir.y))
            return dir.x >= 0f ? Vector2.right : Vector2.left;
        return dir.y >= 0f ? Vector2.up : Vector2.down;
    }
}
