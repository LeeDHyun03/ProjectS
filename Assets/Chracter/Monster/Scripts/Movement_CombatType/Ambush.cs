using UnityEngine;

public class Ambush : MonsterMovement
{

    public override void OnHitReaction()
    {
        throw new System.NotImplementedException();
    }
    void Invisibility(bool isEnabled)
    {
        anim.SetBool("Invisibility", isEnabled);
    }
}
