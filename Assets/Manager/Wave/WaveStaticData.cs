using System;
using System.Collections.Generic;
using UnityEngine;

namespace WaveStaticData
{
    [Serializable]
    public class Root
    {
        public List<Waves> normalStages;
        public List<Waves> extraStages;
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