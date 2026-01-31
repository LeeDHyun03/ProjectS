using System;
using UnityEngine;

public class MSPuzzleManager : PuzzleManager
{
    public MSMineChecking mineChecking;
    public Grid puzzleGrid;
    public Transform player;
    public GameObject[] puzzleDifficulty = new GameObject[3];

    [SerializeField]Vector3 startVec;
    private void Awake()
    {
        Init(0);
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }
    public override void Init(int level)
    {
        puzzleDifficulty[difficulty].SetActive(true);
        puzzleGrid = puzzleDifficulty[difficulty].GetComponent<Grid>();
        mineChecking.SetPuzzleGrid(puzzleGrid);
        var startCellVec = puzzleGrid.WorldToCell(player.position);
        startVec = puzzleGrid.GetCellCenterWorld(startCellVec);
    }
    void MoveToStartPoint()
    {
        player.position = startVec;
    }
    private void OnEnable()
    {
        mineChecking.OnMineTriggered += MoveToStartPoint;
        mineChecking.OnClear += Clear;
    }
    private void OnDisable()
    {
        mineChecking.OnMineTriggered -= MoveToStartPoint;
        mineChecking.OnClear -= Clear;
    }
}
