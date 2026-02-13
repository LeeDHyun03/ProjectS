using System;
using UnityEngine;

public class MSPuzzleManager : PuzzleManager
{
    public MSMineChecking mineChecking;
    public Grid puzzleGrid;
    public override void SetPuzzleLevel(int level)
    {
        base.SetPuzzleLevel(level);
        puzzleGrid = myMap.GetComponent<Grid>();
        mineChecking.SetPuzzleGrid(puzzleGrid);
    }
    void MoveToStartPoint()
    {
        player.position = startVec;
    }
    public override void PuzzleReset()
    {
        base.PuzzleReset();
        puzzleGrid = myMap.GetComponent<Grid>();
        mineChecking.SetPuzzleGrid(puzzleGrid);
    }
    public void OnEnable()
    {
        mineChecking.OnMineTriggered += MoveToStartPoint;
    }
    public override void OnDisable()
    {
        base.OnDisable();
        mineChecking.OnMineTriggered -= MoveToStartPoint;
    }
}
