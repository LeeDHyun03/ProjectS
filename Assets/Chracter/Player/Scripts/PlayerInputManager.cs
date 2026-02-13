using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInputManager : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;

    private InputAction moveAction;
    private InputAction normalAttackAction;
    private InputAction specialAttackAction;
    private InputAction dashAction;
    private InputAction interactAction;
    private InputAction restAction;
    private InputAction statusAction;

    public event Action<Vector2> MoveVectorChanged;
    public event Action<Vector2> NormalAttackTriggered;
    public event Action SpecialAttackTriggered;
    public event Action<Vector2> DashTriggered;
    public event Action SprintStarted;
    public event Action SprintEnded;
    public event Action InteractTriggered;
    public event Action<bool> RestTriggered;
    public event Action StatusToggled;

    private float dashPressTime = 0;
    private float sprintHoldTime = 0.5f;
    private bool isSprinting = false;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        moveAction = playerInput.actions["Move"];
        normalAttackAction = playerInput.actions["NormalAttack"];
        specialAttackAction = playerInput.actions["SpecialAttack"];
        dashAction = playerInput.actions["Dash"];
        interactAction = playerInput.actions["Interaction"];
        restAction = playerInput.actions["Rest"];
        statusAction = playerInput.actions["Status"];
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
        restAction.performed += OnRestChanged;
        statusAction.performed += OnStatusToggled;
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
        restAction.performed -= OnRestChanged;
        statusAction.performed -= OnStatusToggled;
    }
    private void OnRestChanged(InputAction.CallbackContext ctx) => RestTriggered?.Invoke(true);
    private void OnStatusToggled(InputAction.CallbackContext ctx) => StatusToggled?.Invoke();
    private void OnMove(InputAction.CallbackContext ctx)
    {
        RestTriggered?.Invoke(false);
        MoveVectorChanged?.Invoke(ctx.ReadValue<Vector2>());
    }

    private void OnNormalAttack(InputAction.CallbackContext ctx)
    {
        RestTriggered?.Invoke(false);
        Vector2 attackDir;
        if (moveAction.IsPressed())
        {
            attackDir = moveAction.ReadValue<Vector2>().normalized;
        }
        else
        {
            attackDir = Vector2.zero;
        }
        NormalAttackTriggered?.Invoke(attackDir);
    }

    private void OnSpecialAttack(InputAction.CallbackContext ctx)
    {
        RestTriggered?.Invoke(false);
        SpecialAttackTriggered?.Invoke();
    }
    private void OnInteract(InputAction.CallbackContext ctx)
    {
        RestTriggered?.Invoke(false);
        InteractTriggered?.Invoke();
    }
    private void OnDashStart(InputAction.CallbackContext ctx)
    {
        RestTriggered?.Invoke(false);
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
