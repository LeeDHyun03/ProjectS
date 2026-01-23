using System.Collections;
using UnityEngine;

public abstract class MonsterMovement : MonoBehaviour
{
    LayerMask targetLayer = LayerMask.NameToLayer("Player");
    protected MonsterState currentState;
    protected State currentTarget;
    WaitForSeconds setTargetWaitTime = new WaitForSeconds(0.2f);
    protected MonsterType myType => currentState.monsterType;
    private void Awake()
    {
        currentState = GetComponent<MonsterState>();
        StartCoroutine(SetTargetCoroutine());
    }
    void Start()
    {
        
    }

    void Update()
    {
        if (currentState.isDead)
            return;

        if (myType.isPatrol)
        {
            PatrolMove();
            return;
        }

        if (currentTarget == null)
            return;

        MoveToTarget();
    }
    public void Attack()
    {
        currentTarget.TakeDamage(myType.attackDamage);

        if (currentTarget.isDead)
            StartCoroutine(SetTargetCoroutine());
    }
    public void MoveToTarget()
    { 

    }
    IEnumerator SetTargetCoroutine()
    {
        while (!currentState.isDead && currentTarget != null)
        {
            yield return setTargetWaitTime;
            SetTarget();
        }
    }
    void SetTarget()
    {
        Collider[] entitys = Physics.OverlapSphere(transform.position, myType.attackRange, targetLayer);
        float closestDistSqr = Mathf.Infinity;
        Transform targetTrans = null;

        foreach (var t in entitys)
        {
            float dSqr = (t.transform.position - transform.position).sqrMagnitude;

            if (dSqr < closestDistSqr)
            {
                closestDistSqr = dSqr;
                targetTrans = t.transform;
            }
        }
        if (targetTrans != null)
            currentTarget = targetTrans.GetComponent<State>();
    }
    void PatrolMove()
    {

    }
    public abstract void OnHitReaction();
}