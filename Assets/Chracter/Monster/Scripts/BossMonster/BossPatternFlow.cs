using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPatternFlow : MonoBehaviour
{
    public enum PatternId
    {
        DashCombo,   // "첫 돌진 + 검베기/검기" 포함 세트 (dashRunner.RunDashCombo())
        Pinpoint,    // 운석 7회 (pinpoint.Execute())
        MagicZone,   // 마법 장판 3개
        SwordThrow   // 검 날리기(6검 공전->조준->발사)
    }

    [System.Serializable]
    public class Step
    {
        public PatternId pattern;

        [Tooltip("이 스텝이 끝난 뒤, '다음 스텝이 Dash가 아닐 때' 대기 시간(초).")]
        public float waitIfNextNotDash = 5f;

        [Tooltip("이 스텝이 끝난 뒤, '다음 스텝이 Dash라면' 즉시 처음(DashCombo)로 리셋할지.")]
        public bool resetToDashIfNextIsDash = true;

        [Tooltip("이 스텝이 '나머지 패턴 진행 후 3초 대기' 같은 구간 끝이라면 3초 대기를 넣는다. (단, 다음이 Dash면 리셋 우선)")]
        public bool isGroupEndWait3s = false;
    }

    [Header("Refs")]
    [SerializeField] private BossDashPatternRunner dashRunner;
    [SerializeField] private BossPinpointPattern pinpoint;
    [SerializeField] private BossMagicZonePattern magicZone;
    [SerializeField] private BossSwordThrowPattern swordThrow;
    [SerializeField] private BossMonster bossMonster;

    [Header("Rotation (order)")]
    [Tooltip("0번은 DashCombo로 두는 걸 권장(리셋 기준점).")]
    [SerializeField]
    private List<Step> rotation = new List<Step>()
    {
        new Step { pattern = PatternId.DashCombo, waitIfNextNotDash = 0f, resetToDashIfNextIsDash = false, isGroupEndWait3s = false },

        new Step { pattern = PatternId.Pinpoint,  waitIfNextNotDash = 5f, resetToDashIfNextIsDash = true,  isGroupEndWait3s = false },
        new Step { pattern = PatternId.MagicZone, waitIfNextNotDash = 5f, resetToDashIfNextIsDash = true,  isGroupEndWait3s = false },

        new Step { pattern = PatternId.SwordThrow, waitIfNextNotDash = 5f, resetToDashIfNextIsDash = true, isGroupEndWait3s = true },
    };

    [Header("Global Timing Rules")]
    [SerializeField] private float startDelayOnEngage = 0.2f;

    [Tooltip("DashCombo(첫 돌진+검베기/검기) 끝난 뒤 무조건 3초 대기")]
    [SerializeField] private float waitAfterDashCombo = 1f;

    [Tooltip("공격 패턴 후, 다음이 Dash가 아니면 기본 5초 대기 (Step.waitIfNextNotDash로 개별 조정 가능)")]
    [SerializeField] private float defaultWaitIfNextNotDash = 2f;

    [Tooltip("나머지 패턴 진행 후 3초 대기 구간에서 사용할 3초")]
    [SerializeField] private float waitAfterGroupEnd = 1f;

    private Transform target;
    private bool running;
    private Coroutine flowRoutine;

    public void SetTarget(Transform t) => target = t;
    public void SetStartDelay(float seconds) => startDelayOnEngage = Mathf.Max(0f, seconds);

    private void Awake()
    {
        if (rotation == null || rotation.Count == 0) return;

        if (rotation[0].pattern != PatternId.DashCombo)
        {
            Debug.LogWarning("[BossPatternFlow] rotation[0] should be DashCombo for reset-to-dash logic.");
        }
    }

    public void StartFlow()
    {
        if (flowRoutine != null) StopCoroutine(flowRoutine);
        dashRunner.SetTarget(target);
        running = true;
        Debug.Log("StartFlow");
        flowRoutine = StartCoroutine(CoFlow());
    }

    public void StopFlow()
    {
        running = false;
        if (flowRoutine != null)
        {
            StopCoroutine(flowRoutine);
            flowRoutine = null;
        }
    }

    private IEnumerator CoFlow()
    {
        if (dashRunner == null || rotation == null || rotation.Count == 0)
        {
            flowRoutine = null;
            yield break;
        }

        if (startDelayOnEngage > 0f)
            yield return new WaitForSeconds(startDelayOnEngage);

        int idx = 0;

        while (running && target != null)
        {
            Step cur = rotation[idx];
            Step next = rotation[(idx + 1) % rotation.Count];

            yield return ExecutePattern(cur.pattern);

            if (!running || target == null) break;

            bool nextIsDash = (next.pattern == PatternId.DashCombo);

            if (cur.pattern == PatternId.DashCombo)
            {
                if (waitAfterDashCombo > 0f)
                    yield return new WaitForSeconds(waitAfterDashCombo);

                idx = (idx + 1) % rotation.Count;
                continue;
            }

            if (nextIsDash && cur.resetToDashIfNextIsDash)
            {
                idx = 0;
                continue;
            }

            if (cur.isGroupEndWait3s)
            {
                if (waitAfterGroupEnd > 0f)
                    yield return new WaitForSeconds(waitAfterGroupEnd);

                idx = (idx + 1) % rotation.Count;
                continue;
            }

            float wait = cur.waitIfNextNotDash > 0f ? cur.waitIfNextNotDash : defaultWaitIfNextNotDash;
            if (wait > 0f)
                yield return new WaitForSeconds(wait);

            idx = (idx + 1) % rotation.Count;
        }

        flowRoutine = null;
    }

    private IEnumerator ExecutePattern(PatternId id)
    {
        bossMonster?.SetIsAttacking(true);

        switch (id)
        {
            case PatternId.DashCombo:
                yield return dashRunner.RunDashCombo();
                break;

            case PatternId.Pinpoint:
                if (pinpoint != null && target != null)
                yield return pinpoint.Execute(target);
                break;

            case PatternId.MagicZone:
                if (magicZone != null && target != null)
                yield return magicZone.Execute(target);
                break;

            case PatternId.SwordThrow:
                if (swordThrow != null && target != null)
                yield return swordThrow.Execute(target);
                break;

            default:
                yield break;
        }
    }
}
