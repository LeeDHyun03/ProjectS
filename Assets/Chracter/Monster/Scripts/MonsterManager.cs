using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public struct CombatStat
{
    public float detectionRange;    // 인식 범위
    public float pursuitRange;      // 추적 시작 범위
}
public enum ECombatType     //전투 유형
{
    Chase,
    Guard,
    Sniper,
    Ambush
} // 추격, 경계, 저격, 기습

[System.Serializable]
public class MonsterType : EntityType
{
    public ECombatType combatType;

    public bool isPatrol;

    public string name;
}
public class MonsterManager : MonoBehaviour
{
    public static MonsterManager inst { get; private set; }
    public List<CombatStat> CombatStatList = new List<CombatStat>();
    public List<MonsterType> MonsterTypes = new List<MonsterType>();
    private void Awake()
    {
        if (inst != null)
        {
            Destroy(gameObject);
            return;
        }
        inst = this;
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {

    }

    void Update()
    {

    }
}
