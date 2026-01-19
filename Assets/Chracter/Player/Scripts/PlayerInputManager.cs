using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInputManager : MonoBehaviour
{
    private PlayerInput playerInputAction;

    private InputAction moveAction;
    private InputAction normalAttackAction;
    private InputAction specialAttackAction;
    private InputAction dashAction;
    private InputAction interactAction;

    public event Action<InputAction.CallbackContext> MoveVectorChanged; 
    public event Action<InputAction.CallbackContext> NormalAttackTrigger;
    public event Action<InputAction.CallbackContext> SpecialAttackTrigger;
    public event Action<InputAction.CallbackContext> DashTrigger;
    public event Action<InputAction.CallbackContext> InteractTrigger;

    private void Awake()
    {
        playerInputAction = GetComponent<PlayerInput>();
        moveAction = playerInputAction.actions["Move"];
        normalAttackAction = playerInputAction.actions["NormalAttack"];
        specialAttackAction = playerInputAction.actions["SpecialAttack"];
        dashAction = playerInputAction.actions["Dash"];
        interactAction = playerInputAction.actions["Interaction"];
    }
    private void OnEnable()
    {
        moveAction.performed += OnMove;
        moveAction.canceled += OnMove;
        normalAttackAction.performed += OnNormalAttack;
        specialAttackAction.performed += OnSpecialAttack;
        dashAction.performed += OnDash;
        interactAction.performed += OnInteraction;
    }
    private void OnDisable()
    {
        moveAction.performed -= OnMove;
        moveAction.canceled -= OnMove;
        normalAttackAction.performed -= OnNormalAttack;
        specialAttackAction.performed -= OnSpecialAttack;
        dashAction.performed -= OnDash;
        interactAction.performed -= OnInteraction;
    }

    private void OnMove(InputAction.CallbackContext context) => MoveVectorChanged?.Invoke(context);
    private void OnNormalAttack(InputAction.CallbackContext context) => NormalAttackTrigger?.Invoke(context);
    private void OnSpecialAttack(InputAction.CallbackContext context) => SpecialAttackTrigger?.Invoke(context);
    private void OnDash(InputAction.CallbackContext context) => DashTrigger?.Invoke(context);
    private void OnInteraction(InputAction.CallbackContext context) => InteractTrigger?.Invoke(context);

}
