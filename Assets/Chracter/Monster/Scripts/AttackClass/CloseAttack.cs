using UnityEngine;

public class CloseAttack : MonsterAttack
{
    [SerializeField] private BoxCollider2D attackCollider;
    [SerializeField] private float attackIndicatorOffsetX = 0f;
    [SerializeField] private LayerMask targetLayer;

    public override void Attack()
    {
        ExecuteOverlapAttack();
    }

    public void SetAttackDirection()
    {
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        angle -= 180f;
        attackCollider.transform.rotation = Quaternion.Euler(0, 0, angle);

        var indicator = ActivateAttackIndicator(IndicatorShape.Box, attackCollider.transform.position, dir);
        if (indicator is MonoBehaviour indicatorMb)
        {
            indicatorMb.transform.SetParent(attackCollider.transform, worldPositionStays: false);
            indicatorMb.transform.localPosition = new Vector3(attackIndicatorOffsetX, 0, 0);
            indicatorMb.transform.localRotation = Quaternion.Euler(0, 0, -90);
        }

        if (indicator == null) return;

        indicator.OnIndicatorComplete += BroadcastOnStartedAttack;
        indicator.StartIndicator(attackCollider.size, attackSpeed);
    }

    private void ExecuteOverlapAttack()
    {
        Vector2 pos = attackCollider.transform.position;
        Vector2 size = attackCollider.size;
        float angle = attackCollider.transform.eulerAngles.z;

        var hits = Physics2D.OverlapBoxAll(pos, size, angle, targetLayer);

        foreach (var col in hits)
        {
            if (!col.TryGetComponent<Character>(out var character)) continue;
            if (isPlayerSide && character is PlayerCharacter) continue;
            if (!isPlayerSide && character is Monster) continue;

            character.TakeDamage(attackDamage);
        }

        //DisableAttackCollider();
    }

    public void EnableAttackCollider() => attackCollider.enabled = true;
    public void DisableAttackCollider() => attackCollider.enabled = false;
}
