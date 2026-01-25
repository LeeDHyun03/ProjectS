using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpecialWeapon : MonoBehaviour
{
    [SerializeField] private BoxCollider specialAttackCollider;

    public event Action OnSpecialAttackComplete;
    
    void Awake()
    {
        specialAttackCollider ??= GetComponent<BoxCollider>();
        specialAttackCollider.enabled = false;
    }

    public void EnableSpecialAttackCollider()
    {
        transform.localScale = new Vector3(1, 1, 1);
        AimToMouse();
        specialAttackCollider.enabled = true;
        StartCoroutine(SpecialAttackTimer(1f));
    }
    
    IEnumerator SpecialAttackTimer(float duration)
    {
        yield return new WaitForSeconds(duration);
        DisableSpecialAttackCollider();
    }

    public void DisableSpecialAttackCollider()
    {
        transform.localScale = new Vector3(0, 0, 0);
        specialAttackCollider.enabled = false;
        OnSpecialAttackComplete?.Invoke();
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
        Quaternion targetWorldRot = Quaternion.Euler(0f, -angle, 0f);
        transform.rotation = targetWorldRot;
    }
}
