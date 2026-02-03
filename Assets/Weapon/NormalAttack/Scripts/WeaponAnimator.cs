using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponAnimator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Weapon weapon;
    [SerializeField] private PlayerCombat combat;
    [SerializeField] private Transform weaponSocket;
    [SerializeField] private Transform visualRoot;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Animator animator;

    [Header("Settings")]
    [SerializeField] private float spriteAngleOffset = 180f;

    private Camera mainCam;
    private bool isAttacking = false;
    private int currentAttackCount = 0;

    public event Action<Vector2> NormalAttackStarted;
    public event Action NormalAttackCompleted;

    private Vector2 inputAttackDirection = Vector2.zero;

    private void Awake()
    {
        animator ??= GetComponent<Animator>();
        mainCam = Camera.main;
    }

    private void OnEnable()
    {
        if (combat != null)
        {
            combat.NormalAttackTriggered += OnNormalAttack;
            combat.SpecialAttackTriggered += OnSpecialAttack;
        }
    }

    private void OnDisable()
    {
        if (combat != null)
        {
            combat.NormalAttackTriggered -= OnNormalAttack;
            combat.SpecialAttackTriggered -= OnSpecialAttack;
        }
    }

    private void Update()
    {
        if (!isAttacking)
        {
            AimToMouse();
        }
    }

    public void SetIsAttacking(bool newIsAttacking)
    {
        isAttacking = newIsAttacking;
    }

    #region Aim Logic
    private void AimToMouse()
    {
        if (mainCam == null) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector3 weaponScreenPos = mainCam.WorldToScreenPoint(weaponSocket.position);
        Vector2 dir = mousePos - (Vector2)weaponScreenPos;

        if (dir.sqrMagnitude < 0.1f) return;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        if (weaponSocket.parent == visualRoot)
        {
            if (visualRoot.lossyScale.x > 0f)
            {
                angle = -angle + 180f;
            }

            Quaternion targetWorldRot = Quaternion.Euler(0f, 0f, angle);
            Quaternion parentWorldRot = visualRoot.rotation;
            Quaternion relativeRot = Quaternion.Inverse(parentWorldRot) * targetWorldRot;

            weaponSocket.localRotation = Quaternion.Euler(0, 0, relativeRot.eulerAngles.z + spriteAngleOffset);
        }
        else
        {
            weaponSocket.rotation = Quaternion.Euler(0, 0, angle + spriteAngleOffset);
        }
    }
    #endregion

    #region Attack Control (Combat Events)
    private void OnNormalAttack(Vector2 inputAttackDir)
    {
        currentAttackCount++;
        animator.SetInteger("attackCount", currentAttackCount);
        inputAttackDirection = inputAttackDir;
    }

    private void OnSpecialAttack()
    {

    }
    #endregion

    #region Animation Events
    private void AttackStarted()
    {
        isAttacking = true;

        weaponSocket.parent = playerTransform;
        AimToMouse();

        animator.SetBool("isAttacking", true);
    }

    private void AttackDash()
    {
        Vector2 dir = inputAttackDirection;
        if (dir == Vector2.zero)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Vector3 weaponScreenPos = mainCam.WorldToScreenPoint(transform.position);
            dir = (mousePos - (Vector2)weaponScreenPos).normalized;
        }
        NormalAttackStarted?.Invoke(dir);
    }

    private void CheckingMonster(int attackCount)
    {
        bool isFilp = true;
        bool isSecondAttack = false;

        weaponSocket.localScale = new Vector3(1, 1, 1);

        if (visualRoot.lossyScale.x < 0f)
        {
            isFilp = true;
        }
        if (attackCount == 2)
        {
            isSecondAttack = true;
        } 
        weapon.StartAttack(isSecondAttack, isFilp);
    }

    private void AttackEnded(int attackCount)
    {
        animator.speed = 1f;
        animator.SetBool("isAttacking", false);

        inputAttackDirection = Vector2.zero;

        NormalAttackCompleted?.Invoke();

        if (attackCount >= currentAttackCount)
        {
            currentAttackCount = 0;
            animator.SetInteger("attackCount", currentAttackCount);
        }

        combat.OnNormalAttackAnimationComplete();
    }

    private void SetWeaponParent()
    {
        if(currentAttackCount == 0)
        {
            isAttacking = false;
            weaponSocket.parent = visualRoot;
        }
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
    #endregion
}