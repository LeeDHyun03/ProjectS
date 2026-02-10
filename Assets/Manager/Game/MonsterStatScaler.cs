using UnityEngine;
using WaveStaticData;

public static class MonsterStatScaler
{
    private static float TryScaleField(float origin, StatScalingInfo field, int elapsedStage)
    {
        int stageIncrements = elapsedStage / field.unitStage;

        float calc(int increments)
        {
            return field.type switch
            {
                "simple" => origin * (1 + field.value * increments / 100f),
                "compound" => origin * Mathf.Pow(1 + field.value / 100f, increments),
                _ => throw new System.Exception("올바르지 않은 계산 방식")
            };
        }

        return calc(stageIncrements);
    }
    public static CharacterStateDataContainer.MonsterData TryScale(
        this CharacterStateDataContainer.MonsterData baseData
    )
    {
        // 풀링 준비 단계일 경우 조정 필요 없음
        if (GameManager.Instance == null) return baseData;

        return ScaleWorker(
            baseData,
            GameManager.Instance.CurrentScalingInfoGroup,
            GameManager.Instance.InExtraStage
                ? GameManager.Instance.ElapsedExtraStages
                : GameManager.Instance.currentStage
        );
    }
    private static CharacterStateDataContainer.MonsterData ScaleWorker(
        this CharacterStateDataContainer.MonsterData baseData,
        ScalingInfoGroup scalingInfoGroup,
        int elapsedStage
    )
    {
        // TODO: 방어력은 현재 몬스터 및 피해량 로직/데이터에 포함되어 있지 않은 관계로 제외
        return new()
        {
            monsterID = baseData.monsterID,
            stats = new()
            {
                maxHp = TryScaleField(baseData.stats.maxHp, scalingInfoGroup.health, elapsedStage),
                attackDamage = TryScaleField(baseData.stats.attackDamage, scalingInfoGroup.atk, elapsedStage),
                attackSpeed = baseData.stats.attackSpeed,
                moveSpeed = baseData.stats.moveSpeed
            },
            maxSuperArmor = TryScaleField(baseData.maxSuperArmor, scalingInfoGroup.supArm, elapsedStage),
            attackRange = baseData.attackRange,
            chaseInRange = baseData.chaseInRange,
            chaseOutRange = baseData.chaseOutRange,
            cognizanceRange = baseData.cognizanceRange
        };
    }
}
