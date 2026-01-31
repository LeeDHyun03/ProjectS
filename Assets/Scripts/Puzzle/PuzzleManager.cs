using UnityEngine;

public abstract class PuzzleManager : MonoBehaviour
{
    protected int difficulty;
    private void Awake()
    {
        PuzzleDataManager.Instance?.SetCurrentManager(this);
    }
    void Start()
    {
        SetPuzzleLevel(0);
    }

    void Update()
    {
        
    }
    public void GiveReward(int difficulty)
    {

    }
    public void Clear()
    {
        Debug.Log("Clear");
        GiveReward(difficulty);
    }
    public void SetPuzzleLevel(int level)
    {
        difficulty = level;
        Init(level);
    }
    public abstract void Init(int level);
}
