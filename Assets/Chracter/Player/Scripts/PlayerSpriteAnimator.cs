using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerSpriteAnimator : MonoBehaviour
{
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private MouseFacing facing;
    [SerializeField] private Transform visualRoot;
    [SerializeField] private Animator animator;

    public event Action StartSpecialAttacked;

    private void Update()
    {
        ApplyAnimation();
    }

    private void ApplyAnimation()
    {
        animator.SetFloat("speed", movement.IsMoving ? 1f : 0f);
        animator.SetInteger("direction", (int)facing.CurrentDirection);
        animator.SetBool("isSprint", movement.IsSprinting ? true : false);
        animator.SetBool("isAttack", movement.IsAttacking);
        ApplyFlip(facing.CurrentDirection);
    }

    private void ApplyFlip(MouseFacing.Direction dir)
    {
        Vector3 scale = visualRoot.localScale;
        if (movement.IsAttacking) return;
        if (dir == MouseFacing.Direction.UL || dir == MouseFacing.Direction.DL)
            scale.x = -Mathf.Abs(scale.x);
        else if (dir == MouseFacing.Direction.UR || dir == MouseFacing.Direction.DR)
            scale.x = Mathf.Abs(scale.x);
        
        visualRoot.localScale = scale;
    }

    public void ToggledRestAnimation(bool isRest)
    {
        animator.SetBool("isRest", isRest);
    }

    public void ToggledSpecialAttackAnim(bool isAttack)
    {
        animator.SetBool("isSpecialAttack", isAttack);
    }

    public void BlockSpecialAttackAnim()
    {
        animator.SetBool("isSpecialAttacking", true);
    }

    public void AllowSpecialAttackAnim()
    {
        animator.SetBool("isSpecialAttacking", false);
    }

    public void SpecialAttackStart()
    {
        ToggledSpecialAttackAnim(false);
        StartSpecialAttacked?.Invoke();
    }

    private void SpawnWalkEffect()
    {
        PlayerEffectManager.Instance.OnWalkEffect();
    }
}
