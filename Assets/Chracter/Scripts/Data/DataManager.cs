using System;
using System.IO;
using System.Resources;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    // 데이터 저장용 변수
    public CharacterStateDataContainer.GameDataRoot BaseData;
    public CharacterStateDataContainer.SaveData PlayerSave;

    private string savePath;
    private string baseJsonPath;

    void Awake()
    {
        Instance = this;
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

    public float GetFinalCritChance()
        => Mathf.Min(BaseData.player.critChance + (PlayerSave.critChanceLevel * BaseData.player.critChanceGainPerLevel), 0.8f); // 최대 80% 제한 예시

    public float GetFinalCritDamage()
        => BaseData.player.critDamage + (PlayerSave.critDamageLevel * 0.05f);

    public int GetFinalRerollCount()
        => BaseData.player.baseRerollCount + PlayerSave.rerollLevel;

    public bool IsElementalStarterAvailable()
        => PlayerSave.isElementalStarterUnlocked;

    public void Save()
    {
        string json = JsonUtility.ToJson(PlayerSave, true);
        File.WriteAllText(savePath, json);
    }
}