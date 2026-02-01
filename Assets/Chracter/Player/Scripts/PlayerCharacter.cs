using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInputManager))]
[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerCombat))]
[RequireComponent(typeof(PlayerRest))]
[RequireComponent(typeof(PlayerUI))]

public class PlayerCharacter : Character
{
    [SerializeField] private PlayerInputManager input;
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private PlayerSpriteAnimator animator;
    [SerializeField] private PlayerCombat combat;
    [SerializeField] private PlayerRest rest;
    [SerializeField] private PlayerUI ui;
    [SerializeField] private WeaponAnimator weapon;

    public event Action<float, float> OnHpChanged;
    public event Action<float, float> OnMpChanged;
    public event Action<float, float> OnExpChanged;

    private float currentMp;
    private float currentExp;

    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float maxMp = 100f;
    [SerializeField] private float maxExp = 100f;
    [SerializeField] private float specialAttackUsedMp = 5;

    private bool isAttacking = false;
    void Start()
    {
        ApplyMetaData();

        OnHpChanged?.Invoke(currentHp, maxHp);
        OnMpChanged?.Invoke(currentMp, maxMp);
        OnExpChanged?.Invoke(currentExp, maxExp);
    }

    private void ApplyMetaData()
    {
        var dm = DataManager.Instance;
        var baseData = dm.BaseData.player;

        maxHp = dm.GetFinalMaxHp();
        currentHp = maxHp;
        attackDamage = dm.GetFinalAtk();
        moveSpeed = dm.GetFinalMoveSpeed();
        attackSpeed = baseData.stats.attackSpeed;

        maxMp = dm.GetFinalMp();
        currentMp = maxMp;

        maxExp = baseData.baseMaxExp;
        currentExp = 0;

        sprintSpeed = baseData.sprintSpeed;
        specialAttackUsedMp = baseData.specialAttackUsedMp;

        movement.SetSpeed(moveSpeed, sprintSpeed);
        combat.SetStats(attackDamage, attackSpeed);
    }

    private void OnEnable()
    {
        input.MoveVectorChanged += SetMoveInput;
        input.DashTriggered += OnDash;
        input.SprintStarted += ActivateSprintMode;
        input.SprintEnded += DeactivateSprintMode;
        input.NormalAttackTriggered += OnNormalAttack;
        input.SpecialAttackTriggered += OnSpecialAttack; // 내부 함수로 변경
        input.RestTriggered += RestModeChanged;
        input.StatusToggled += ToggleStatusDisplay;

        weapon.NormalAttackStarted += NormalAttackStarted;

        rest.OnHpCure += CureHp;

        OnHpChanged += ui.HpBarUpdate;
        OnMpChanged += ui.MpBarUpdate;
        OnExpChanged += ui.ExpBarUpdate;
    }

    private void NormalAttackStarted(Vector3 dir)
    {
        movement.OnAttack(dir);
    }

    private void OnDisable()
    {
        input.MoveVectorChanged -= movement.SetMoveInput;
        input.DashTriggered -= movement.OnDash;
        input.SprintStarted -= movement.ActivateSprintMode;
        input.SprintEnded -= movement.DeactivateSprintMode;
        input.NormalAttackTriggered -= OnNormalAttack;
        input.SpecialAttackTriggered -= combat.OnSpecialAttack;
        input.RestTriggered -= rest.RestModeChanged;
        input.StatusToggled -= ui.ToggleStatusDisplay;
        rest.OnHpCure -= CureHp;

        OnHpChanged -= ui.HpBarUpdate;
        OnMpChanged -= ui.MpBarUpdate;
        OnExpChanged -= ui.ExpBarUpdate;
    }

    void SetMoveInput(Vector2 moveInput)
    {
        movement.SetMoveInput(moveInput);
    }

    void OnDash(Vector2 moveInput)
    {
        movement.OnDash(moveInput);
    }

    void ActivateSprintMode()
    {
        movement.ActivateSprintMode();
    }

    void DeactivateSprintMode()
    {
        movement.DeactivateSprintMode();
    }

    void OnNormalAttack()
    {
        combat.OnNormalAttack();
    }

    void OnSpecialAttack()
    {
        if(UsedMp(specialAttackUsedMp))
            combat.OnSpecialAttack();
    }

    void RestModeChanged(bool isResting)
    {
        if(isResting)
            combat.EnableRestMode();
        else
            combat.DisableRestMode();

        animator.ToggledRestAnimation(isResting);
        rest.RestModeChanged(isResting);
    }

    void ToggleStatusDisplay()
    {
        ui.ToggleStatusDisplay();
    }

    public override void TakeDamage(float damage)
    {
        // 방어력 적용 (PlayerData에 있는 defense 활용)
        float defense = DataManager.Instance.BaseData.player.defense;
        float finalDamage = Mathf.Max(damage - defense, 1); // 최소 1 데미지

        base.TakeDamage(finalDamage);
        OnHpChanged?.Invoke(currentHp, maxHp);
    }

    private bool UsedMp(float useMp)
    {
        if (currentMp >= useMp)
        {
            currentMp -= useMp;
            OnMpChanged?.Invoke(currentMp, maxMp);
            return true;
        }
        return false;
    }

    private void CureMp(float amountMp)
    {
        currentMp = Mathf.Clamp(currentMp + amountMp, 0, maxMp);
        OnMpChanged?.Invoke(currentMp, maxMp);
    }

    private void CureHp(float amountHp)
    {
        currentHp = Mathf.Clamp(currentHp + amountHp, 0, maxHp);
        OnHpChanged?.Invoke(currentHp, maxHp);
    }

    public override void Dead()
    {
        Debug.Log("플레이어 사망");
        // SaveData에 현재까지 모은 포인트를 저장하는 로직 추가 가능
    }
}
