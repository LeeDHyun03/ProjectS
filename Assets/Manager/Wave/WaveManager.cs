using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class WaveManager : MonoBehaviour
{
    public enum Phase
    {
        Day,
        Night
    }

    [SerializeField] private float waveAmountPerDay = 5;
    [SerializeField] private float waveDuration = 6;

    // 푸아송 샘플링 대신 랜덤 소환 방식을 선택할 최소 몬스터 수
    [SerializeField] private int spawnRandomFallbackThreshold = 40;

    // 한 번에 소환되어야 할 양이 너무 많을 때 소환 범위를 늘리는 정도 

    [SerializeField] private float spawnAreaAdditionalScaleFactor = 0.2f;

    [SerializeField] private float nightDuration = 60 * 7;

    [SerializeField] private Light2D globalLight;

    [SerializeField] private Color dayColor = new(1, 1, 1);

    [SerializeField] private Color twilightColor = new(1f, 0.6f, 0.3f);

    [SerializeField] private Color nightColor = new(0.23f, 0.18f, 0.36f);

    [HideInInspector] public WaveStaticData.Root data;

    [HideInInspector] public Phase currentPhase = Phase.Day;

    [HideInInspector] public int currentStage = 1;

    private int currentWave = 0;

    private float currentWaveTime = 0;
    private float currentPhaseTime = 0;

    private int currentExtraStagePattern = 0;

    private float lastUpdated = 0;

    private bool inPuzzle = false;

    [HideInInspector] public bool initializedByPuzzleTestScene = false;

    public static WaveManager Instance;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Instance = this;

        string baseJsonPath = Path.Combine(Application.streamingAssetsPath, "WaveData.json");
        if (File.Exists(baseJsonPath))
        {
            string json = File.ReadAllText(baseJsonPath);
            data = JsonUtility.FromJson<WaveStaticData.Root>(json);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("start 호출됨");
        if (initializedByPuzzleTestScene) ResumeFromPuzzle();
        else StartCoroutine(Test());
    }
    IEnumerator Test()
    {
        Debug.Log("곧 퍼즐로감");
        yield return new WaitForSeconds(5);
        SaveCurrentStateBeforeEnterPuzzle();
        inPuzzle = true;
        SceneManager.LoadScene("P_MineScene");
    }
    // 퍼즐 맵에서 돌아왔을 때 그동안 흘러갔어야 하는 진행 상황을 적용
    public void ResumeFromPuzzle()
    {
        inPuzzle = false;
        float elapsed = Time.time - lastUpdated;
        float dayDuration = waveDuration * waveAmountPerDay;
        float totalDuration = dayDuration + nightDuration;

        // 스킵된 날들을 전부 제외하고 남은 현재 진행 시간(초)
        float remainingCurrentTime = elapsed % totalDuration;

        int elapsedWave = (int)((elapsed - (nightDuration * (elapsed / totalDuration))) / waveDuration);

        currentStage = (int)(elapsed / totalDuration);
        if (remainingCurrentTime >= dayDuration)
        {
            currentPhase = Phase.Night;
            currentWaveTime = 0;
            currentWave = 0;
            currentPhaseTime = remainingCurrentTime - dayDuration;
        }
        else
        {
            currentPhase = Phase.Day;
            currentWaveTime = remainingCurrentTime;
            currentWave = (int)(remainingCurrentTime / waveDuration);
        }

        // 소환되어야 하는 양 계산
        WaveStaticData.MonsterAmountInfo pendingWaveAmountInfo = new();

        int targetStage = currentStage;
        int targetExtraStagePattern = currentExtraStagePattern;
        for (int i = 1; i < elapsedWave + 1; i++)
        {
            int targetWave = currentWave;
            if (targetWave + i > waveAmountPerDay)
            {
                targetWave = 1;
                if (++targetStage > 5) targetExtraStagePattern++;
                targetStage++;
            }
            pendingWaveAmountInfo = pendingWaveAmountInfo.Add(
                PickWaveAmountInfo(targetExtraStagePattern, targetStage - 1, targetWave - 1 + i)
            );
        }


        int pendingWaveTotalAmount = pendingWaveAmountInfo.GetTotalAmount();
        Debug.Log($"소환할양: {pendingWaveTotalAmount}");

        // 너무 많이 소환되어야 할 경우 푸아송 샘플링 대신 완전 무작위 지정
        if (pendingWaveTotalAmount >= spawnRandomFallbackThreshold)
        {
            Debug.Log("Fallback");
            MonsterSpawner.Instance.SpawnWaveFallback(
                pendingWaveAmountInfo,
                1 + (pendingWaveTotalAmount - spawnRandomFallbackThreshold)
                    * spawnAreaAdditionalScaleFactor
            );
        }
        else
        {
            MonsterSpawner.Instance.SpawnWave(
                pendingWaveAmountInfo,
                1 + pendingWaveTotalAmount * spawnAreaAdditionalScaleFactor
            );
        }
    }
    // 퍼즐 맵으로 넘어가기 전에 현재 상황 저장
    public void SaveCurrentStateBeforeEnterPuzzle()
    {
        lastUpdated = Time.time;
    }
    WaveStaticData.MonsterAmountInfo PickWaveAmountInfo(int extraStagePattern, int stage, int wave)
    {
        return stage > waveAmountPerDay
            ? data.extraStages[extraStagePattern].waves[wave]
            : data.normalStages[stage].waves[wave];
    }
    // Update is called once per frame
    void Update()
    {
        if (inPuzzle) return;

        // 매 웨이브 도달시마다 0으로 초기화됨
        currentWaveTime += Time.deltaTime;

        // 매 낮이 끝날 때, 매 밤이 끝날 때 각각 0으로 초기화됨
        currentPhaseTime += Time.deltaTime;

        if (currentPhase == Phase.Day)
        {
            float dayToTwilightDuration = waveDuration * waveAmountPerDay / 2f;
            float totalDayDuration = waveDuration * waveAmountPerDay;

            if (currentPhaseTime < dayToTwilightDuration)
            {
                float t = currentPhaseTime / dayToTwilightDuration;
                globalLight.color = Color.Lerp(dayColor, twilightColor, t);
            }
            else
            {
                float t = (currentPhaseTime - dayToTwilightDuration)
                    / (totalDayDuration - dayToTwilightDuration);
                globalLight.color = Color.Lerp(twilightColor, nightColor, t);
            }

            if (currentWaveTime >= waveDuration)
            {
                currentWaveTime = 0;
                if (currentWave == 5)
                {
                    currentPhase = Phase.Night;
                    currentPhaseTime = 0;
                    return;
                }
                currentWave++;
                MonsterSpawner.Instance.SpawnWave(
                    PickWaveAmountInfo(currentExtraStagePattern, currentStage - 1, currentWave - 1)
                );
            }
        }
        if (currentPhase == Phase.Night)
        {
            float t = currentPhaseTime / nightDuration;
            globalLight.color = Color.Lerp(nightColor, dayColor, t);

            if (currentPhaseTime >= nightDuration)
            {
                currentPhaseTime = 0;
                currentWaveTime = 0;
                currentWave = 0;
                if (++currentStage > 5) currentExtraStagePattern++;
                currentPhase = Phase.Day;
            }
        }
    }
}
