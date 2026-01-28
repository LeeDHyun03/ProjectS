using System;
using System.Collections;
using UnityEngine;

public class PlayerRest : MonoBehaviour
{
    public event Action<float> OnHpCure;
    bool currentRestMode = false;
    [SerializeField] private float restBenefitInterval = 1f;

    [SerializeField] private float cureHp = 1f;
    float currentTime = 0f;
    public void RestModeChanged(bool isResting)
    {
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
