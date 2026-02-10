using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Stigma : MonoBehaviour
{
    [SerializeField] private Image stigmaFillImage;

    private Monster owner;
    private float pride;
    private float controlConstant = 1.0f;
    private float lostHpPercent = 0.05f;

    public int CurrentStack { get; private set; } = 0;
    private const int maxStack = 4;
    private Coroutine expireRoutine;

    public event Action<Monster> OnStigmaEnded;

    private bool isActivate = false;

    public void Init(Monster target, float pride, float controlConstant)
    {
        CurrentStack = 0;
        owner = target;
        transform.SetParent(owner.transform);
        transform.localPosition = Vector3.zero;
        isActivate = true;
        this.pride = pride;
        this.controlConstant = controlConstant;
        AddStack(1);
    }

    public void AddStack(int stackAmount)
    {
        if (!isActivate) return;
        CurrentStack += stackAmount;
        stigmaFillImage.fillAmount += 0.25f;
        if (expireRoutine != null) StopCoroutine(expireRoutine);

        if(CurrentStack >= maxStack)
        {
            Explode(true);
        }
        else
        {
            expireRoutine = StartCoroutine(ExpireTimer());
        }
    }

    private IEnumerator ExpireTimer()
    {
        yield return new WaitForSeconds(3f);
        Explode(false);
    }

    private void Explode(bool isMax)
    {
        if (owner == null) return;

        float damage = Mathf.Clamp(CurrentStack, 1, 4) * pride * controlConstant;

        if(isMax)
        {
            float missingHp = owner.GetMaxHp - owner.GetCurrentHp;
            damage += missingHp * lostHpPercent;
        }
        OnStigmaEnded?.Invoke(owner);
        Debug.Log($"³«ÀÎ µ¥¹ÌÁö: {damage}");
        ObjectPooler.Instance.SpawnFromPool("StigmaHitEffect", owner.transform.position, Quaternion.identity);
        owner.TakeDamage(damage);
        transform.SetParent(null);
        OnStigmaEnded = null;
        isActivate = false;
        ObjectPooler.ReturnToPool(gameObject);
    }
}
