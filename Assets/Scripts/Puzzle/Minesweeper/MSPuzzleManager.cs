using System;
using UnityEngine;

public class MSPuzzleManager : PuzzleManager
{
    public MSMineChecking mineChecking;
    public Grid puzzleGrid;
    public GameObject[] puzzleDifficulty = new GameObject[3];

    [SerializeField]Vector3 startVec;
    public override void Init(int level)
    {
        puzzleDifficulty[level].SetActive(true);
        puzzleGrid = puzzleDifficulty[level].GetComponent<Grid>();
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
    }
    private void OnDisable()
    {
        mineChecking.OnMineTriggered -= MoveToStartPoint;
    }
}
