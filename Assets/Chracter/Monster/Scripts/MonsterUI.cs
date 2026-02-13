using UnityEngine;
using UnityEngine.UI;

public class MonsterUI : MonoBehaviour
{
    [SerializeField] private Image superArmorBar;
    [SerializeField] private Image hpBar;
    [SerializeField] private Monster monster;

    private void OnEnable()
    {
        monster.OnChangedSuperArmor += SuperArmorBarUpdate;

        monster.OnChangedHp += HpBarUpdate;
    }

    private void OnDisable()
    {
        monster.OnChangedSuperArmor -= SuperArmorBarUpdate;

        monster.OnChangedHp -= HpBarUpdate;
    }

    public void HpBarUpdate(float currentHp, float maxHp)
    {
        if (hpBar == null) return;
        float healthPercent = currentHp / maxHp;
        hpBar.fillAmount = healthPercent;
    }

    public void SuperArmorBarUpdate(float currentSuperArmor, float maxSuperArmor)
    {
        if (superArmorBar == null) return;
        float superArmorPercent = currentSuperArmor / maxSuperArmor;
        superArmorBar.fillAmount = superArmorPercent;
    }
}
