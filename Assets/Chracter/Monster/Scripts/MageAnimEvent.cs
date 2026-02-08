using UnityEngine;

public class MageAnimEvent : MonoBehaviour
{
    [SerializeField] private Monster targetPos;
    [SerializeField] private MageAttack mageAttack;

    public void MageReadyAttack()
    {
        var target = GetCurrentTarget();
        if (target == null)
            return;

        mageAttack.SetAttackIndicatorPosition(target.transform.position);
    }

    private Character GetCurrentTarget()
    {
        return targetPos.currentTarget;
    }
}
