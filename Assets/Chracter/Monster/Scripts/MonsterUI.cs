using UnityEngine;
using UnityEngine.UI;

public class MonsterUI : MonoBehaviour
{
    [SerializeField] private Slider hpBar;
    [SerializeField] private Slider superArmorBar;
    [SerializeField] private Monster monster;

    private void OnEnable()
    {
        monster.OnChangedHp += HpBarUpdate;
        monster.OnChangedSuperArmor += SuperArmorBarUpdate;
    }

    private void OnDisable()
    {
        monster.OnChangedHp -= HpBarUpdate;
        monster.OnChangedSuperArmor -= SuperArmorBarUpdate;
    }

    public void HpBarUpdate(float currentHp, float maxHp)
    {
        float healthPercent = currentHp / maxHp;
        hpBar.value = healthPercent;
    }

    public void SuperArmorBarUpdate(float currentSuperArmor, float maxSuperArmor)
    {
        //float superArmorPercent = currentSuperArmor / maxSuperArmor;
        //superArmorBar.value = superArmorPercent;
    }
}
