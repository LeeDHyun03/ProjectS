using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponAnimator : MonoBehaviour
{
    [SerializeField] private PlayerCombat combat;
    
    private Animator animator;
    private int currentAttackCount = 0;

    public event Action<Vector3> NormalAttackStarted;

    private void Awake()
    {
        animator ??= GetComponent<Animator>();
    }

    private void OnEnable()
    {
        combat.NormalAttackTriggered += OnNormalAttack;
        combat.SpecialAttackTriggered += OnSpecialAttack;
    }
    private void OnDisable()
    {
        combat.NormalAttackTriggered -= OnNormalAttack;
        combat.SpecialAttackTriggered -= OnSpecialAttack;
    }

    private void OnNormalAttack()
    {
        currentAttackCount++;
        animator.SetInteger("attackCount", currentAttackCount);
    }
    private void OnSpecialAttack()
    {
        //animator.SetTrigger("specialAttack");
    }
    private void AttackStarted()
    {
        animator.SetBool("isAttacking", true);
        Vector2 mousePos = Mouse.current.position.ReadValue();

        Vector3 weaponScreenPos = Camera.main.WorldToScreenPoint(transform.position);

        Vector2 dir = mousePos - (Vector2)weaponScreenPos;
        NormalAttackStarted?.Invoke(dir);
    }

    private void AttackEnded(int attackCount)
    {
        animator.speed = 1f;
        animator.SetBool("isAttacking", false);
        if (attackCount >= currentAttackCount)
        {
            currentAttackCount = 0;
            animator.SetInteger("attackCount", currentAttackCount);
        }
        combat.OnNormalAttackAnimationComplete();
    }

    private void ApplyDelaySpeed()
    {
        float attackSpeed = combat.AttackSpeed;
        float increaseAmount = attackSpeed - 100f;
        float collectedSpeed = 100 + (increaseAmount * 0.5f);
        animator.speed = collectedSpeed / 100f;
    }

    private void ApplySwingSpeed()
    {
        float attackSpeed = combat.AttackSpeed;
        animator.speed = attackSpeed / 100f;
    }
}
