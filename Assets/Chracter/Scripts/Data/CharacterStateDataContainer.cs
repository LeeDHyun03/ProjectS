using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStateDataContainer : MonoBehaviour
{
    [Serializable]
    public class GameDataRoot
    {
        public PlayerData player;
        public List<MonsterData> monsters;
    }

    [Serializable]
    public struct CommonStats
    {
        public float maxHp;
        public float attackDamage;
        public float attackSpeed;
        public float moveSpeed;
    }

    [Serializable]
    public class MonsterData
    {
        public string monsterID;
        public CommonStats stats;
        public float maxSuperArmor;
        public float attackRange;
        public float chaseInRange;
        public float chaseOutRange;
        public float cognizanceRange;
    }

    [Serializable]
    public class PlayerData
    {
        public string playerID;
        public CommonStats stats;

        public float maxMp;
        public float defense;
        public float critChance;
        public float critDamage;

        public float sprintSpeed;
        public float specialAttackUsedMp;
        public float specialAttackSpeed;

        public float baseMaxExp;
        public float expIncrement;
        public int baseRerollCount;

        public float hpGainPerLevel = 10f;
        public float atkGainPerLevel = 2f;
        public float critChanceGainPerLevel = 0.01f;

        public int baseUpgradeCost = 100;
        public float costMultiplier = 1.2f;

        public int maxCritLevel = 10;
        public int maxAtkLevel = 50;

        public int elementalUnlockCost = 5000;

        public float pride = 3;
        public float anger = 3;
        public float jealousy = 3;
    }

    [Serializable]
    public class SaveData
    {
        public int totalPoints;

        public int hpLevel;
        public int atkLevel;
        public int atkSpdLevel;
        public int mpLevel;
        public int defLevel;
        public int spdLevel;
        public int critChanceLevel;
        public int critDamageLevel;
        public int rerollLevel;
        public int prideLevel;
        public int jealousyLevel;
        public int angerLevel;
        public bool isElementalStarterUnlocked;
    }
}