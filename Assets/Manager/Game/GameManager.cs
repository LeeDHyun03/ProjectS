using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

// 게임 상태 전환 (날, 필드 <-> 퍼즐) 및 웨이브 진행 등을 담당
public class GameManager : MonoBehaviour
{
    public enum Phase
    {
        Day,
        Night
    }
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

    private Vector2 lastSpawnCenterPosition;

    private WaveStaticData.GeneralData general;

    public static GameManager Instance;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        string baseJsonPath = Path.Combine(Application.streamingAssetsPath, "WaveData.json");
        if (File.Exists(baseJsonPath))
        {
            string json = File.ReadAllText(baseJsonPath);
            data = JsonConvert.DeserializeObject<WaveStaticData.Root>(json);
            general = data.general;
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }
    IEnumerator Test()
    {
        yield return new WaitForSeconds(5);
        SaveCurrentStateBeforeEnterPuzzle();
        inPuzzle = true;
        SceneManager.LoadScene("P_MineScene");
    }
    // 퍼즐 맵에서 돌아왔을 때 그동안 흘러갔어야 하는 진행 상황을 적용
    public void ResumeFromPuzzle()
    {
        float elapsed = Time.time - lastUpdated;
        float dayDuration = general.waveDuration * general.waveAmountPerDay;
        float totalDuration = dayDuration + general.nightDuration;

        // 스킵된 날들을 전부 제외하고 남은 현재 진행 시간(초)
        float remainingCurrentTime = elapsed % totalDuration;

        int elapsedWave = (int)(elapsed / totalDuration * general.waveDuration * general.waveAmountPerDay);

        if (remainingCurrentTime >= dayDuration)
        {
            currentPhase = Phase.Night;
            currentWaveTime = 0;
            // currentWave = 0;
            currentPhaseTime = remainingCurrentTime - dayDuration;
        }
        else
        {
            currentPhase = Phase.Day;
            currentWaveTime = remainingCurrentTime;
            currentPhaseTime = remainingCurrentTime;
        }

        // 소환되어야 하는 양 계산
        WaveStaticData.MonsterAmountInfo pendingWaveAmountInfo = new();

        for (int i = 1; i < elapsedWave + 1; i++)
        {
            if (currentWave + i > general.waveAmountPerDay)
            {
                currentWave = 1;
                UpdateExtraStagePattern();
            }
            pendingWaveAmountInfo = pendingWaveAmountInfo.Add(
                PickWaveAmountInfo(currentExtraStagePattern, currentStage - 1, currentWave - 1 + i)
            );
        }


        int pendingWaveTotalAmount = pendingWaveAmountInfo.GetTotalAmount();

        // 너무 많이 소환되어야 할 경우 푸아송 샘플링 대신 완전 무작위 지정
        if (pendingWaveTotalAmount >= general.spawnRandomFallbackThreshold)
        {
            MonsterManager.Instance.SpawnWave(
                MonsterManager.SpawnMethod.RandomFallback,
                pendingWaveAmountInfo,
                lastSpawnCenterPosition,
                Mathf.Min(
                    general.maxSpawnAreaExpansionFactor,
                    1 + (pendingWaveTotalAmount - general.spawnRandomFallbackThreshold)
                    * general.spawnAreaAdditionalScaleFactor
                )
            );
        }
        else
        {
            MonsterManager.Instance.SpawnWave(
                MonsterManager.SpawnMethod.PoissonDiscSampling,
                pendingWaveAmountInfo,
                lastSpawnCenterPosition,
                Mathf.Min(
                    general.maxSpawnAreaExpansionFactor,
                    1 + pendingWaveTotalAmount * general.spawnAreaAdditionalScaleFactor
                )
            );
        }
        inPuzzle = false;
    }
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Field")
        {
            ResumeFromPuzzle();
            StartCoroutine(Test());
        }
    }
    // 퍼즐 맵으로 넘어가기 전에 현재 상황 저장
    public void SaveCurrentStateBeforeEnterPuzzle()
    {
        MonsterManager.Instance.Test_ClearMobs();

        lastSpawnCenterPosition = MonsterManager.Instance.GetCurrentSpawnAreaCenter();
        lastUpdated = Time.time;
    }

    WaveStaticData.MonsterAmountInfo PickWaveAmountInfo(int extraStagePattern, int stage, int wave)
    {
        return stage > general.normalStageMax - 1
            ? data.extraStages[extraStagePattern].waves[wave]
            : data.normalStages[stage].waves[wave];
    }
    void UpdateExtraStagePattern()
    {
        if (++currentStage > general.normalStageMax) currentExtraStagePattern++;
        if (currentExtraStagePattern == 3) currentExtraStagePattern = 0;
    }
    public int ElapsedExtraStages
    {
        get { return currentStage - general.normalStageMax; }
    }
    public bool InExtraStage
    {
        get { return currentStage > general.normalStageMax; }
    }
    public WaveStaticData.ScalingInfoGroup CurrentScalingInfoGroup
    {
        get
        {
            return InExtraStage
                ? data.statScaleData.extraStages
                : data.statScaleData.normalStages;
        }
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
            float dayToTwilightDuration = general.waveDuration * general.waveAmountPerDay / 2f;
            float totalDayDuration = general.waveDuration * general.waveAmountPerDay;

            if (currentPhaseTime < dayToTwilightDuration)
            {
                float t = currentPhaseTime / dayToTwilightDuration;
                GlobalLight.Instance.SetColor(Color.Lerp(dayColor, twilightColor, t));
            }
            else
            {
                float t = (currentPhaseTime - dayToTwilightDuration)
                    / (totalDayDuration - dayToTwilightDuration);
                GlobalLight.Instance.SetColor(Color.Lerp(twilightColor, nightColor, t));
            }

            if (currentWaveTime >= general.waveDuration)
            {
                currentWaveTime = 0;
                if (currentWave == 5)
                {
                    currentPhase = Phase.Night;
                    currentPhaseTime = 0;
                    return;
                }
                currentWave++;
                MonsterManager.Instance.SpawnWave(
                    MonsterManager.SpawnMethod.PoissonDiscSampling,
                    PickWaveAmountInfo(currentExtraStagePattern, currentStage - 1, currentWave - 1),
                    MonsterManager.Instance.GetCurrentSpawnAreaCenter()
                );
            }
        }
        if (currentPhase == Phase.Night)
        {
            float t = currentPhaseTime / general.nightDuration;
            GlobalLight.Instance.SetColor(Color.Lerp(nightColor, dayColor, t));

            if (currentPhaseTime >= general.nightDuration)
            {
                currentPhaseTime = 0;
                currentWaveTime = 0;
                currentWave = 0;
                UpdateExtraStagePattern();
                currentPhase = Phase.Day;
            }
        }
    }
}
