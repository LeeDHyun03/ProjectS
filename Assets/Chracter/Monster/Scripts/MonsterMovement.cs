using UnityEngine;

public abstract class MonsterMovement : MonoBehaviour
{
    protected MonsterState myStat;
    private void Awake()
    {
        myStat = GetComponent<MonsterState>();
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}