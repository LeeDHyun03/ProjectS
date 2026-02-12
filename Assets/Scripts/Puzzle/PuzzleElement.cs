using UnityEngine;

public class PuzzleElement : MonoBehaviour
{
    public PuzzleManager puzzleManager;
    public void SetPuzzleManager(PuzzleManager pzManager)
    {
        puzzleManager = pzManager;
    }
}
