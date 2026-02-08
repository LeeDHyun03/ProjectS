using UnityEngine;

public class DarkMageAttack : MageProjectileAttack
{
    [SerializeField] private Monster monster;
    [SerializeField] private string projectileTag;

    [Header("Position Offset")]
    [SerializeField] private float forwardOffset = 1.5f;
    [SerializeField] private float sideOffset = 1.0f;

    private int completedIndicatorCount;

    private readonly Vector3[] positions = new Vector3[3];

    private bool startedAttackBroadcasted;

    // 애니메이션 이벤트 (ReadyAttack)
    public void Spawn()
    {
        if (monster == null || monster.currentTarget == null)
            return;

        completedIndicatorCount = 0;
        startedAttackBroadcasted = false;

        Vector3 myPos = transform.position;
        Vector3 targetPos = monster.currentTarget.transform.position;

        Vector3 forward = (targetPos - myPos).normalized;

        Vector3 side = Vector3.Cross(Vector3.forward, forward).normalized;

        // 중앙/좌/우 위치 계산
        positions[0] = myPos + forward * forwardOffset;
        positions[1] = positions[0] - side * sideOffset;
        positions[2] = positions[0] + side * sideOffset;

        // 인디케이터 생성
        for (int i = 0; i < positions.Length; i++)
        {
            SpawnIndicator(positions[i]);
        }
    }

    private void SpawnIndicator(Vector3 pos)
    {
        Vector2 size = new Vector2(radius * 2f, radius * 2f);
        var indicator = ActivateAttackIndicator(IndicatorShape.Circle, pos, Vector3.zero);

        if (indicator == null)
            return;

        indicator.OnIndicatorComplete += () => OnIndicatorComplete(pos);

        indicator.StartIndicator(size, attackSpeed);
    }

    private void OnIndicatorComplete(Vector3 pos)
    {
        completedIndicatorCount++;

        DoAction(pos);

        int expected = Mathf.Min(actionCount, positions.Length); // 방어
        if (!startedAttackBroadcasted && completedIndicatorCount >= expected)
        {
            startedAttackBroadcasted = true;
            BroadcastOnStartedAttack();
        }
    }

    protected override void DoAction(Vector3 indicatorCenterPos)
    {
        ObjectPooler.Instance.SpawnFromPool(projectileTag, indicatorCenterPos, Quaternion.identity);
    }
}
