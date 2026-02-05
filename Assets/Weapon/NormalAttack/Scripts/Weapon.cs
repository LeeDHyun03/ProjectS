using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Weapon : MonoBehaviour
{
    [SerializeField] private BoxCollider2D attackCollider;
    [SerializeField] private LayerMask targetLayer;

    private float attackDamage = 5;

    public void SetAttackDamage(float newAttackDamage)
    {
        attackDamage = newAttackDamage;
    }

    public void StartAttack(bool isSecondAttack, bool isFilp)
    {
        ExecuteOverlapAttack(isSecondAttack, isFilp);
    }

    private void ExecuteOverlapAttack(bool isSecondAttack, bool isFilp)
    {
        GameObject effect = PlayerEffectManager.Instance.OnNormalAttackEffect();
        if (effect != null)
        {
            if (isFilp)
            {
                if (isSecondAttack)
                {
                    effect.transform.localRotation *= Quaternion.Euler(0f, 180f, 0f);
                }
            }
            else
            {
                if (!isSecondAttack)
                {
                    effect.transform.localRotation *= Quaternion.Euler(0f, 180f, 0f);
                }
            }
        }

        Vector2 pos = attackCollider.transform.position;
        Vector2 size = attackCollider.size;
        float angle = attackCollider.transform.eulerAngles.z;

        Collider2D[] hitCharacters = Physics2D.OverlapBoxAll(pos, size, angle, targetLayer);

        foreach (var collision in hitCharacters)
        {
            if (collision.TryGetComponent<Monster>(out Monster monster))
            {
                monster.TakeDamage(attackDamage);
            }
        }

        DisableAttackCollider();
    }

    public void EnableAttackCollider() => attackCollider.enabled = true;
    public void DisableAttackCollider() => attackCollider.enabled = false;

    private void OnDrawGizmos()
    {
        if (attackCollider == null) return;
        Gizmos.color = Color.red;
        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.matrix = attackCollider.transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, attackCollider.size);
        Gizmos.matrix = oldMatrix;
    }
}

