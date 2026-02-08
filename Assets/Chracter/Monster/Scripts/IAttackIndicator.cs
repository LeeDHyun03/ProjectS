using System;
using UnityEngine;

public interface IAttackIndicator
{
    event Action OnIndicatorComplete;
    void StartIndicator(Vector2 size, float duration);
    
}
