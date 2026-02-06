using UnityEngine;

public class SummonedMonster : Monster, IPooledObject
{
    CharacterStateDataContainer.MonsterData summonData;
    void Awake()
    {
        summonData = DataManager.Instance.BaseData.monsters.Find(m => m.monsterID == monsterID);
        if (summonData == null)
        {
            Debug.LogError("Monster ID is Null");
        }
        else
            InitializeMonster(summonData);
    }

    private void OnEnable()
    {
        OnObjectSpawn();
    }

    public void OnObjectSpawn()
    {
        InitializeMonster(summonData);
        isDead = false;
    }

    public override void Dead()
    {   
        ObjectPooler.ReturnToPool(gameObject);
    }
}
