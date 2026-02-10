using System;
using System.IO;
using System.Resources;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    private static DataManager instance;

    public CharacterStateDataContainer.GameDataRoot BaseData;
    public CharacterStateDataContainer.SaveData PlayerSave;

    private string savePath;
    private string baseJsonPath;


    public static DataManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameObject().AddComponent<DataManager>();
            }
            return instance;
        }
    }

    void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "SaveData.json");
        LoadAll();
    }

    public void LoadAll()
    {
        baseJsonPath = Path.Combine(Application.streamingAssetsPath, "GameData.json");
        if (File.Exists(baseJsonPath))
        {
            string json = File.ReadAllText(baseJsonPath);
            BaseData = JsonUtility.FromJson<CharacterStateDataContainer.GameDataRoot>(json);
        }

        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            PlayerSave = JsonUtility.FromJson<CharacterStateDataContainer.SaveData>(json);
        }
        else
        {
            PlayerSave = new CharacterStateDataContainer.SaveData();
        }
    }

    public float GetFinalMaxHp()
            => BaseData.player.stats.maxHp + (PlayerSave.hpLevel * BaseData.player.hpGainPerLevel);

    public float GetFinalAtk()
        => BaseData.player.stats.attackDamage + (PlayerSave.atkLevel * BaseData.player.atkGainPerLevel);

    public float GetFinalMp()
        => BaseData.player.maxMp + (PlayerSave.mpLevel * 10f);

    public float GetFinalDef()
        => BaseData.player.defense + (PlayerSave.defLevel * 1f);

    public float GetFinalMoveSpeed()
        => BaseData.player.stats.moveSpeed + (PlayerSave.spdLevel * 0.1f);

    public float GetFinalAttackSpeed()
        => BaseData.player.stats.attackSpeed + (PlayerSave.atkSpdLevel * 0.1f);

    public float GetFinalCritChance()
        => Mathf.Min(BaseData.player.critChance + (PlayerSave.critChanceLevel * BaseData.player.critChanceGainPerLevel), 0.8f);

    public float GetFinalCritDamage()
        => BaseData.player.critDamage + (PlayerSave.critDamageLevel * 0.05f);

    public int GetFinalRerollCount()
        => BaseData.player.baseRerollCount + PlayerSave.rerollLevel;

    public bool IsElementalStarterAvailable()
        => PlayerSave.isElementalStarterUnlocked;

    public float GetFinalPride()
        => BaseData.player.pride + PlayerSave.prideLevel;

    public float GetFinalAnger()
        => BaseData.player.anger + PlayerSave.angerLevel;

    public float GetFinalJealousy()
        => BaseData.player.jealousy + PlayerSave.jealousyLevel;

    public void Save()
    {
        string json = JsonUtility.ToJson(PlayerSave, true);
        File.WriteAllText(savePath, json);
    }
}