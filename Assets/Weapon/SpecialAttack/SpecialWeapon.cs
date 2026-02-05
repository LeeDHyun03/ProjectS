using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpecialWeapon : MonoBehaviour
{
    [SerializeField] private BoxCollider2D specialAttackCollider;

    public event Action<Vector2> SpecialAttackTriggered;
    public event Action OnSpecialAttackComplete;

    private Vector2 attackDir = Vector2.zero;
    private float attackDamage = 5;
    [SerializeField] private LayerMask targetLayer;

    void Awake()
    {
        if(specialAttackCollider != null )
        {
            specialAttackCollider.enabled = false;
        }
    }

    public void SetAttackDamage(float newDamage)
    {
        attackDamage = newDamage;
    }

    public void EnableSpecialAttackCollider()
    {
        AimToMouse();
        SpecialAttackTriggered?.Invoke(attackDir);

        ObjectPooler.Instance.SpawnFromPool("SpecialAttackEffect", 
            specialAttackCollider.transform.position, specialAttackCollider.transform.rotation);

        StartCoroutine(SpecialAttackTimer(1f));

        Vector2 pos = specialAttackCollider.transform.position;
        Vector2 size = specialAttackCollider.size;
        float angle = specialAttackCollider.transform.eulerAngles.z;

        Collider2D[] hitCharacters = Physics2D.OverlapBoxAll(pos, size, angle, targetLayer);

        foreach (var collision in hitCharacters)
        {
            if (collision.TryGetComponent<Monster>(out Monster monster))
            {
                monster.TakeDamage(attackDamage);
            }
        }
    }
    
    IEnumerator SpecialAttackTimer(float duration)
    {
        yield return new WaitForSeconds(duration);
        SpecialAttackEnded();
    }

    public void SpecialAttackEnded()
    {
        OnSpecialAttackComplete?.Invoke();
    }

    private void AimToMouse()
    {
        Vector2 center = new(Screen.width * 0.5f, Screen.height * 0.5f);

        Vector2 mousePos = Mouse.current != null
            ? Mouse.current.position.ReadValue()
            : center;

        Vector2 dir = mousePos - center;
        attackDir = dir.normalized;
        if (dir.sqrMagnitude < 0.0001f)
            return;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion targetWorldRot = Quaternion.Euler(0f, 0, angle);
        transform.rotation = targetWorldRot;
    }
}
