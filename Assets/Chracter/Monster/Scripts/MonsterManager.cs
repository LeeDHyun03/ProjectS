using UnityEngine;
using System.Collections.Generic;
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
public enum ENonCombatType  //비전투 행동 유형
{
    Idle,
    Patrol,
    Explore,
    Tracking
} // 대기, 순찰, 탐험, 추적(추적형)

public class MonsterType
{
    public ECombatType combatType;
    public ENonCombatType nonCombatType;

    public string name;
    public float maxHp;
    public float attackRange;
    public float attackDelay;
    public float moveSpeed;
}
public class MonsterManager : MonoBehaviour
{
    public static MonsterManager inst { get; private set; }
    public List<CombatStat> CombatStatList = new List<CombatStat>();
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
