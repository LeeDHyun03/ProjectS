using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Weapon : MonoBehaviour
{
    [SerializeField] private Transform weaponSocket;  
    [SerializeField] private Transform visualRoot;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float spriteAngleOffset = 180f;

    private bool isAttacking = false;

    private void Reset()
    {
        weaponSocket = transform;
        visualRoot = transform.parent;
    }

    private void Update()
    {
        if (!isAttacking)
        {
            AimToMouse();
        }
    }

    public void StartAttack()
    {
        weaponSocket.parent = playerTransform;
        isAttacking = true;
    }
    public void EndAttack()
    {
        weaponSocket.parent = visualRoot;
        isAttacking = false;
    }

    private void AimToMouse()
    {
        Vector2 center = new(Screen.width * 0.5f, Screen.height * 0.5f);

        Vector2 mousePos = Mouse.current != null
            ? Mouse.current.position.ReadValue()
            : center;

        Vector2 dir = mousePos - center;
        if (dir.sqrMagnitude < 0.0001f)
            return;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        angle += spriteAngleOffset;
        
        bool flipped = visualRoot.lossyScale.x > 0f;
        if (flipped)
        {
            angle = -angle;
        }

        Quaternion targetWorldRot = Quaternion.Euler(0f, 0f, angle);

        Quaternion parentWorldRot = visualRoot.rotation;
        Quaternion inverseQuat = Quaternion.Inverse(parentWorldRot) * targetWorldRot;
        inverseQuat.x = 0f;
        inverseQuat.y = 0f;
        weaponSocket.localRotation = inverseQuat;
    }
}
