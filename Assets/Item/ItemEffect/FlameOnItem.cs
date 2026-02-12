using Roguelike.Items;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class FlameOnItem : MonoBehaviour
{

    [SerializeField] private PlayerCombat combat;
    private readonly string lamplightItemId = "IT_054_Item054";
    private readonly string ignitionItemId = "IT_032_Item032";
    private readonly string gasItemId = "IT_079_Item079";
    private readonly string emberItemId = "IT_004_Item004";
    private readonly string fireflyItemId = "IT_105_Item105";
    private readonly string oxygenItemId = "IT_014_Item014";
    private readonly string arsonItemId = "IT_016_Item016";
    private readonly string fierceFlamesItemId = "IT_116__";

    [SerializeField] private float detectionRadius = 10f;
    [SerializeField] private LayerMask layerMask;

    [SerializeField] private float controlConstant = 1.0f;
    [SerializeField] private float anger = 1;

    [SerializeField] private int defaultFlameOnDuration = 4;
    private int currentFlameOnDuration = 0;

    [SerializeField] private int defaultFlameOnStack = 4;
    private int currentFlameOnStack = 0;
    private Dictionary<Monster, FlameOn> activateFlameMonsters = new();

    public void SetAnger(float anger) => this.anger = anger;

    private void OnEnable()
    {
        combat.OnAttackSuccessed += StartFlameOn;
    }

    private void OnDisable()
    {
        combat.OnAttackSuccessed -= StartFlameOn;
        StopAllCoroutines();
    }

    private void StartFlameOn(Monster monster, PlayerCombat.AttackType attackType)
    {
        SetFlameOnDuration();
        SetFlameOnMaxStack();

        OxygenItemEffect(monster);
        FierceFlamesItemEffect(monster);

        if (attackType == PlayerCombat.AttackType.NormalAttacked)
            LamplightItemEffect(monster);


        if (attackType == PlayerCombat.AttackType.SpecialAttacked)
            IgnitionItemEffect(monster);
    }

    private void LamplightItemEffect(Monster monster)
    {
        if (!PlayerItemStatController.FindItem(lamplightItemId)) return;

        int value = PlayerItemStatController.GetItemValueByLevel(lamplightItemId, 0);

        SpawnFlame(monster, value);
    }

    private void IgnitionItemEffect(Monster monster)
    {
        if (!PlayerItemStatController.FindItem(ignitionItemId)) return;

        int value = PlayerItemStatController.GetItemValueByLevel(ignitionItemId, 0);

        SpawnFlame(monster, value);
    }

    private void FierceFlamesItemEffect(Monster monster)
    {
        if (!PlayerItemStatController.FindItem(fierceFlamesItemId)) return;

        int value = PlayerItemStatController.GetItemValueByLevel(fierceFlamesItemId, 1);

        SpawnFlame(monster, value);
    }

    private void OxygenItemEffect(Monster monster)
    {
        if (!PlayerItemStatController.FindItem(oxygenItemId)) return;

        int value = PlayerItemStatController.GetItemValueByLevel(lamplightItemId, 0);

        if (activateFlameMonsters[monster].CurrentStack < currentFlameOnStack) return;

        monster.TakeDamage(currentFlameOnStack * value);
        activateFlameMonsters[monster].FlameOnEnded();
        activateFlameMonsters.Remove(monster);
    }

    public void SpawnFlame(Monster monster, int index)
    {
        if (activateFlameMonsters.ContainsKey(monster))
        {
            if(EmberItemEffect())
            {
                activateFlameMonsters[monster].AddMaxDuration();
            }
            activateFlameMonsters[monster].AddStack(1);
            return;
        }

        GameObject effect = ObjectPooler.Instance.SpawnFromPool("FlameOnEffect", monster.transform.position, Quaternion.identity);
        if (effect.TryGetComponent<FlameOn>(out FlameOn flameOnEff))
        {
            flameOnEff.Init(monster, anger, controlConstant, currentFlameOnDuration, currentFlameOnStack, index);
            flameOnEff.OnFlameOnMonsterDie += FlameOnMonsterDie;
            flameOnEff.OnFlameOnEnded += RemoveActivateFlameOnMonster;
            activateFlameMonsters.Add(monster, flameOnEff);
        }
    }

    private void RemoveActivateFlameOnMonster(Monster monster)
    {
        activateFlameMonsters.Remove(monster);
    }
    
    private void SetFlameOnDuration()
    {
        if (!PlayerItemStatController.FindItem(gasItemId))
        {
            currentFlameOnDuration = defaultFlameOnDuration;
            return;
        }

        int value = PlayerItemStatController.GetItemValueByLevel(lamplightItemId, 0);

        currentFlameOnDuration = defaultFlameOnDuration + value;
    }

    private void SetFlameOnMaxStack()
    {
        if (!PlayerItemStatController.FindItem(arsonItemId))
        {
            currentFlameOnStack = defaultFlameOnStack;
            return;
        }

        int value = PlayerItemStatController.GetItemValueByLevel(lamplightItemId, 0);

        currentFlameOnStack = defaultFlameOnStack + value;
    }

    private void FlameOnMonsterDie()
    {
        if (!PlayerItemStatController.FindItem(fireflyItemId)) return;
        ApplyFlameOnToCircle();
    }

    private void ApplyFlameOnToCircle()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius, layerMask);
        foreach (Collider2D hit in hits)
        {
            if (!hit.TryGetComponent<Monster>(out Monster hitMonster)) continue;
            if (activateFlameMonsters.ContainsKey(hitMonster)) continue;

            GameObject effect = ObjectPooler.Instance.SpawnFromPool("FlameOnEffect", hitMonster.transform.position, Quaternion.identity);
            if (effect.TryGetComponent<FlameOn>(out FlameOn flameOnEff))
            {
                flameOnEff.Init(hitMonster, anger, controlConstant, currentFlameOnDuration, currentFlameOnStack, 1);
                flameOnEff.OnFlameOnMonsterDie += FlameOnMonsterDie;
                flameOnEff.OnFlameOnEnded += RemoveActivateFlameOnMonster;
                activateFlameMonsters.Add(hitMonster, flameOnEff);
                return;
            }
        }
    }

    private bool EmberItemEffect()
    {
        if (!PlayerItemStatController.FindItem(emberItemId)) return false;

        int value = PlayerItemStatController.GetItemValueByLevel(lamplightItemId, 0);

        int randomIndex = Random.Range(1, 101);
        if(randomIndex < value)
        {
            return true;
        }
        return false;
    }
}
