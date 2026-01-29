using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Weapon : MonoBehaviour
{
    [SerializeField] private Transform weaponSocket;  
    [SerializeField] private Transform visualRoot;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float spriteAngleOffset = 180f;

    private Camera mainCam;
    private bool isAttacking = false;

    private void Reset()
    {
        weaponSocket = transform;
        visualRoot = transform.parent;
    }

    private void Start()
    {
        mainCam = Camera.main;
        if(mainCam == null)
        {
            mainCam = FindFirstObjectByType<Camera>();
        }
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
        if (mainCam == null) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();

        Vector3 weaponScreenPos = mainCam.WorldToScreenPoint(weaponSocket.position);

        Vector2 dir = mousePos - (Vector2)weaponScreenPos;

        if (dir.sqrMagnitude < 0.1f) return;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        if (visualRoot.lossyScale.x > 0f)
        {
            angle = -angle + 180f;
        }

        Quaternion targetWorldRot = Quaternion.Euler(0f, 0f, angle);
        Quaternion parentWorldRot = visualRoot.rotation;

        Quaternion relativeRot = Quaternion.Inverse(parentWorldRot) * targetWorldRot;

        weaponSocket.localRotation = Quaternion.Euler(0, 0, relativeRot.eulerAngles.z + spriteAngleOffset);
    }
}

