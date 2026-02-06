using UnityEngine;
using System.Collections.Generic; // 리스트 사용을 위해 추가

public class CloseAttack : MonsterAttack
{
    [SerializeField] private BoxCollider2D attackCollider;
    [SerializeField] private float attackIndicatorOffsetX = 0;
    [SerializeField] private LayerMask targetLayer;

    public override void Attack()
    {
        ExecuteOverlapAttack();
    }

    private void ExecuteOverlapAttack()
    {
        Vector2 pos = attackCollider.transform.position;
        Vector2 size = attackCollider.size;
        float angle = attackCollider.transform.eulerAngles.z;

        Collider2D[] hitCharacters = Physics2D.OverlapBoxAll(pos, size, angle, targetLayer);

        foreach (var collision in hitCharacters)
        {
            if (collision.TryGetComponent<Character>(out Character character))
            {
                if (isPlayerSide && character is PlayerCharacter) continue;
                if (!isPlayerSide && character is Monster) continue;

                character.TakeDamage(attackDamage);
            }
        }

        DisableAttackCollider();
    }

    public void SetAttackDirection()
    {
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        angle -= 180f;
        attackCollider.transform.rotation = Quaternion.Euler(0, 0, angle);

        AttackIndicator attackIndicator = ActivateAttackIndicator();
        if (attackIndicator == null) return;

        attackIndicator.OnIndicatorComplete += BroadcastOnStartedAttack;
        Vector3 attackIndicatorPos = new Vector3(attackIndicatorOffsetX, 0, 0);
        attackIndicator.transform.localPosition = attackIndicatorPos;
        attackIndicator.transform.localRotation = Quaternion.Euler(0, 0, -90);
        attackIndicator.StartIndicator(attackCollider.size, attackSpeed);
    }

    public void EnableAttackCollider() => attackCollider.enabled = true;
    public void DisableAttackCollider() => attackCollider.enabled = false;

    protected override AttackIndicator ActivateAttackIndicator()
    {
        var spawned = ObjectPooler.Instance.SpawnFromPool("AttackIndicator", Vector3.zero, Quaternion.identity);
        if (spawned.TryGetComponent<AttackIndicator>(out AttackIndicator attackIndicator))
        {
            attackIndicator.transform.parent = attackCollider.transform;
            attackIndicator.transform.localPosition = Vector3.zero;
            attackIndicator.transform.localRotation = Quaternion.identity;
            return attackIndicator;
        }
        return null;
    }
    private void OnDrawGizmosSelected()
    {
        if (attackCollider == null) return;
        Gizmos.color = Color.red;
        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.matrix = attackCollider.transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, attackCollider.size);
        Gizmos.matrix = oldMatrix;
    }
}