using UnityEngine;

public class PlayerEffectManager : MonoBehaviour
{
    public static PlayerEffectManager Instance;

    [SerializeField] private Transform attackEffectTransform;
    [SerializeField] private Transform walkEffectTransform;

    private void Awake()
    {
        Instance = this;
    }

    public GameObject OnAttackEffect()
    {
        GameObject effect = ObjectPooler.Instance.SpawnFromPool("NormalAttackEffect",
            attackEffectTransform.position, attackEffectTransform.localRotation);
        if (effect == null) return null;
        effect.transform.SetParent(attackEffectTransform, false);
        effect.transform.localPosition = Vector3.zero;
        effect.transform.localScale = new Vector3(1f, 1f, 1f);
        effect.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);

        return effect;
    }

    public void OnWalkEffect()
    {
        ObjectPooler.Instance.SpawnFromPool("WalkEffect",
            walkEffectTransform.position, walkEffectTransform.localRotation);
    }
}
