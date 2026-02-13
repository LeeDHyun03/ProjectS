using System;
using UnityEngine;

[RequireComponent(typeof(PlayerInputManager))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float attackDashSpeed = 10;
    [SerializeField] private float maxDashTimer = 0.3f;
    [SerializeField] private float maxAttackTimer = 0.1f;

    private float currentSpeed;
    private float currentDashTimer = 0f;
    private float currentAttackTimer = 0f;

    private bool isDashing = false;
    private bool isAttack = false;

    private Vector3 moveDir;
    private Vector3 dashDir;
    private Vector3 attackDir;

    public event Action OnDashEnded;

    public bool IsMoving => moveDir.sqrMagnitude > 0.0001f;

    public bool IsSprinting => currentSpeed == sprintSpeed;

    public bool IsAttacking => isAttack;

    private bool isAttackDash = false;

    private bool isSpecialAttack = false;

    private bool isSpecialAttackDash = false;
    private void Awake()
    {
        currentSpeed = moveSpeed;
    }

    private void Update()
    {
        if(isSpecialAttackDash)
        {
            float dashDistance = attackDashSpeed * Time.deltaTime;
            transform.position += attackDir * dashDistance;
            currentAttackTimer += Time.deltaTime;
            if (currentAttackTimer >= maxAttackTimer)
            {
                isSpecialAttackDash = false;
                isSpecialAttack = false;
                currentAttackTimer = 0f;
            }
            return;
        }

        if (isSpecialAttack) return;

        if (isDashing)
        {
            float dashDistance = moveSpeed * 2f * Time.deltaTime;
            transform.position += dashDir.normalized * dashDistance;
            currentDashTimer += Time.deltaTime;
            if (currentDashTimer >= maxDashTimer)
            {
                isDashing = false;
                currentDashTimer = 0f;
                OnDashEnded?.Invoke();
            }
        }

        if (isAttackDash)
        {
            float dashDistance = attackDashSpeed * Time.deltaTime;
            transform.position += attackDir * dashDistance;
            currentAttackTimer += Time.deltaTime;
            if (currentAttackTimer >= maxAttackTimer)
            {
                isAttackDash = false;
                currentAttackTimer = 0f;
            }
            return;
        }

        if (IsMoving && !isAttack)
        {
            transform.position += moveDir * currentSpeed * Time.deltaTime;
        }
    }

    public void SetMoveInput(Vector2 input)
    {
        moveDir = new Vector3(input.x, input.y, 0f);
        moveDir = Vector3.ClampMagnitude(moveDir, 1f);
    }

    public void ActivateSprintMode()
    {
        currentSpeed = sprintSpeed;
    }

    public void DeactivateSprintMode()
    {
        currentSpeed = moveSpeed;
    }

    public void OnAttackDash(Vector2 attackDirection)
    {
        SetAttackDashDir(attackDirection);
        currentAttackTimer = 0;
        isAttack = true;
        isAttackDash = true;
    }

    public void SetAttackDashDir(Vector2 attackDirection)
    {
        attackDir = new Vector3(attackDirection.x, attackDirection.y, 0);
        attackDir.Normalize();
        isSpecialAttackDash = true;
    }

    public void SetIsAttack(bool newIsAttack)
    {
        isAttack = newIsAttack;
    }

    public void OnSpecialAttack()
    {
        isSpecialAttack = true;
    }

    public void OnDash(Vector2 dashDirection)
    {
        if(isDashing)
            return;
        dashDir = new Vector3(dashDirection.x, dashDirection.y, 0);
        isDashing = true;
    }

    public void SetSpeed(float newMoveSpeed, float newSprintSpeed)
    {
        moveSpeed = newMoveSpeed;
        sprintSpeed = newSprintSpeed;
    }
    
    public Vector3 GetMoveDir() => moveDir;
}
