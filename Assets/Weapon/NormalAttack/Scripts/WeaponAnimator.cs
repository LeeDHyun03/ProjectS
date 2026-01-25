using UnityEngine;

public class WeaponAnimator : MonoBehaviour
{
    [SerializeField] private PlayerCombat combat;

    private Animator animator;
    private int currentAttackCount = 0;

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
    }
    private void AttackEnded(int attackCount)
    {
        animator.SetBool("isAttacking", false);
        if (attackCount >= currentAttackCount)
        {
            currentAttackCount = 0;
            animator.SetInteger("attackCount", currentAttackCount);
        }
        combat.OnNormalAttackAnimationComplete();
    }
}
