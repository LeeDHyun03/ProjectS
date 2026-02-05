using UnityEngine;

public class PooledEffect : MonoBehaviour
{
    [SerializeField] private float duration = 1.5f;

    private void OnEnable()
    {
        Invoke(nameof(Deactivate), duration);
    }

    private void Deactivate()
    {
        ObjectPooler.ReturnToPool(gameObject);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }
}