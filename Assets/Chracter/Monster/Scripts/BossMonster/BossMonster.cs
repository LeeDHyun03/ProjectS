using System;
using UnityEngine;

public class BossMonster : Monster
{
    [Header("Refs")]
    [SerializeField] private BossPatternFlow patternFlow;
    [SerializeField] private BossAnimatorController animatorController;
    [SerializeField] private BossMovement bossMovement;

    [Header("Engage Settings")]
    [SerializeField] private float startDelayOnEngage = 0.2f;

    [Header("Layers (names)")]
    [SerializeField] private string bossLayerName = "Boss";
    [SerializeField] private string propLayerName = "Prop";

    private bool facingLocked = false;

    [SerializeField] private PlayerUI playerUI;

    private Character target;
    private bool engaged;

    private void Start()
    {
        OnChangedHp += playerUI.BossHpBarUpdate;
        OnObjectSpawn();
    }

    private void Awake()
    {
        if (!patternFlow) patternFlow = GetComponent<BossPatternFlow>();

        // Prop�� �ձ�(��Ÿ�� ������ġ)
        int bossLayer = LayerMask.NameToLayer(bossLayerName);
        int propLayer = LayerMask.NameToLayer(propLayerName);
        if (bossLayer >= 0 && propLayer >= 0)
            Physics2D.IgnoreLayerCollision(bossLayer, propLayer, true);

        if (patternFlow != null)
            patternFlow.SetStartDelay(startDelayOnEngage);
    }

    private void OnEnable()
    {
        if (detection != null)
            detection.OnDetectionStateChanged += HandleDetectionChanged;

        if (bossMovement != null)
        {
            bossMovement.OnAimUpdated += HandleDashAimUpdated;
            bossMovement.OnDashCommitted += HandleDashCommitted;
            bossMovement.OnDashEnd += HandleDashEnd;
        }

        OnChangedHp += playerUI.BossHpBarUpdate;
    }


    private void OnDisable()
    {
        if (detection != null)
            detection.OnDetectionStateChanged -= HandleDetectionChanged;

        if (bossMovement != null)
        {
            bossMovement.OnAimUpdated -= HandleDashAimUpdated;
            bossMovement.OnDashCommitted -= HandleDashCommitted;
            bossMovement.OnDashEnd -= HandleDashEnd;
        }

        OnChangedHp -= playerUI.BossHpBarUpdate;
        StopCombat();
    }

    protected override void InitializeMonster(CharacterStateDataContainer.MonsterData data)
    {
        // Dbg.L("����������", WaveManager.Instance.currentStage);
        // data.Show("��");
        var scaledData = data.TryScale();
        // scaledData.Show("��");

        InitializeCharacter(scaledData.stats);
        detection.SetupDectectionRange(scaledData.attackRange, scaledData.chaseInRange, scaledData.chaseOutRange, scaledData.cognizanceRange); detection.SetIsPlayerSide(false);
        //movement.SetupMovement(moveSpeed);
        //attack.SetAttackStat(attackDamage, normalAttackSpeed);
        //maxSuperArmor = scaledData.maxSuperArmor;
        //currentSuperArmor = maxSuperArmor;
    }

    private void HandleDetectionChanged(MonsterDetection.DetectionState state, Character newTarget)
    {
        target = newTarget;

        bool shouldEngage = (state != MonsterDetection.DetectionState.None && target != null);

        if (shouldEngage && !engaged)
        {
            StartCombat();
        }
        else if (!shouldEngage && engaged)
        {
            StopCombat();
        }
        else if (shouldEngage && engaged)
        {
            if (patternFlow != null && target != null)
                patternFlow.SetTarget(target.transform);
        }
    }

    private void StartCombat()
    {
        engaged = true;

        if (patternFlow != null && target != null)
        {
            patternFlow.SetTarget(target.transform);
            patternFlow.StartFlow();
        }
    }

    private void StopCombat()
    {
        engaged = false;

        if (patternFlow != null)
            patternFlow.StopFlow();
    }

    // �ܺ�(������ ��ȯ/���/�ƽ�)���� ���� ������
    public void ForceStop()
    {
        StopCombat();
    }

    public override void TakeDamage(float damage)
    {

        Debug.Log($"{currentHp} / {maxHp}");
        currentHp -= damage;
        if (currentHp <= 0)
        {
            Dead();
        }
        CallOnChangedHp(currentHp, maxHp);
    }

    public override void Dead()
    {
        CancelInvoke();
        isDead = true;
        PlayerUI.Instance.resultScreen.gameObject.SetActive(true);
    }


    private void Update()
    {
        if (!engaged) return;
        if (target == null) return;
        if (isAttacking) return;
        if (facingLocked) return;

        UpdateFlipByTarget(target.transform.position);
    }

    private void HandleDashAimUpdated(Vector2 dir)
    {
        if (!engaged) return;
        if (facingLocked) return;

        ApplyFlipByDir(dir);
    }

    private void HandleDashCommitted(Vector2 dir)
    {
        ApplyFlipByDir(dir);
        facingLocked = true;
    }

    private void HandleDashEnd()
    {
        facingLocked = false;
    }

    private void UpdateFlipByTarget(Vector3 targetPos)
    {
        Vector2 to = (Vector2)(targetPos - transform.position);
        if (Mathf.Abs(to.x) < 0.001f) return;

        ApplyFlipByDir(to.normalized);
    }

    private void ApplyFlipByDir(Vector2 dir)
    {
        if (Mathf.Abs(dir.x) < 0.001f) return;

        float currentDir = dir.x < 0 ? 1f : -1f;

        if (Mathf.Abs(currentDir - lastDir) > 0.0001f)
        {
            lastDir = currentDir;
            animatorController.ApplyFlip(lastDir);
        }
    }

    public void SetIsAttacking(bool isAttacking) => this.isAttacking = isAttacking;
}
