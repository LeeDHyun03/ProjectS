using System;
using System.Collections;
using UnityEngine;

public class PlayerRest : MonoBehaviour
{
    [SerializeField] private float restBenefitInterval = 1f;
    [SerializeField] private float cureHp = 1f;

    public event Action<float> OnHpCure;
    
    private bool currentRestMode = false;
    private float currentTime = 0f;

    public void RestModeChanged(bool isResting)
    {
        if (Time.timeScale < 0.9f && !isResting) return;
        currentTime = 0f;
        currentRestMode = isResting;
        Time.timeScale = currentRestMode ? 2 : 1;
    }

    private void Update()
    {
        if (!currentRestMode)
            return;
        currentTime += Time.deltaTime;
        if (currentTime >= restBenefitInterval)
        {
            currentTime = 0f;
            OnHpCure?.Invoke(cureHp);
        }
    }
}
