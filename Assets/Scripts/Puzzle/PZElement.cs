using UnityEngine;

public class PZElement : MonoBehaviour
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
