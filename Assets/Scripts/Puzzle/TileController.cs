using UnityEngine;

public class TileController : MonoBehaviour
{
    protected PuzzleManager puzzleManager;
    public void SetPuzzleManager(PuzzleManager pzManager)
    {
        puzzleManager = pzManager;
    }
}
