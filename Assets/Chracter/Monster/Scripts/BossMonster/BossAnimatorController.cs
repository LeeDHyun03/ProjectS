using UnityEngine;

public class BossAnimatorController : MonsterSpriteAnimator
{
    [SerializeField] private Animator animator;

    // Trigger hashes
    private static readonly int TrgDashTelegraph = Animator.StringToHash("DashTelegraph");
    private static readonly int TrgDashReady = Animator.StringToHash("DashReady");
    private static readonly int TrgDash = Animator.StringToHash("Dash");
    private static readonly int TrgDashEnded = Animator.StringToHash("DashEnded");
    private static readonly int TrgDashRecover = Animator.StringToHash("DashRecover");

    private static readonly int TrgSwingTelegraph = Animator.StringToHash("SwingTelegraph");
    private static readonly int TrgSwing = Animator.StringToHash("Swing");
    private static readonly int TrgSwingReady = Animator.StringToHash("SwingReady");

    private static readonly int TrgWaveTelegraph = Animator.StringToHash("WaveTelegraph");
    private static readonly int TrgWave = Animator.StringToHash("Wave");

    private static readonly int TrgPinpoint = Animator.StringToHash("Pinpoint");
    private static readonly int TrgMagicZone = Animator.StringToHash("MagicZone");
    private static readonly int TrgSwordThrow = Animator.StringToHash("SwordThrow");

    private static readonly int BoolIsMoving = Animator.StringToHash("IsMoving");

    private void Reset()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public void SetMoving(bool moving)
    {
        if (animator != null) animator.SetBool(BoolIsMoving, moving);
    }

    public void DashTelegraph() { if (animator != null) animator.SetTrigger(TrgDashTelegraph); }
    public void Dash() { if (animator != null) animator.SetTrigger(TrgDash); }
    public void DashReady() { if (animator != null) animator.SetTrigger(TrgDashReady); }
    public void DashEnded() { if (animator != null) animator.SetTrigger(TrgDashEnded); }

    public void DashRecover() { if (animator != null) animator.SetTrigger(TrgDashRecover); }

    public void SwingTelegraph() { if (animator != null) animator.SetTrigger(TrgSwingTelegraph); }
    public void Swing() { if (animator != null) animator.SetTrigger(TrgSwing); }
    public void SwingReady() { if (animator != null) animator.SetTrigger(TrgSwingReady); }

    public void WaveTelegraph() { if (animator != null) animator.SetTrigger(TrgWaveTelegraph); }
    public void Wave() { if (animator != null) animator.SetTrigger(TrgWave); }

    public void Pinpoint() { if (animator != null) animator.SetTrigger(TrgPinpoint); }
    public void MagicZone() { if (animator != null) animator.SetTrigger(TrgMagicZone); }
    public void SwordThrow() { if (animator != null) animator.SetTrigger(TrgSwordThrow); }
}
