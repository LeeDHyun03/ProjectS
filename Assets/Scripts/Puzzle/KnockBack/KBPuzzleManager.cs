using UnityEngine;

public class KBPuzzleManager : PuzzleManager
{
    public KBCart cart;
    public override void Awake()
    {
        base.Awake();

    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }
    public override void PuzzleReset()
    {
        base.PuzzleReset();
        cart.CartReset();
    }
}
