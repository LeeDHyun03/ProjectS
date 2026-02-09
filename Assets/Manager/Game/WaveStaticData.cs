using System;
using System.Collections.Generic;
using UnityEngine;

namespace WaveStaticData
{
    [Serializable]
    public class Root
    {
        public GeneralData general;
        public StatScaleData statScaleData;
        public List<Waves> normalStages;
        public List<Waves> extraStages;
    }

    [Serializable]
    public class GeneralData
    {
        // 정규 최대 스테이지 (기본: 5)
        public int normalStageMax;

        // 하루에 나오는 웨이브 수
        public float waveAmountPerDay;

        // 웨이브 당 할당된 시간 (초)
        public float waveDuration;

        // 밤에 할당된 시간 (초)
        public float nightDuration;

        // 한 번에 소환되는 적의 수가 많을 때 확장되는 소환 영역 크기 배율의 최대치
        public float maxSpawnAreaExpansionFactor;
        // 한 번에 소환되는 적의 수가 많을 때 소환 범위를 늘리는 정도
        public float spawnAreaAdditionalScaleFactor;

        // 푸아송 샘플링 대신 랜덤 소환 방식을 선택할 최소 몬스터 수
        public int spawnRandomFallbackThreshold;

    }
    [Serializable]
    public class StatScaleData
    {
        public ScalingInfoGroup normalStages;
        public ScalingInfoGroup extraStages;
    }

    [Serializable]
    public class ScalingInfoGroup
    {
        public StatScalingInfo health;
        public StatScalingInfo atk;
        public StatScalingInfo def;
        public StatScalingInfo supArm;
    }

    [SerializeField]
    public class StatScalingInfo
    {
        // "simple" :: 단리 증가
        // "compound" :: 복리 증가
        public string type;
        public float value;
        public int unitStage;
    }

    [Serializable]
    public class Waves
    {
        public List<MonsterAmountInfo> waves;
    }
    [Serializable]
    public class MonsterAmountInfo
    {
        public int Knight;
        public int Archer;
        public int SpearMan;
        public int Mage;

        public int DarkMage;

        public int Rogue;

        public Dictionary<string, int> ToDictionary()
        {
            return new()
            {
                { "Knight", Knight },
                { "Archer", Archer },
                { "Mage", Mage },
                { "DarkMage", DarkMage },
                { "SpearMan", SpearMan },
                { "Rogue", Rogue },
            };
        }
        public int GetTotalAmount()
        {
            return Knight + Archer + SpearMan + Mage + DarkMage + Rogue;
        }

        public MonsterAmountInfo Add(MonsterAmountInfo other)
        {
            return new()
            {
                Knight = Knight + other.Knight,
                Archer = Archer + other.Archer,
                SpearMan = SpearMan + other.SpearMan,
                Mage = Mage + other.Mage,
                DarkMage = DarkMage + other.DarkMage,
                Rogue = Rogue + other.Rogue
            };
        }
    }
}