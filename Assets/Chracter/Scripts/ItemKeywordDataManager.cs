using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public class ItemKeywordDataManager : MonoBehaviour
{
    [Serializable]
    public class KeywordInfo
    {
        public string name;
        public string description;
    }
    private Dictionary<string, KeywordInfo> data = new();

    public static ItemKeywordDataManager Instance;
    public void LoadData()
    {
        string baseJsonPath = Path.Combine(Application.streamingAssetsPath, "ItemKeywords.json");
        if (File.Exists(baseJsonPath))
        {
            string json = File.ReadAllText(baseJsonPath);
            data = JsonConvert.DeserializeObject<Dictionary<string, KeywordInfo>>(json);
        }
    }
    public KeywordInfo GetKeywordInfo(string keyword)
    {
        // Debug.Log(keyword);
        return data[keyword];
    }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadData();
    }
}
