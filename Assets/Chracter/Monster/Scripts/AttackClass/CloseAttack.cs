using UnityEngine;

public class CloseAttack : MonsterAttack
{
    [SerializeField] private BoxCollider2D attackCollider;
    [SerializeField] private float attackIndicatorOffsetX = 0;
    public override void Attack()
    {
        EnableAttackCollider();
    }

    public void SetAttackDirection()
    {
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        angle -= 180f;
        attackCollider.transform.rotation = Quaternion.Euler(0, 0, angle);
        AttackIndicator attackIndicator = ActivateAttackIndicator();
        if(attackIndicator == null) return;
        attackIndicator.OnIndicatorComplete += BroadcastOnStartedAttack;
        Vector3 attackIndicatorPos = new Vector3(attackIndicatorOffsetX, 0, 0);
        attackIndicator.transform.localPosition = attackIndicatorPos;
        attackIndicator.transform.localRotation = Quaternion.Euler(0, 0, -90);
        attackIndicator.StartIndicator(attackCollider.size, attackSpeed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.TryGetComponent<Character>(out Character character);
        if (character == null) return;
        if (isPlayerSide && character is PlayerCharacter) return;
        if (!isPlayerSide && character is Monster) return;

        character.TakeDamage(attackDamage);
    }

    public void EnableAttackCollider()
    {
        attackCollider.enabled = true;
    }

    public void DisableAttackCollider()
    {
        attackCollider.enabled = false;
    }

    protected override AttackIndicator ActivateAttackIndicator()
    {
        ObjectPooler.Instance.SpawnFromPool("AttackIndicator", Vector3.zero, Quaternion.Euler(Vector3.zero))
            .TryGetComponent<AttackIndicator>(out AttackIndicator attackIndicator);
        if (attackIndicator == null) return null;
        attackIndicator.transform.parent = attackCollider.transform;
        attackIndicator.transform.localPosition = Vector3.zero;
        attackIndicator.transform.localRotation = Quaternion.Euler(Vector3.zero);
        return attackIndicator;
    }
}
