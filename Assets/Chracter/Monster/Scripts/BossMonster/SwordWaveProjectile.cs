using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SwordWaveProjectile : MonoBehaviour
{
    [SerializeField] private Collider2D col;
    [SerializeField] private LayerMask targetMask;

    private float damage;
    private GameObject instigator;

    private void Awake()
    {
        if (!col) col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    public void Launch(
        Vector2 dir,
        float speed,
        float dmg,
        float lifeTime,
        float growthInterval,
        float growthStep,
        GameObject instigatorObj)
    {
        damage = dmg;
        instigator = instigatorObj;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        StartCoroutine(CoMove(dir.normalized, speed, lifeTime));
        StartCoroutine(CoGrow(growthInterval, growthStep, lifeTime));

        Destroy(gameObject, lifeTime + 0.1f); // 풀링이면 반환으로 교체
    }

    private IEnumerator CoMove(Vector2 dir, float speed, float lifeTime)
    {
        float t = 0f;
        while (t < lifeTime)
        {
            transform.position += (Vector3)(dir * speed * Time.deltaTime);
            t += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator CoGrow(float interval, float step, float lifeTime)
    {
        if (interval <= 0f) yield break;

        float t = 0f;
        while (t < lifeTime)
        {
            Vector3 s = transform.localScale;
            s.x += step;
            transform.localScale = s;

            yield return new WaitForSeconds(interval);
            t += interval;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & targetMask.value) == 0)
            return;

        var dmgable = other.GetComponentInParent<IDamageable>();
        if (dmgable != null)
        {
            dmgable.ApplyDamage(damage, instigator);
            return;
        }

        var ch = other.GetComponentInParent<Character>();
        if (ch != null)
            ch.TakeDamage(damage);
    }
}
