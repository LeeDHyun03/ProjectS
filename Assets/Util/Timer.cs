using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Timer
{
    public float currentTime = 0f;
    public float endTime;
    public bool isRunning = false;
    public int count = 0;
    public bool once = false;

    public delegate void TimerEndAction(int count);
    private TimerEndAction timerEndAction;

    public Timer(float _endTime, bool _once)
    {
        endTime = _endTime;
        once = _once;
    }

    public void Start()
    {
        currentTime = 0;
        count = 0;
        isRunning = true;
    }
    public void Stop()
    {
        isRunning = false;
    }
    public void Resume()
    {
        isRunning = true;
    }
    public void OnEnd(TimerEndAction action)
    {
        timerEndAction = action;
    }
    public void UpdateEndTime(float val)
    {
        endTime = val;
    }
    public void Tick()
    {
        if (isRunning)
        {
            currentTime += Time.deltaTime;

            if (currentTime >= endTime)
            {
                count++;
                timerEndAction(count);
                currentTime = 0;
                if (once) Stop();
                else
                {
                    currentTime = 0;
                }
            }
        }
    }
    public void Skip()
    {
        if (!isRunning) return;
        currentTime = endTime;
    }
}
