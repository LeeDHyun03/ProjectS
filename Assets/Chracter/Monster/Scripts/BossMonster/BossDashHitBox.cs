using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BossDashHitbox : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private BossMovement movement;
    [SerializeField] private Collider2D hitboxCollider;

    [Header("Damage")]
    [SerializeField] private float dashDamage = 10f;

    [Header("Target Filter")]
    [SerializeField] private LayerMask targetMask; // Player 레이어만 권장

    private readonly HashSet<int> hitSet = new();
    private bool active;

    private void Awake()
    {
        if (!hitboxCollider) hitboxCollider = GetComponent<Collider2D>();
        hitboxCollider.isTrigger = true;
        hitboxCollider.enabled = false;
    }

    private void OnEnable()
    {
        if (movement != null)
        {
            movement.OnDashStart += EnableHitbox;
            movement.OnDashEnd += DisableHitbox;
        }
    }

    private void OnDisable()
    {
        if (movement != null)
        {
            movement.OnDashStart -= EnableHitbox;
            movement.OnDashEnd -= DisableHitbox;
        }
        DisableHitbox();
    }

    public void SetDamage(float damage) => dashDamage = damage;

    private void EnableHitbox()
    {
        active = true;
        hitSet.Clear();
        hitboxCollider.enabled = true;
    }

    private void DisableHitbox()
    {
        active = false;
        hitSet.Clear();
        hitboxCollider.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other) => TryHit(other);
    private void OnTriggerStay2D(Collider2D other) => TryHit(other);

    private void TryHit(Collider2D other)
    {
        if (!active) return;

        if (((1 << other.gameObject.layer) & targetMask.value) == 0)
            return;

        int id = other.transform.root.GetInstanceID();
        if (hitSet.Contains(id)) return;
        hitSet.Add(id);

        // 폴백: Character
        other.TryGetComponent<PlayerCharacter>(out PlayerCharacter character);
        if (character != null)
        {
            character.TakeDamage(dashDamage);
        }
    }
}
