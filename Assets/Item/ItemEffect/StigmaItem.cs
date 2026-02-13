using Roguelike.Items;
using System.Collections.Generic;
using UnityEngine;

public class StigmaItem : MonoBehaviour
{
    [SerializeField] private PlayerCombat combat;
    [SerializeField] private float detectionRadius = 10f;
    [SerializeField] private LayerMask layerMask;

    [SerializeField] private float controlConstant = 1.0f;
    [SerializeField] private float pride = 1;
    private Dictionary<Monster, Stigma> activateStigmaMonsters = new();

    //搁了何
    private string indulgenceItemId = "IT_078_Item078";
    //部府钎
    private string tagItemId = "IT_060_Item060";
    //备汲荐
    private string rumorItemId = "IT_062_Item062";

    private string gazeItemId = "IT_045_Item045";

    private string hyperventilationItemId = "IT_065_Item065";

    public void SetPride(float pride) => this.pride = pride;

    private void OnEnable()
    {
        combat.OnAttackSuccessed += StartStigma;
    }

    private void OnDisable()
    {
        combat.OnAttackSuccessed -= StartStigma;
        StopAllCoroutines();
    }

    private void StartStigma(Monster monster, PlayerCombat.AttackType attackType)
    {
        IndulgenceEffect();
        bool isTagTrigger = (attackType == PlayerCombat.AttackType.NormalAttacked &&
            PlayerItemStatController.FindItem(tagItemId));
        bool isRumorTrigger = (attackType == PlayerCombat.AttackType.SpecialAttacked &&
            PlayerItemStatController.FindItem(rumorItemId));

        if (isTagTrigger || isRumorTrigger)
            AddOrUpdateStigma(monster);
    }

    private void AddOrUpdateStigma(Monster monster)
    {
        if (activateStigmaMonsters.ContainsKey(monster))
        {
            activateStigmaMonsters[monster].AddStack(1);
            return;
        }

        GameObject effect = ObjectPooler.Instance.SpawnFromPool("StigmaEffect", monster.transform.position, Quaternion.identity);
        if (effect.TryGetComponent<Stigma>(out Stigma stigmaEff))
        {
            stigmaEff.Init(monster, pride, controlConstant);
            stigmaEff.OnStigmaEnded += StigmaEnded;
            activateStigmaMonsters.Add(monster, stigmaEff);
        }
    }

    public void StigmaEnded(Monster monster)
    {
        Debug.Log(monster.name);
        activateStigmaMonsters.Remove(monster);
        if (monster.IsDead) return;
        if(PlayerItemStatController.FindItem(hyperventilationItemId))
        {
            monster.ResetMoveSpeed();
        }
        if(PlayerItemStatController.FindItem(gazeItemId))
        {
            monster.ResetAttackDamage();
        }
    }

    private void HyperventilationItemEffect(Monster monster)
    {
        if (!PlayerItemStatController.FindItem(hyperventilationItemId)) return;

        int value = PlayerItemStatController.GetItemValueByLevel(hyperventilationItemId, 0);

        float reductionMultiplier = 1f - (monster.GetMoveSpeed / 100f);
        reductionMultiplier = Mathf.Max(0f, reductionMultiplier);

        monster.SetMoveSpeed(monster.GetMoveSpeed * reductionMultiplier);
    }

    private void GazeItemEffect(Monster monster)
    {
        if (!PlayerItemStatController.FindItem(gazeItemId)) return;

        int value = PlayerItemStatController.GetItemValueByLevel(gazeItemId, 0);

        float reductionMultiplier = 1f - (monster.GetAttackDamage / 100f);
        reductionMultiplier = Mathf.Max(0f, reductionMultiplier);

        monster.SetAttackDamage(monster.GetAttackDamage * reductionMultiplier);
    }

    private void IndulgenceEffect()
    {
        if (!PlayerItemStatController.FindItem(indulgenceItemId)) return;

        int value = PlayerItemStatController.GetItemValueByLevel(indulgenceItemId, 0);

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius, layerMask);
        int currentMonsterCount = 0;
        foreach (Collider2D hit in hits)
        {
            if (!hit.TryGetComponent<Monster>(out Monster hitMonster)) continue;
            if (activateStigmaMonsters.ContainsKey(hitMonster)) continue;
            currentMonsterCount++;
            
            GameObject effect = ObjectPooler.Instance.SpawnFromPool("StigmaEffect", hitMonster.transform.position, Quaternion.identity);
            if(effect.TryGetComponent<Stigma>(out Stigma stigmaEff))
            {
                stigmaEff.Init(hitMonster, pride, controlConstant);
                stigmaEff.OnStigmaEnded += StigmaEnded;
                activateStigmaMonsters.Add(hitMonster, stigmaEff);
            }

            if (currentMonsterCount == value) return;
        }

        int stigmaCount = activateStigmaMonsters.Count;
        int randomNum = Random.Range(1, 10);
        if (randomNum >= 10 - stigmaCount)
        {
            List<Stigma> targetStigmas = new List<Stigma>(activateStigmaMonsters.Values);

            foreach (Stigma stigma in targetStigmas)
            {
                if (stigma != null)
                {
                    stigma.AddStack(4);
                }
            }
        }
        
    }
}
