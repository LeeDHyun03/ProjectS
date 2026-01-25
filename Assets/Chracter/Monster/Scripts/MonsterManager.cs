using UnityEngine;
using System.Collections.Generic;

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

    public float pursuitRange;      // 추적 시작 범위
    public float patrolRange;
}
public class MonsterManager : MonoBehaviour
{
    public static MonsterManager inst { get; private set; }
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
        foreach(var type in MonsterTypes)
        {
            Debug.Log(type.name);
        }
    }
    void Start()
    {
    }

    void Update()
    {

    }
    public MonsterType SetMonsterType(int index)
    {
        Debug.Log("spawn"+MonsterTypes[index]);
        return MonsterTypes[index];
    }
}
