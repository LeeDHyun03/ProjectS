using UnityEngine;

public class PuzzleElement : MonoBehaviour
{
    public PuzzleManager puzzleManager;
    public void SetPuzzleManager(PuzzleManager pzManager)
    {
        puzzleManager = pzManager;
    }
    public virtual void Awake()
    {
        if(puzzleManager == null)
            puzzleManager = FindAnyObjectByType<PuzzleManager>();
    }
}
