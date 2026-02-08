using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MonsterDetection))]
[RequireComponent(typeof(MonsterMovement))]
public class Monster : Character
{
    public enum ActionType
    {
        Attack,
        Move,
        Alert,
        Idle,
        None
    }
    public ActionType currentActionType;
    public ActionType nextActionType;

    public event Action<float, float> OnChangedHp;
    public event Action<float, float> OnChangedSuperArmor;

    [SerializeField] protected string monsterID;
    [SerializeField] private MonsterDetection detection;
    [SerializeField] private MonsterMovement movement;
    [SerializeField] private MonsterAttack attack;
    [SerializeField] private MonsterSpriteAnimator animator;
    [SerializeField] private BoxCollider2D col;

    [SerializeField] private bool hasAlert = false;
    [SerializeField] private List<Vector3> path = new();

    public Character currentTarget;

    private float lastDir;

    private bool isPlayerSide = false;
    private bool isAttacking = false;

    private float maxSuperArmor;
    private float currentSuperArmor = 0;

    private void Start()
    {
        var dm = DataManager.Instance;
        var baseData = dm.BaseData.monsters;
        var data = baseData.Find(m => m.monsterID == monsterID);
        if (data == null)
        {
            Debug.LogError("Monster ID is Null");
        }
        else
        {
            InitializeMonster(data);
        }
    }

    protected void InitializeMonster(CharacterStateDataContainer.MonsterData data)
    {
        InitializeCharacter(data.stats);
        detection.SetupDectectionRange(data.attackRange, data.chaseInRange, data.chaseOutRange, data.cognizanceRange);        detection.SetIsPlayerSide(false);
        movement.SetupMovement(moveSpeed);
        attack.SetAttackStat(attackDamage, attackSpeed);
        maxSuperArmor = data.maxSuperArmor;
        currentSuperArmor = maxSuperArmor;
    }

    private void OnEnable()
    {
        detection.OnDetectionStateChanged += ChangeState;
        attack.OnAttackEnd += AttackEnd;
        attack.OnStartedAttack += Attack;
        animator.OnEndedStun += EndedStun;
        col.isTrigger = false;
    }

    private void OnDisable()
    {
        detection.OnDetectionStateChanged -= ChangeState;
        attack.OnAttackEnd -= AttackEnd;
        attack.OnStartedAttack -= Attack;
        animator.OnEndedStun -= EndedStun;
    }

    private void Update()
    {
        if (isAttacking) return;
        CheckDirection();
    }
    private void CheckDirection()
    {
        if (currentTarget == null || currentTarget.transform == null) return;
        Vector3 direction = (currentTarget.transform.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
        if ((Mathf.Abs(direction.x) < 0.001f)) return;

        float currentDir = direction.x < 0 ? 1f : -1f;

        if (currentDir != lastDir)
        {
            lastDir = currentDir;
            animator.ApplyFlip(lastDir);
        }
    }
    protected virtual void ChangeState(MonsterDetection.DetectionState detectionState, Character target)
    {
        currentTarget = target;
        movement.SetTarget(currentTarget);
        ActionType targetAction = ActionType.Idle;
        switch (detectionState)
        {
            case MonsterDetection.DetectionState.Attack: 
                targetAction = ActionType.Attack; 
                break;
            case MonsterDetection.DetectionState.Chase: 
                targetAction = ActionType.Move; 
                break;
            case MonsterDetection.DetectionState.Cognizance:
                if (hasAlert)
                    targetAction = ActionType.Alert;
                else
                    targetAction = ActionType.Move;
                    break;
            case MonsterDetection.DetectionState.None: 
                targetAction = ActionType.Idle; 
                break;
        }

        if (isAttacking)
        {
            nextActionType = targetAction;
            return;
        }

        ExecuteAction(targetAction);
    }

    public void ExecuteAction(ActionType action)
    {
        currentActionType = action;

        switch (action)
        {
            case ActionType.Attack:
                isAttacking = true;
                OnAttackRoutine();
                break;
            case ActionType.Move:
                movement.SetWaiting(false);
                animator.ApplyAnimation("isMove", true);
                break;
            case ActionType.Idle:
                movement.SetWaiting(true);
                animator.ApplyAnimation("isMove", false);
                break;
            case ActionType.Alert:
                movement.SetWaiting(true);
                animator.ApplyAnimation("isAlert", true);
                break;
        }
    }

    private void OnAttackRoutine()
    {
        Vector3 dir = (currentTarget.transform.position - transform.position).normalized;
        attack.SetAttackDir(dir);
        animator.ApplyAnimation("isMove", false);
        animator.ApplyAnimation("isReadyAttack", true);
        movement.SetWaiting(true);
    }

    private void Attack()
    {
        animator.ApplyAnimation("isReadyAttack", false);
        animator.ApplyAnimation("isAttack", true);
    }

    private void AttackEnd(string animName, bool isActive)
    {
        CheckDirection();
        animator.ApplyAnimation(animName, isActive);
        if (nextActionType == ActionType.Attack || nextActionType == ActionType.None)
        {
            OnAttackRoutine();
        }
        else
        {
            isAttacking = false;
            movement.SetWaiting(false);
            currentActionType = nextActionType;
            nextActionType = ActionType.None;
            ExecuteAction(currentActionType);
        }
    }


    public override void Dead()
    {
        if(attack != null)
            attack.DespawnIndicator();

        attack.CancelInvoke();
        CancelInvoke();

        isDead = true;

        animator.ApplyAnimation("isDie", true);
        col.isTrigger = true;
        Invoke(nameof(Despawn), 1.5f);
    }

    private void Despawn() =>
        ObjectPooler.ReturnToPool(gameObject);

    public override void TakeDamage(float damage)
    {
        if(currentSuperArmor > 1)
        {
            float calcDamage = damage * 0.3f;
            currentSuperArmor -= calcDamage;
            if(currentSuperArmor <= 0) 
            { 
                currentSuperArmor = 0;
                base.TakeDamage(maxHp * 0.1f);
            }
            OnChangedSuperArmor?.Invoke(currentSuperArmor, maxSuperArmor);
            damage -= calcDamage;
        }
        movement.SetWaiting(true);
        animator.ApplyAnimation("isDamaged", true);
        base.TakeDamage(damage);
        OnChangedHp?.Invoke(currentHp, maxHp);
    }

    private void EndedStun()
    {
        movement.SetWaiting(false);
    }
}
