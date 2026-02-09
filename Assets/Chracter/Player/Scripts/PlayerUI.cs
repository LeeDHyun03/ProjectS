using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public static PlayerUI Instance;
    
    [SerializeField] private Image statusPanel;
    [SerializeField] private Image hpBar;
    [SerializeField] private Image mpBar;
    [SerializeField] private Image expBar;

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

    public void HpBarUpdate(float currentHp, float maxHp)
    {
        float healthPercent = currentHp / maxHp;
        hpBar.fillAmount = healthPercent;
    }
    public void MpBarUpdate(float currentMp, float maxMp)
    {
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
}
