using UnityEngine;

public class PlayerCharacterWiring : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private PlayerInputManager input;
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private PlayerSpriteAnimator animator;
    [SerializeField] private PlayerCombat combat;

    private void Awake()
    {
        input ??= GetComponent<PlayerInputManager>();
        movement ??= GetComponent<PlayerMovement>();
        animator ??= GetComponentInChildren<PlayerSpriteAnimator>();
        combat ??= GetComponent<PlayerCombat>();
    }

    private void OnEnable()
    {
        input.MoveVectorChanged += movement.SetMoveInput;
        input.DashTriggered += movement.OnDash;
        input.SprintStarted += movement.ActivateSprintMode;
        input.SprintEnded += movement.DeactivateSprintMode;
        input.NormalAttackTriggered += combat.OnNormalAttack;
        input.SpecialAttackTriggered += combat.OnSpecialAttack;
    }

    private void OnDisable()
    {
        input.MoveVectorChanged -= movement.SetMoveInput;
        input.DashTriggered -= movement.OnDash;
        input.SprintStarted -= movement.ActivateSprintMode;
        input.SprintEnded -= movement.DeactivateSprintMode;
        input.NormalAttackTriggered -= combat.OnNormalAttack;
        input.SpecialAttackTriggered -= combat.OnSpecialAttack;
    }
}
