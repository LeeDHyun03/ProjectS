using NUnit.Framework;
using Roguelike.Items;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public static PlayerUI Instance;

    public ItemTooltip itemTooltip;
    public ItemKeywordTooltip itemKeywordTooltip;

    public ResultScreen resultScreen;

    [SerializeField] private Image statusPanel;
    [SerializeField] private Image hpBar;
    [SerializeField] private Image mpBar;
    [SerializeField] private Image expBar;
    [SerializeField] private TMP_Text hpBarIndex;
    [SerializeField] private TMP_Text mpBarIndex;
    [SerializeField] private TMP_Text maxHpIndex;
    [SerializeField] private TMP_Text maxMpIndex;
    [SerializeField] private TMP_Text speedIndex;
    [SerializeField] private TMP_Text atkIndex;
    [SerializeField] private TMP_Text critChance;
    [SerializeField] private TMP_Text defIndex;
    [SerializeField] private TMP_Text angerIndex;
    [SerializeField] private TMP_Text prideIndex;
    [SerializeField] private TMP_Text jealousyIndex;
    [SerializeField] private TMP_Text rerollIndex;
    [SerializeField] private List<Image> dashGageBars;
    [SerializeField] private TMP_Text currentMonsterCount;
    [SerializeField] private Image watchHand;
    [SerializeField] private Image watchNight;
    [SerializeField] private RewardScreen rewardScreen;
    [SerializeField] private Image bossHpBar;
    [SerializeField] private Image bossHpFillBar;

    private bool isStatusVisible = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }


    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnCurrentPhaseTimeChanged += ChagnedCurrentPhaseTime;
        }
    }

    private void OnEnable()
    {
        MonsterManager.Instance.OnChangedAliveMonsterCount += ChangedMonsterCount;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnCurrentPhaseTimeChanged += ChagnedCurrentPhaseTime;
        }
    }

    private void OnDisable()
    {
        MonsterManager.Instance.OnChangedAliveMonsterCount -= ChangedMonsterCount;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnCurrentPhaseTimeChanged -= ChagnedCurrentPhaseTime;
        }
    }

    public void ActivateRewardScreen()
    {
        rewardScreen.gameObject.SetActive(true);
        List<string> itemIds = ItemDataManager.Instance.availableItemIds;
        rewardScreen.SetRewards(RewardManager.PickItemsFromID(itemIds, 3));
    }

    public void DeactivateRewardScreen()
    {
        rewardScreen.gameObject.SetActive(false);
    }

    public void ActivateBossHpBar()
    {
        bossHpBar.gameObject.SetActive(true);
    }

    public RewardScreen GetRewardScreen() => rewardScreen;

    public void ChangedMonsterCount(int currentMonsterCount)
    {
        this.currentMonsterCount.text = $"{currentMonsterCount}마리";
    }

    public void StatsUpdate(
        float maxHpIndex, float maxMpIndex, float speedIndex, float atkIndex, float critChance,
        float defIndex, float angerIndex, float prideIndex, float jealousyIndex, float rerollIndex)
    {
        this.maxHpIndex.text = $"{maxHpIndex}";
        this.maxMpIndex.text = $"{maxMpIndex}";
        this.speedIndex.text = $"{speedIndex}";
        this.atkIndex.text = $"{atkIndex}";
        this.critChance.text = $"{critChance * 100}%";
        this.defIndex.text = $"{defIndex}";
        this.angerIndex.text = $"{angerIndex}";
        this.prideIndex.text = $"{prideIndex}";
        this.jealousyIndex.text = $"{jealousyIndex}";
        this.rerollIndex.text = $"{rerollIndex}";
    }

    public void HpBarUpdate(float currentHp, float maxHp)
    {
        hpBarIndex.text = $"{currentHp} / {maxHp}";
        float healthPercent = currentHp / maxHp;
        if (healthPercent <= 0)
            hpBar.fillAmount = 0;
        else
            hpBar.fillAmount = healthPercent;
    }

    public void MpBarUpdate(float currentMp, float maxMp)
    {
        mpBarIndex.text = $"{currentMp} / {maxMp}";
        float mpPercent = currentMp / maxMp;
        mpBar.fillAmount = mpPercent;
    }

    public void ExpBarUpdate(float currentExp, float maxExp)
    {
        float expPercent = currentExp / maxExp;
        expBar.fillAmount = expPercent;
    }

    public void ToggleStatusDisplay()
    {
        isStatusVisible = !isStatusVisible;
        statusPanel.gameObject.SetActive(isStatusVisible);
    }

    public void BossHpBarUpdate(float currentHp, float maxHp)
    {
        float hpBarPercent = currentHp / maxHp;
        bossHpFillBar.fillAmount = hpBarPercent;
    }

    public void ChagnedDashGageBar(bool isCharged, float fillAmount)
    {
        if (isCharged)
        {
            for (int i = 0; i < dashGageBars.Count; i++)
            {
                if (dashGageBars[i].fillAmount < 1)
                {
                    dashGageBars[i].fillAmount = fillAmount;
                    return;
                }
            }
        }
        else
        {

            for (int i = dashGageBars.Count - 1; i >= 0; i--)
            {
                if (dashGageBars[i].fillAmount == 1)
                {
                    dashGageBars[i].fillAmount = 0;
                    return;
                }
                if (dashGageBars[i].fillAmount > 0)
                {
                    dashGageBars[i - 1].fillAmount = dashGageBars[i].fillAmount;
                    dashGageBars[i].fillAmount = 0;
                    return;
                }
            }
        }
    }

    private void ChagnedCurrentPhaseTime(float currentTime, float maxTime)
    {
        float angle = (currentTime / maxTime) * 360f;
        watchHand.transform.localRotation = Quaternion.Euler(0, 0, -angle);
        if(GameManager.Instance.currentPhase == GameManager.Phase.Day)
            watchNight.gameObject.SetActive(false);
        else watchNight.gameObject.SetActive(true);
    }
}
