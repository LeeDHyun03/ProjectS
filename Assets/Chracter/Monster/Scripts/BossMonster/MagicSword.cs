using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class MagicSword : MonoBehaviour
{
    private Collider2D hitbox;

    [SerializeField] private LayerMask targetMask;
    private float orbitHitDamage;
    private bool orbitHitEnabled;

    private float currentAngleDeg;

    private float projectileDamage;
    private float projectileSpeed;
    private float projectileLifeTime;
    private float projectileAlive;
    private bool launched;

    private Vector2 projectileDir;
    private GameObject instigator;

    private BossSwordThrowPattern owner;

    private void Awake()
    {
        hitbox = GetComponent<Collider2D>();
        hitbox.isTrigger = true;
        hitbox.enabled = false;
    }

    public void SetOwner(BossSwordThrowPattern o)
    {
        owner = o;
        launched = false;
    }

    public void SetInitialAngle(float angleDeg)
    {
        currentAngleDeg = angleDeg;
        transform.rotation = Quaternion.Euler(0f, 0f, currentAngleDeg - 90f);
    }

    /// <summary>보스 중심 주위를 반시계로 공전</summary>
    public void OrbitAround(Vector2 center, float radius, float deltaDegreesCCW)
    {
        if (launched) return;

        currentAngleDeg += deltaDegreesCCW; 
        currentAngleDeg = (currentAngleDeg + 360f) % 360f;

        float r = currentAngleDeg * Mathf.Deg2Rad;
        Vector2 pos = center + new Vector2(Mathf.Cos(r), Mathf.Sin(r)) * radius;
        transform.position = pos;

        transform.rotation = Quaternion.Euler(0f, 0f, currentAngleDeg - 90f);
    }

    /// <summary>조준: 플레이어를 바라봄(발사 전까지 계속 호출)</summary>
    public void AimAt(Vector2 targetPos)
    {
        if (launched) return;

        Vector2 dir = (targetPos - (Vector2)transform.position);
        if (dir.sqrMagnitude < 0.0001f) return;

        float a = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, a - 180f);
    }

    public void EnableOrbitHit(float damage, LayerMask mask)
    {
        orbitHitEnabled = true;
        orbitHitDamage = damage;
        targetMask = mask;
        hitbox.enabled = true;
    }

    public void DisableHit()
    {
        orbitHitEnabled = false;
        hitbox.enabled = false;
    }

    public void LaunchTowards(Vector2 targetPos, float speed, float lifeTime, float damage, LayerMask mask, GameObject instigator)
    {
        launched = true;

        projectileSpeed = speed;
        projectileLifeTime = lifeTime;
        projectileDamage = damage;
        targetMask = mask;
        this.instigator = instigator;

        projectileAlive = 0f;

        Vector2 dir = (targetPos - (Vector2)transform.position);
        if (dir.sqrMagnitude < 0.0001f) dir = Vector2.right;
        projectileDir = dir.normalized;

        hitbox.enabled = true;
    }

    private void Update()
    {
        if (!launched) return;

        projectileAlive += Time.deltaTime;
        transform.position += (Vector3)(projectileDir * projectileSpeed * Time.deltaTime);

        if (projectileAlive >= projectileLifeTime)
        {
            launched = false;
            ObjectPooler.ReturnToPool(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & targetMask.value) == 0)
            return;

        float dmg = launched ? projectileDamage : (orbitHitEnabled ? orbitHitDamage : 0f);
        if (dmg <= 0f) return;

        other.TryGetComponent<PlayerCharacter>(out PlayerCharacter playerCharacter);
        if (playerCharacter != null)
        {
            playerCharacter.TakeDamage(dmg);
        }
    }
}
