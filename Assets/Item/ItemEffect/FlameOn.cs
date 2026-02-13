using System;
using System.Collections;
using UnityEngine;

public class FlameOn : MonoBehaviour
{
    private Monster owner;
    private float anger;
    private float controlConstant = 1.0f;
    private float flameOnDuration = 0;
    private float currentFlameOnDuration = 0;
    public int CurrentStack { get; private set; } = 0;
    private int maxStack = 4;
    private Coroutine flameOnRoutine;

    public event Action<Monster> OnFlameOnEnded;
    public event Action OnFlameOnMonsterDie;

    private bool isActivate = false;

    public void Init(Monster target, float anger, float controlConstant, float flameOnDuration, int maxStack, int amountStack)
    {
        CurrentStack = 0;
        owner = target;
        transform.SetParent(owner.transform);
        transform.localPosition = Vector3.zero;
        isActivate = true;
        this.maxStack = maxStack;
        this.flameOnDuration = flameOnDuration;
        this.anger = anger;
        this.controlConstant = controlConstant;
        AddStack(amountStack);
        flameOnRoutine = StartCoroutine(AttackRoutine());
    }

    public void AddStack(int stack)
    {
        if (!isActivate) return;
        CurrentStack += stack;
    }
    
    public void AddMaxDuration()
    {
        flameOnDuration++;
    }

    private IEnumerator AttackRoutine()
    {
        if(owner.IsDead)
        {
            OnFlameOnMonsterDie?.Invoke();
            FlameOnEnded();
            yield return null;
        }
        while(currentFlameOnDuration < flameOnDuration)
        {
            yield return new WaitForSeconds(1f);
            currentFlameOnDuration++;
            float damage = anger * Mathf.Clamp(CurrentStack, 1, maxStack) * controlConstant;
            owner.TakeDamage(damage);
        }
        FlameOnEnded();
    }

    public void FlameOnEnded()
    {
        StopAllCoroutines();
        transform.SetParent(null);
        OnFlameOnEnded?.Invoke(owner);
        OnFlameOnMonsterDie = null;
        isActivate = false;
        ObjectPooler.ReturnToPool(gameObject);
    }
}
