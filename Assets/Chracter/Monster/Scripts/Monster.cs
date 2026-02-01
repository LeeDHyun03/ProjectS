using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [SerializeField] protected string monsterID;
    [SerializeField] private MonsterDetection detection;
    [SerializeField] private MonsterMovement movement;
    [SerializeField] private MonsterAttack attack;
    [SerializeField] private MonsterSpriteAnimator animator;

    [SerializeField] private bool hasAlert = false;
    [SerializeField] private List<Vector3> path = new();

    private Character currentTarget;

    private float lastDir;

    private bool isPlayerSide = false;
    private bool isAttacking = false;

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
    }

    private void OnEnable()
    {
        detection.OnDetectionStateChanged += ChangeState;
        attack.OnAttackEnd += AttackEnd;
        attack.OnStartedAttack += Attack;
    }

    private void OnDisable()
    {
        detection.OnDetectionStateChanged -= ChangeState;
        attack.OnAttackEnd -= AttackEnd;
        attack.OnStartedAttack -= Attack;
    }

    private void Update()
    {
        if (isAttacking) return;
        CheckDirection();
    }
    private void CheckDirection()
    {
        if (currentTarget == null) return;
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

        Debug.Log($"detectionState: {detectionState}");

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

        Debug.Log($"현재 액션: {action}");
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
        animator.ApplyAnimation("isDie", true);
    }
}
