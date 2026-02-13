using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class SimplePlayerInput : MonoBehaviour
{
    private PlayerInput playerInput;
    private InputAction moveAction;

    public event Action<Vector2> OnMove;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
    }

    private void Update()
    {
        OnMove?.Invoke(moveAction.ReadValue<Vector2>());
    }
}