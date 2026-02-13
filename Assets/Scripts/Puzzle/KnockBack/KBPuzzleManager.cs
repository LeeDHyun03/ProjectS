using UnityEngine;

public class KBPuzzleManager : PuzzleManager
{
    public KBCart cart;
    public override void PuzzleReset()
    {
        base.PuzzleReset();
        cart.CartReset();
    }
}
