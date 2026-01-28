using UnityEngine;

[RequireComponent(typeof(PlayerInputManager))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float maxDashTimer = 0.3f;

    private float currentSpeed;
    private float currentDashTimer = 0f;

    private bool isDashing = false;

    private Vector3 moveDir;
    private Vector3 dashDir;

    public bool IsMoving => moveDir.sqrMagnitude > 0.0001f;
    public bool IsSprinting => currentSpeed == sprintSpeed;
    private void Awake()
    {
        currentSpeed = moveSpeed;
    }

    private void Update()
    {
        if (IsMoving)
        {
            transform.position += moveDir * currentSpeed * Time.deltaTime;
        }

        if(isDashing)
        {
            float dashDistance = moveSpeed * 2f * Time.deltaTime;
            transform.position += dashDir.normalized * dashDistance;
            currentDashTimer += Time.deltaTime;
            if(currentDashTimer >= maxDashTimer)
            {
                isDashing = false;
                currentDashTimer = 0f;
            }
        }
    }

    public void SetMoveInput(Vector2 input)
    {
        moveDir = new Vector3(input.x, input.y, 0f);
        moveDir = Vector3.ClampMagnitude(moveDir, 1f);
    }

    public void ActivateSprintMode()
    {
        currentSpeed = sprintSpeed;
    }

    public void DeactivateSprintMode()
    {
        currentSpeed = moveSpeed;
    }

    public void OnDash(Vector2 dashDirection)
    {
        if(isDashing)
            return;
        dashDir = new Vector3(dashDirection.x, dashDirection.y, 0);
        isDashing = true;
    }

    public void SetSpeed(float newMoveSpeed, float newSprintSpeed)
    {
        moveSpeed = newMoveSpeed;
        sprintSpeed = newSprintSpeed;
    }
    
    public Vector3 GetMoveDir() => moveDir;
}
