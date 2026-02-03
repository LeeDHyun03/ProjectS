using UnityEngine;

[RequireComponent(typeof(Animator))]
public class MonsterSpriteAnimator : MonoBehaviour
{
    [SerializeField]
    private Transform visualRoot;
    private Animator animator => GetComponent<Animator>();

    public void ApplyAnimation(string animName, bool animBoolean)
    {
        animator.SetBool(animName, animBoolean);
    }

    public void ApplyFlip(float dirX)
    {
        Vector3 scale = visualRoot.localScale;
        scale.x = dirX;

        visualRoot.localScale = scale;
    }
}
