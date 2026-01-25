using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInputManager : MonoBehaviour
{
    private PlayerInput playerInput;

    private InputAction moveAction;
    private InputAction normalAttackAction;
    private InputAction specialAttackAction;
    private InputAction dashAction;
    private InputAction interactAction;

    public event Action<Vector2> MoveVectorChanged;
    public event Action NormalAttackTriggered;
    public event Action SpecialAttackTriggered;
    public event Action<Vector2> DashTriggered;
    public event Action SprintStarted;
    public event Action SprintEnded;
    public event Action InteractTriggered;

    private float dashPressTime = 0;
    private bool isSprinting = false;
    private float sprintHoldTime;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        moveAction = playerInput.actions["Move"];
        normalAttackAction = playerInput.actions["NormalAttack"];
        specialAttackAction = playerInput.actions["SpecialAttack"];
        dashAction = playerInput.actions["Dash"];
        interactAction = playerInput.actions["Interaction"];
    }

    private void OnEnable()
    {
        moveAction.performed += OnMove;
        moveAction.canceled += OnMove;

        normalAttackAction.performed += OnNormalAttack;
        specialAttackAction.performed += OnSpecialAttack;
        dashAction.started += OnDashStart;
        dashAction.canceled += OnDashEnd;
        interactAction.performed += OnInteract;
    }

    private void OnDisable()
    {
        moveAction.performed -= OnMove;
        moveAction.canceled -= OnMove;

        normalAttackAction.performed -= OnNormalAttack;
        specialAttackAction.performed -= OnSpecialAttack;
        dashAction.started -= OnDashStart;
        dashAction.canceled -= OnDashEnd;
        interactAction.performed -= OnInteract;
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        MoveVectorChanged?.Invoke(ctx.ReadValue<Vector2>());
    }

    private void OnNormalAttack(InputAction.CallbackContext ctx) => NormalAttackTriggered?.Invoke();
    private void OnSpecialAttack(InputAction.CallbackContext ctx) => SpecialAttackTriggered?.Invoke();
    private void OnInteract(InputAction.CallbackContext ctx) => InteractTriggered?.Invoke();
    private void OnDashStart(InputAction.CallbackContext ctx)
    {
        Vector2 dashDir;
        if (moveAction.IsPressed())
        {
            dashDir = moveAction.ReadValue<Vector2>().normalized;
        }
        else
        {
            dashDir = GetMouseDirectionFromCenter();
        }
         
        DashTriggered?.Invoke(dashDir);
        dashPressTime = Time.time;
        isSprinting = false;
    }
    private void OnDashEnd(InputAction.CallbackContext ctx)
    {
        float heldTime = Time.time - dashPressTime;

        if (heldTime > sprintHoldTime)
        {
            SprintEnded?.Invoke();
        }
    }

    void Update()
    {
        if (dashAction.IsPressed() && !isSprinting)
        {
            float heldTime = Time.time - dashPressTime;
            if (heldTime > sprintHoldTime)
            {
                isSprinting = true;
                SprintStarted?.Invoke();
            }
        }
    }
    private Vector2 GetMouseDirectionFromCenter()
    {
        Vector2 center = new(Screen.width * 0.5f, Screen.height * 0.5f);

        Vector2 mousePos = Mouse.current != null
            ? Mouse.current.position.ReadValue()
            : center;

        Vector2 delta = mousePos - center;

        if (delta.sqrMagnitude < 0.0001f)
            return Vector2.zero;

        return delta.normalized;
    }
}
