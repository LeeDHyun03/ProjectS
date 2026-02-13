using UnityEngine;

public class SimplePlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    private Vector3 moveInput;

    public bool IsMoving => moveInput.sqrMagnitude > 0.01f;
    public Vector3 MoveDir => moveInput;

    private void Update()
    {
        if (IsMoving)
        {
            transform.position += moveInput * moveSpeed * Time.deltaTime;
        }
    }

    public void SetMoveInput(Vector2 input)
    {
        moveInput = new Vector3(input.x, input.y, 0).normalized;
    }
}