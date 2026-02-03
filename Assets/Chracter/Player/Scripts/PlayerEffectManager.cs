using UnityEngine;

public class PlayerEffectManager : MonoBehaviour
{
    public static PlayerEffectManager Instance;

    [SerializeField] private Transform normalAttackEffectTransform;
    [SerializeField] private Transform walkEffectTransform;
    [SerializeField] private Transform specialAttackEffectTransform;

    private void Awake()
    {
        Instance = this;
    }

    public GameObject OnNormalAttackEffect()
    {
        GameObject effect = ObjectPooler.Instance.SpawnFromPool("NormalAttackEffect",
            normalAttackEffectTransform.position, normalAttackEffectTransform.localRotation);
        if (effect == null) return null;
        effect.transform.SetParent(normalAttackEffectTransform, false);
        effect.transform.localPosition = Vector3.zero;
        effect.transform.localScale = new Vector3(1f, 1f, 1f);
        effect.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);

        return effect;
    }

    public GameObject OnSpecialAttackEffect()
    {
        GameObject effect = ObjectPooler.Instance.SpawnFromPool("SpecialAttackEffect",
            specialAttackEffectTransform.position, specialAttackEffectTransform.localRotation);
        if (effect == null) return null;
        return effect;
    }

    public void OnWalkEffect()
    {
        ObjectPooler.Instance.SpawnFromPool("WalkEffect",
            walkEffectTransform.position, walkEffectTransform.localRotation);
    }
}
