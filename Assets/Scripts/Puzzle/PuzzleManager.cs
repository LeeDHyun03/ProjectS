using Unity.Cinemachine;
using UnityEngine;

public abstract class PuzzleManager : MonoBehaviour
{
    public Transform player;
    public PZClearTrigger pZClear;
    public PZConfinerTrigger pZConfiner;
    public CinemachineConfiner2D confiner; 
    public GameObject[] puzzleDifficulty = new GameObject[3];
    [SerializeField] protected BoxCollider2D myCon;
    [SerializeField]protected GameObject myMap;
    protected int difficulty;
    public Vector3 startVec, firstVec;
    public virtual void Awake()
    {
        PuzzleDataManager.Instance?.SetCurrentManager(this); 

        SetPuzzleLevel(1);  
    }
    public void GiveReward(int difficulty)
    {

    }
    public void Clear()
    {
        Debug.Log("Clear");
        GiveReward(difficulty);
    }
    public virtual void SetPuzzleLevel(int level)
    {
        difficulty = level;
        myMap = Instantiate(puzzleDifficulty[level], transform);
        firstVec = myMap.transform.Find("FirstVec").position;
        player.position = firstVec;
        startVec = myMap.transform.Find("StartVec").position;
        if (player.TryGetComponent<PZClearTrigger>(out var existingPZ))
        {
            pZClear = existingPZ;
        }
        else if (pZClear == null)
            pZClear = FindAnyObjectByType<PZClearTrigger>();
        if(player.TryGetComponent<PZConfinerTrigger>(out var existingPZC))
        {
            pZConfiner = existingPZC;
        }
        else if (pZConfiner == null)
            pZConfiner = FindAnyObjectByType<PZConfinerTrigger>();
        Debug.Log(level+" start");
        pZClear.OnClear += Clear;
        pZConfiner.OnConfinerTrigger += SetConfiner;
    }
    public virtual void PuzzleReset()
    {
        Destroy(myMap);
        myMap = Instantiate(puzzleDifficulty[difficulty], transform);
        player.position = startVec;
    }
    public virtual void OnEnable()
    {

    }
    public virtual void OnDisable()
    {
        pZClear.OnClear -= Clear;
        pZConfiner.OnConfinerTrigger -= SetConfiner;
    }
    void SetConfiner()
    {
        if (myCon == null)
        {
            myCon = myMap.transform.Find("Confiner").GetComponentInChildren<BoxCollider2D>();
        }
        confiner.BoundingShape2D = myCon;
        myCon.enabled = false;
    }
}
