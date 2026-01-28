using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private Image statusPanel;
    [SerializeField] private Slider hpBar;
    [SerializeField] private Slider mpBar;
    [SerializeField] private Slider expBar;

    private bool isStatusVisible = false;
    void Start()
    {

    }

    void Update()
    {

    }

    public void HpBarUpdate(float currentHp, float maxHp)
    {
        float healthPercent = currentHp / maxHp;
        hpBar.value = healthPercent;
    }
    public void MpBarUpdate(float currentMp, float maxMp)
    {
        float mpPercent = currentMp / maxMp;
        mpBar.value = mpPercent;
    }
    public void ExpBarUpdate(float currentExp, float maxExp)
    {
        float expPercent = currentExp / maxExp;
        expBar.value = expPercent;
    }

    public void ToggleStatusDisplay()
    {
        isStatusVisible = !isStatusVisible;
        statusPanel.gameObject.SetActive(isStatusVisible);
    }
}
