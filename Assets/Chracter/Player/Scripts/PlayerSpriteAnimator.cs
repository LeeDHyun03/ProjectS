using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerSpriteAnimator : MonoBehaviour
{
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private MouseFacing facing;
    [SerializeField] private Transform visualRoot;
    [SerializeField] private Transform weaponRoot;
    [SerializeField] private Vector3 weaponPosForDown;
    [SerializeField] private Vector3 weaponPosForSide;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        ApplyAnimation();
    }

    private void ApplyAnimation()
    {
        animator.SetFloat("speed", movement.IsMoving ? 1f : 0f);
        animator.SetInteger("direction", (int)facing.CurrentDirection);
        animator.SetBool("isSprint", movement.IsSprinting ? true : false);
        ApplyFlip(facing.CurrentDirection);
    }

    private void ApplyFlip(MouseFacing.Direction dir)
    {
        Vector3 scale = visualRoot.localScale;

        if (dir == MouseFacing.Direction.UL || dir == MouseFacing.Direction.DL)
            scale.x = -Mathf.Abs(scale.x);
        else if (dir == MouseFacing.Direction.UR || dir == MouseFacing.Direction.DR)
            scale.x = Mathf.Abs(scale.x);
        
        visualRoot.localScale = scale;
    }

    public void ToggledRestAnimation(bool isRest)
    {
        animator.SetBool("isRest", isRest);
    }
}
