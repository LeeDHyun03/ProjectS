using UnityEngine;

public static class Dbg
{
    private static readonly bool inDevMode = true;
    public static void L(string label, dynamic value)
    {
        if (inDevMode) Debug.Log($"{label}: {value}");
    }
    public static void L(string value)
    {
        if (inDevMode) Debug.Log(value);
    }
    public static void Show(
        this CharacterStateDataContainer.MonsterData data,
        string tag = ""
    )
    {
        if (!inDevMode) return;

        Debug.Log($"---- {tag} ----");
        L("체력", data.stats.maxHp);
        L("공격력", data.stats.attackDamage);
        L("슈퍼아머", data.maxSuperArmor);
    }
}
