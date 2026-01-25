using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseFacing : MonoBehaviour
{
    public enum Direction
    {
        DL = 0,
        DR = 1,
        UL = 2,
        UR = 3
    }

    [SerializeField] private float yThresholdPixels = 50f;

    public Direction CurrentDirection { get; private set; } = Direction.DL;
    public event Action<Direction> DirectionChanged;

    private void Update()
    {
        Direction newDir = CalculateDirection();

        if (newDir == CurrentDirection)
            return;

        CurrentDirection = newDir;
        DirectionChanged?.Invoke(newDir);
    }

    private Direction CalculateDirection()
    {
        Vector2 screenCenter = new(Screen.width * 0.5f, Screen.height * 0.5f);
        Vector2 mousePos = Mouse.current != null
            ? Mouse.current.position.ReadValue()
            : screenCenter;

        Vector2 delta = mousePos - screenCenter;

        if (delta.y >= yThresholdPixels)
            return delta.x < 0 ? Direction.UL : Direction.UR;
        else
            return delta.x < 0 ? Direction.DL : Direction.DR;
    }
}
