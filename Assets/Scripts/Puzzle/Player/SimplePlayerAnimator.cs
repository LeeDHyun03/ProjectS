using UnityEngine;

public class SimplePlayerAnimator : MonoBehaviour
{
    [SerializeField] private SimplePlayerInput input;
    [SerializeField] private SimplePlayerMovement movement;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform visualRoot;

    private void OnEnable()
    {
        input.OnMove += movement.SetMoveInput;
    }

    private void OnDisable()
    {
        input.OnMove -= movement.SetMoveInput;
    }

    private void Update()
    {
        if (animator == null) return;

        animator.SetFloat("speed", movement.IsMoving ? 1f : 0f);

        UpdateFlip();
    }

    private void UpdateFlip()
    {
        if (movement.MoveDir.x == 0) return;

        Vector3 scale = visualRoot.localScale;
        scale.x = (movement.MoveDir.x > 0) ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
        visualRoot.localScale = scale;
    }

    public void SpawnWalkEffect() { }
}