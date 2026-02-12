using UnityEngine;

public abstract class PuzzleManager : MonoBehaviour
{
    public Transform player;
    public GameObject[] puzzleDifficulty = new GameObject[3];
    [SerializeField] GameObject myMap;
    protected int difficulty;
    public Vector3 startVec, firstVec;
    public virtual void Awake()
    {
        PuzzleDataManager.Instance?.SetCurrentManager(this); 

        SetPuzzleLevel(2);  
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
        myMap = Instantiate(puzzleDifficulty[level]);
        firstVec = myMap.transform.Find("FirstVec").position;
        player.position = firstVec;
        startVec = myMap.transform.Find("StartVec").position;
        Init(level);
        Debug.Log(level+" start");
    }
    public void PuzzleReset()
    {
        Destroy(myMap);
        myMap = Instantiate(puzzleDifficulty[difficulty]);
        player.position = startVec;
    }
    public abstract void Init(int level);
    public virtual void OnEnable()
    {
        if (player.TryGetComponent<PZPlayer>(out PZPlayer pZ))
        {
            pZ.OnClear += Clear;
        }

    }
    public virtual void OnDisable()
    {
        if (player != null)
        {
            if (player.TryGetComponent<PZPlayer>(out PZPlayer pZ))
            {
                pZ.OnClear -= Clear;
            }
        }
    }
}
