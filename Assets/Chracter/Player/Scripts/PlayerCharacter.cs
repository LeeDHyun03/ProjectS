using System;
using UnityEngine;

public class PlayerCharacter : State
{
    //    public string name;
    //    public float maxHp;
    //    public float attackDamage;
    //    public float attackRange;
    //    public float attackDelay;
    //    public float moveSpeed;
    [SerializeField] private PlayerInputManager input;
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private PlayerSpriteAnimator animator;
    [SerializeField] private PlayerCombat combat;
    [SerializeField] private PlayerRest rest;
    [SerializeField] private PlayerUI ui;
    
    public event Action<float, float> OnHpChanged;
    public event Action<float, float> OnMpChanged;
    public event Action<float, float> OnExpChanged;

    private float currentMp;
    private float currentExp;

    [SerializeField] private float sprintSpeed = 10f;

    [SerializeField] private float maxMp = 100f;
    [SerializeField] private float maxExp = 100f;
    [SerializeField] private float specialAttackUsedMp = 5;

    public float GetMaxHp()
    {
        return myType.maxHp;
    }
    public override void Awake()
    {
        base.Awake();
        ui ??= GetComponent<PlayerUI>();
        movement.SetSpeed(myType.moveSpeed, sprintSpeed);
    }
    private void OnEnable()
    {
        input.MoveVectorChanged += SetMoveInput;
        input.DashTriggered += OnDash;
        input.SprintStarted += ActivateSprintMode;
        input.SprintEnded += DeactivateSprintMode;
        input.NormalAttackTriggered += combat.OnNormalAttack;
        input.SpecialAttackTriggered += combat.OnSpecialAttack;
        input.RestTriggered += RestModeChanged;
        input.StatusToggled += ToggleStatusDisplay;
        rest.OnHpCure += CureHp;
        OnHpChanged += ui.HpBarUpdate;
        OnMpChanged += ui.MpBarUpdate;
        OnExpChanged += ui.ExpBarUpdate;
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
    private void OnDisable()
    {
        input.MoveVectorChanged -= movement.SetMoveInput;
        input.DashTriggered -= movement.OnDash;
        input.SprintStarted -= movement.ActivateSprintMode;
        input.SprintEnded -= movement.DeactivateSprintMode;
        input.NormalAttackTriggered -= combat.OnNormalAttack;
        input.SpecialAttackTriggered -= combat.OnSpecialAttack;
        input.RestTriggered -= rest.RestModeChanged;
        input.StatusToggled -= ui.ToggleStatusDisplay;
        OnHpChanged -= ui.HpBarUpdate;
        OnMpChanged -= ui.MpBarUpdate;
        OnExpChanged -= ui.ExpBarUpdate;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override void Dead()
    {
        throw new System.NotImplementedException();
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        OnHpChanged?.Invoke(currentHp, myType.maxHp);
    }

    private bool UsedMp(float useMp)
    {
        float tempMp = currentMp;
        if (tempMp - useMp > 0)
        {
            currentMp -= useMp;
            return true;
        }
        else 
            return false;
    }

    private void CureMp(float amountMp)
    {
        currentMp = Math.Clamp(currentMp + amountMp, 0, myType.maxHp);
        OnMpChanged?.Invoke(currentMp, maxMp);
    }

    private void CureHp(float amountHp)
    {
        currentHp = Math.Clamp(currentHp + amountHp, 0, myType.maxHp);
        OnHpChanged?.Invoke(currentHp, myType.maxHp);
    }
}
