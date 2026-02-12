using System;
using UnityEngine;

public class MSPuzzleManager : PuzzleManager
{
    public MSMineChecking mineChecking;
    public Grid puzzleGrid;

    public override void Init(int level)
    {
        puzzleDifficulty[level].SetActive(true);
        puzzleGrid = puzzleDifficulty[level].GetComponent<Grid>();
        mineChecking.SetPuzzleGrid(puzzleGrid);
        //var startCellVec = puzzleGrid.WorldToCell(player.position);
        //startVec = puzzleGrid.GetCellCenterWorld(startCellVec);
    }
    void MoveToStartPoint()
    {
        player.position = startVec;
    }
    public override void OnEnable()
    {
        base.OnEnable();
        mineChecking.OnMineTriggered += MoveToStartPoint;
    }
    public override void OnDisable()
    {
        base.OnDisable();
        mineChecking.OnMineTriggered -= MoveToStartPoint;
    }
}
