public class Sniper : MonsterMovement
{
    public override void OnHitReaction()
    {
        throw new System.NotImplementedException();
    }
    public override bool KeepTargeting()
    {
        return currentTarget != null;
    }
}