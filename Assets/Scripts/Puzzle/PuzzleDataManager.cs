using System.Collections.Generic;
using UnityEngine;
public enum Puzzle
{
    ObjectActivation,
    Portal,
    Rhythm,
    Minesweeper,
    KnockBack
}

public class PuzzleDataManager : MonoBehaviour
{
    public static PuzzleDataManager Instance { get; set; }
    int currentLevel;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        
    }
    public void SetCurrentManager(PuzzleManager puzzle)
    {
        puzzle.SetPuzzleLevel(currentLevel);
    }
    public void SetCurrentLevel(int level)
    {
        currentLevel = level;
    }
}
