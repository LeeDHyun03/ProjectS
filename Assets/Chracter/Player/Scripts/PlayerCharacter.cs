using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInputManager))]
[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerCombat))]
[RequireComponent(typeof(PlayerRest))]

public class PlayerCharacter : Character
{
    [SerializeField] private PlayerInputManager input;
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private PlayerSpriteAnimator animator;
    [SerializeField] private PlayerCombat combat;
    [SerializeField] private PlayerRest rest;
    [SerializeField] private WeaponAnimator weapon;

    private PlayerUI ui;
    private PlayerStatRefreshScheduler statRefreshScheduler;

    private PlayerItemStatController itemStatController;

    public event Action<float, float> OnHpChanged;
    public event Action<float, float> OnMpChanged;
    public event Action<float, float> OnExpChanged;
    public event Action<float, float, float, float, float, float, float, float, float, float> OnStatsUpdate;

    public event Action<bool, float> OnChagnedDashCount;

    public event Action OnLevelUp;

    private float currentMp;
    private float currentExp;

    private float defense;
    private float critChance;
    private float critDamage;
    private float meleeDamage;
    private float rangedDamage;
    private float anger;
    private float pride;
    private float jealousy;
    private int rerollCount;
    private float expIncrement;

    private bool isDashing = false;
    private bool isInvincibility = false;

    private Coroutine invincibilityDurationRoutine;

    private int maxDashCount = 3;
    private int currentDashCount = 3;
    private float dashChargedTime = 1;
    private float dashChargedCurrentTime = 0;

    private float mpChargedCurrentTime = 0;
    private float mpChargedTime = 1;

    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float maxMp = 100f;
    [SerializeField] private float maxExp = 100f;
    [SerializeField] private float specialAttackUsedMp = 5;

    public bool isInteracting = false;

    public float CurrentHp => currentHp;
    public float MaxHp => maxHp;
    public float CurrentMp => currentMp;
    public float MaxMp => maxMp;
    
    #region Debug Func
    public float DebugAttackDamage => attackDamage;
    public float DebugMoveSpeed => moveSpeed;
    public float DebugAttackSpeed => normalAttackSpeed;
    public float CritChance => critChance;
    #endregion


    void Start()
    {
        ApplyMetaData();

        ui = PlayerUI.Instance;
        statRefreshScheduler = GetComponent<PlayerStatRefreshScheduler>();

        if (ui != null)
        {
            OnHpChanged += ui.HpBarUpdate;
            OnMpChanged += ui.MpBarUpdate;
            OnExpChanged += ui.ExpBarUpdate;
            OnStatsUpdate += ui.StatsUpdate;
            OnChagnedDashCount += ui.ChagnedDashGageBar;
            OnLevelUp += ui.ActivateRewardScreen;
        }

        if (MonsterManager.Instance != null)
        {
            MonsterManager.Instance.OnDeathMonster += GainExp;
        }

        OnHpChanged += statRefreshScheduler.MarkDirty;
        OnMpChanged += statRefreshScheduler.MarkDirty;

        OnHpChanged?.Invoke(currentHp, maxHp);
        OnMpChanged?.Invoke(currentMp, maxMp);
        OnExpChanged?.Invoke(currentExp, maxExp);
    }

    #region Event Subscribe
    private void OnEnable()
    {
        input.MoveVectorChanged += SetMoveInput;
        input.DashTriggered += OnDash;
        input.SprintStarted += ActivateSprintMode;
        input.SprintEnded += DeactivateSprintMode;
        input.NormalAttackTriggered += OnNormalAttack;
        input.SpecialAttackTriggered += OnSpecialAttack;
        input.InteractTriggered += OnInteractionTrigger;
        input.RestTriggered += RestModeChanged;
        input.StatusToggled += ToggleStatusDisplay;

        weapon.NormalAttackStarted += NormalAttackStarted;
        weapon.NormalAttackCompleted += NormalAttackEnded;

        combat.SpecialAttackTriggered += OnSpecialAttackDash;

        rest.OnHpCure += CureHp;

        animator.StartSpecialAttacked += OnActivateSpecialAttack;

        movement.OnDashEnded += DashEnded;

        if (MonsterManager.Instance != null)
        {
            MonsterManager.Instance.OnDeathMonster += GainExp;
        }

        if (ui != null)
        {
            OnHpChanged += ui.HpBarUpdate;
            OnMpChanged += ui.MpBarUpdate;
            OnExpChanged += ui.ExpBarUpdate;
            OnStatsUpdate += ui.StatsUpdate;
            OnChagnedDashCount += ui.ChagnedDashGageBar;
            OnLevelUp += ui.ActivateRewardScreen;
        }

        if (statRefreshScheduler != null)
        {
            OnHpChanged += statRefreshScheduler.MarkDirty;
            OnMpChanged += statRefreshScheduler.MarkDirty;
        }

    }

    private void OnDisable()
    {
        input.MoveVectorChanged -= SetMoveInput;
        input.DashTriggered -= OnDash;
        input.SprintStarted -= ActivateSprintMode;
        input.SprintEnded -= DeactivateSprintMode;
        input.NormalAttackTriggered -= OnNormalAttack;
        input.SpecialAttackTriggered -= OnSpecialAttack;
        input.InteractTriggered -= OnInteractionTrigger;
        input.RestTriggered -= RestModeChanged;
        input.StatusToggled -= ToggleStatusDisplay;

        rest.OnHpCure -= CureHp;

        weapon.NormalAttackStarted -= NormalAttackStarted;
        weapon.NormalAttackCompleted -= NormalAttackEnded;

        combat.SpecialAttackTriggered -= OnSpecialAttackDash;

        animator.StartSpecialAttacked -= OnActivateSpecialAttack;

        movement.OnDashEnded -= DashEnded;

        if (MonsterManager.Instance != null)
        {
            MonsterManager.Instance.OnDeathMonster -= GainExp;
        }

        if (ui != null)
        {
            OnHpChanged -= ui.HpBarUpdate;
            OnMpChanged -= ui.MpBarUpdate;
            OnExpChanged -= ui.ExpBarUpdate;
            OnStatsUpdate -= ui.StatsUpdate;
            OnChagnedDashCount -= ui.ChagnedDashGageBar;
            OnLevelUp -= ui.ActivateRewardScreen;
        }

        if (statRefreshScheduler != null)
        {
            OnHpChanged -= statRefreshScheduler.MarkDirty;
            OnMpChanged -= statRefreshScheduler.MarkDirty;
        }
    }

    #region EventFunc

    private void NormalAttackStarted(Vector2 dir)
    {
        movement.OnAttackDash(dir);
    }

    private void NormalAttackEnded()
    {
        movement.SetIsAttack(false);
    }

    void SetMoveInput(Vector2 moveInput)
    {
        movement.SetMoveInput(moveInput);
    }

    void OnDash(Vector2 moveInput)
    {
        if (isDashing || currentDashCount <= 0) return;
        isDashing = true;
        currentDashCount--;
        OnChagnedDashCount?.Invoke(false, 0);
        isInvincibility = true;
        invincibilityDurationRoutine = StartCoroutine(TakeDamageInvincibility(0.25f));
        movement.OnDash(moveInput);
    }

    void DashEnded()
    {
        isDashing = false;
    }

    void ActivateSprintMode()
    {
        movement.ActivateSprintMode();
    }

    void DeactivateSprintMode()
    {
        movement.DeactivateSprintMode();
    }

    void OnNormalAttack(Vector2 attackDir)
    {
        movement.SetIsAttack(true);
        combat.OnNormalAttack(attackDir);
    }

    void OnSpecialAttack()
    {
        if (UsedMp(specialAttackUsedMp))
        {
            movement.OnSpecialAttack();
            animator.ToggledSpecialAttackAnim(true);
        }
    }

    public void OnActivateSpecialAttack()
    {
        combat.OnSpecialAttack();
    }

    void OnSpecialAttackDash(Vector2 attackDir)
    {
        movement.SetAttackDashDir(-attackDir);
    }

    void OnInteractionTrigger()
    {
        isInteracting = true;
    }

    IEnumerator OnInteractionRoutine()
    {
        yield return new WaitForSeconds(0.3f);
        isInteracting = false;
    }

    void RestModeChanged(bool isResting)
    {
        if (isResting)
        {
            combat.EnableRestMode();
            weapon.SetIsAttacking(true);
        }
        else
        {
            combat.DisableRestMode();
            weapon.SetIsAttacking(false);
        }

        animator.ToggledRestAnimation(isResting);
        rest.RestModeChanged(isResting);
    }

    void ToggleStatusDisplay()
    {
        ui.ToggleStatusDisplay();
    }
    #endregion

    #endregion

    #region State
    private void ApplyMetaData()
    {
        var dm = DataManager.Instance;
        var baseData = dm.BaseData.player;

        var baseStats = BuildBaseStatsFromDataManager();

        maxHp = baseStats.maxHp;
        currentHp = maxHp;

        attackDamage = baseStats.attackDamage;
        moveSpeed = baseStats.moveSpeed;
        normalAttackSpeed = baseStats.normalAttackSpeed;

        maxMp = baseStats.maxMp;
        currentMp = maxMp;

        defense = baseStats.defense;
        critChance = baseStats.critChance;
        critDamage = baseStats.critDamage;
        meleeDamage = 0;
        rangedDamage = 0;
        pride = baseStats.pride;
        jealousy = baseStats.jealousy;
        anger = baseStats.anger;

        maxExp = baseData.baseMaxExp;
        currentExp = 0;

        rerollCount = baseStats.rerollCount;

        sprintSpeed = baseData.sprintSpeed;
        specialAttackUsedMp = baseData.specialAttackUsedMp;

        expIncrement = baseData.expIncrement;

        movement.SetSpeed(moveSpeed, sprintSpeed);
        combat.SetStats(attackDamage, normalAttackSpeed, critChance, critDamage, meleeDamage, rangedDamage, anger, pride, jealousy);

        OnStatsUpdate?.Invoke(maxHp, maxMp, moveSpeed, attackDamage, critChance, defense, anger, pride, jealousy, rerollCount);

        itemStatController = GetComponent<PlayerItemStatController>();
        if (itemStatController) itemStatController.RebuildAndApply();
    }

    public PlayerItemStatController.PlayerStatBlock BuildBaseStatsFromDataManager()
    {
        var dm = DataManager.Instance;

        return new PlayerItemStatController.PlayerStatBlock
        {
            maxHp = dm.GetFinalMaxHp(),
            attackDamage = dm.GetFinalAtk(),
            moveSpeed = dm.GetFinalMoveSpeed(),
            normalAttackSpeed = dm.GetFinalAttackSpeed(),

            maxMp = dm.GetFinalMp(),
            defense = dm.GetFinalDef(),
            critChance = dm.GetFinalCritChance(),
            critDamage = dm.GetFinalCritDamage(),
            pride = dm.GetFinalPride(),
            jealousy = dm.GetFinalJealousy(),
            anger = dm.GetFinalAnger(),
            rerollCount = dm.GetFinalRerollCount()


        };
    }

    public void ApplyFinalStats(PlayerItemStatController.PlayerStatBlock s)
    {
        maxHp = s.maxHp;
        currentHp = Mathf.Clamp(currentHp, 0, maxHp);

        attackDamage = s.attackDamage;
        normalAttackSpeed = s.normalAttackSpeed;
        moveSpeed = s.moveSpeed;

        maxMp = s.maxMp;
        currentMp = Mathf.Clamp(currentMp, 0, maxMp);
        if (currentMp <= 0) currentMp = maxMp;

        defense = s.defense;
        critChance = s.critChance;
        critDamage = s.critDamage;
        meleeDamage = s.normalAttackDamage;
        rangedDamage = s.specialAttackDamage;
        movement.SetSpeed(moveSpeed, sprintSpeed);
        combat.SetStats(attackDamage, normalAttackSpeed, critChance, critDamage, meleeDamage, rangedDamage, anger, pride, jealousy);

        OnHpChanged?.Invoke(currentHp, maxHp);
        OnMpChanged?.Invoke(currentMp, maxMp);
        OnStatsUpdate?.Invoke(maxHp, maxMp, moveSpeed, attackDamage, critChance, defense, anger, pride, jealousy, rerollCount);
    }
    #endregion

    public override void TakeDamage(float damage)
    {
        if (isInvincibility) return;
        float finalDamage = Mathf.Max(damage - defense, 1f);
        base.TakeDamage(finalDamage);
        PlayerEffectManager.Instance.OnHitEffect();
        OnHpChanged?.Invoke(currentHp, maxHp);
        isInvincibility = true;
        invincibilityDurationRoutine = StartCoroutine(TakeDamageInvincibility(0.5f));
    }

    private IEnumerator TakeDamageInvincibility(float invincibilityDuration)
    {
        yield return new WaitForSeconds(invincibilityDuration);
        isInvincibility = false;
        StopCoroutine(invincibilityDurationRoutine);
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

    public void CureHp(float amountHp)
    {
        currentHp = Mathf.Clamp(currentHp + amountHp, 0, maxHp);
        OnHpChanged?.Invoke(currentHp, maxHp);
    }

    public void GainExp()
    {
        currentExp += 7;
        if (currentExp >= maxExp)
        {
            LevelUp();
        }
        OnExpChanged?.Invoke(currentExp, maxExp);
    }

    private void LevelUp()
    {
        currentExp = 0;
        maxExp += expIncrement;
        CureHp(maxHp);
        OnLevelUp?.Invoke();
    }

    public override void Dead()
    {
        Debug.Log("플레이어 사망");
        // SaveData에 현재까지 모은 포인트를 저장하는 로직 추가 가능
    }

    void Update()
    {
        if(currentMp < maxMp)
        {
            mpChargedCurrentTime += Time.deltaTime;
            if(mpChargedCurrentTime >= mpChargedTime)
            {
                currentMp++;
                mpChargedCurrentTime = 0;
                OnMpChanged?.Invoke(currentMp, maxMp);                
            }
        }

        if(currentDashCount < maxDashCount)
        {
            dashChargedCurrentTime += Time.deltaTime;
            if (dashChargedCurrentTime >= dashChargedTime)
            {
                currentDashCount++;
                dashChargedCurrentTime = 0;
                OnChagnedDashCount?.Invoke(true, 1);
                return;
            }
            OnChagnedDashCount?.Invoke(true, dashChargedCurrentTime);
        }
    }
}
