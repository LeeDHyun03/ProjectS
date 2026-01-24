using System.Collections;
using UnityEngine;
public enum MonsterMotion
{
    None,
    Pursuit,
    Attaking
}

public abstract class MonsterMovement : MonoBehaviour
{
    [SerializeField]protected LayerMask targetLayer;
    protected Animator anim => GetComponent<Animator>();
    float attackingCool;
    protected MonsterMotion motion;
    protected MonsterState myState => GetComponent<MonsterState>();
    protected ContactFilter2D contactFilter;
    [SerializeField]protected State currentTarget;
    WaitForSeconds setTargetWaitTime = new WaitForSeconds(0.2f);
    bool _isTargeted;
    public bool isTargeted
    {
        get => _isTargeted;
        set
        {
            if (_isTargeted == value) return;
            _isTargeted = value;

            if (!isTargeted)
            {
                currentTarget = null;
                motion = MonsterMotion.None;
                StartCoroutine(SetTargetCoroutine());
            }
        }
    }

    protected MonsterType myType => myState.monsterType;
    [SerializeField]bool isPatrol;
    private void Awake()
    {
        contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(targetLayer); 
        contactFilter.useTriggers = true;       
        contactFilter.useLayerMask = true;      

    }
    private void Start()
    {
        attackingCool = myType.attackDelay;
        StartCoroutine(SetTargetCoroutine());
    }

    void Update()
    {
        if (myState.isDead)
            return;

        MotionChange();

        if (!KeepTargeting())
        {
            isTargeted = false;
        }

        if(motion == MonsterMotion.Attaking)
        {
            AttackingRoutine();
        }
        else if (motion == MonsterMotion.Pursuit)
        {
            MoveToTarget();
        }
        else if (isPatrol && !isTargeted)
        {
            PatrolMove();
        }
    }
    public void Attack()
    {
        if (!isTargeted)
        {
            return;
        }
        if(isTargetInAttackRange())
        {
            currentTarget.TakeDamage(myType.attackDamage);
            Debug.Log($"{myType.attackDamage}만큼 공격 받음. {currentTarget.name}의 currentHp: {currentTarget.GetCurrentHp()}");
        }
    }
    public void MoveToDir(Vector3 dir, float speed = 1)
    {
        transform.Translate(dir * myType.moveSpeed * speed * Time.deltaTime);
    }
    void MoveToTarget()
    {
        Vector3 toTargetDir = (currentTarget.transform.position - transform.position).normalized;

        MoveToDir(toTargetDir);
    }
    IEnumerator SetTargetCoroutine()
    {
        while (!myState.isDead && !isTargeted)
        {
            yield return setTargetWaitTime;
            var tar = SetTarget();
            if (tar != null)
            {
                currentTarget = tar;
                isTargeted = true;
            }
        }
    }
    State SetTarget()
    {
        Debug.Log("타겟 찾는 중...");
        Collider2D tar = Physics2D.OverlapCircle(transform.position, myType.pursuitRange, targetLayer);

        return tar.GetComponent<State>();
    }
    void AttackingRoutine()
    {
        if (isTargetInAttackRange())
        {
            attackingCool -= Time.deltaTime;
            if (attackingCool <= 0)
            {
                attackingCool = myType.attackDelay;
                Attack();
            }
        }
        else
            attackingCool = myType.attackDelay;
    }
    void MotionChange()
    {
        if (isTargetInAttackRange())
            motion = MonsterMotion.Attaking;
        else if (isTargeted)
            motion = MonsterMotion.Pursuit;
        else
            motion = MonsterMotion.None;
    }
    void PatrolMove()
    {
        if (myType.patrolRange <= 0) return;
    }
    void SetPatrolPos()
    {

    }

    protected bool isTargetInAttackRange()
    {
        if(currentTarget == null) 
            return false;

        return (currentTarget.transform.position - transform.position).magnitude <= myType.attackRange;
    }
    public virtual bool KeepTargeting()
    {
        if(currentTarget == null)
            return false;

        return (currentTarget.transform.position - transform.position).magnitude <= myType.pursuitRange;
    }
    public abstract void OnHitReaction();
}