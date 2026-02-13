using System;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1000)]
public class SfxManager : MonoBehaviour
{
    public static SfxManager Instance { get; private set; }

    [Serializable]
    public struct NamedClip
    {
        public string Name;
        public AudioClip Clip;
    }

    [Serializable]
    public class SfxGroups
    {
        public List<NamedClip> Character_Sfx = new();
        public List<NamedClip> Monster_Sfx = new();
        public List<NamedClip> Puzzle_Sfx = new();
        public List<NamedClip> Item_Sfx = new();
        public List<NamedClip> UI_Sfx = new();
    }

    [Header("SFX")]
    [SerializeField] private SfxGroups SFXS = new();
    [SerializeField, Min(1)] private int channels = 25;

    [Header("Volume")]
    [SerializeField, Range(0f, 1f)] private float masterVolume = 1f;
    [SerializeField, Range(0f, 1f)] private float sfxVolume = 1f;

    private AudioSource[] sfxPlayers;
    private int channelIndex;

    private Dictionary<string, AudioClip> sfxMap;

    public float GetSfxVolume() => sfxVolume;
    public float GetMasterVolume() => masterVolume;

    private const string PREF_MASTER = "MasterVolume";
    private const string PREF_SFX = "SfxVolume";

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        BuildMap();
        InitPlayers();
        LoadVolumes();
    }

    private void BuildMap()
    {
        sfxMap = new Dictionary<string, AudioClip>(256, StringComparer.Ordinal);

        MergeInto(sfxMap, SFXS?.Character_Sfx, "SFX/Character");
        MergeInto(sfxMap, SFXS?.Monster_Sfx, "SFX/Monster");
        MergeInto(sfxMap, SFXS?.Puzzle_Sfx, "SFX/Puzzle");
        MergeInto(sfxMap, SFXS?.Item_Sfx, "SFX/Item");
        MergeInto(sfxMap, SFXS?.UI_Sfx, "SFX/UI");
    }

    private void MergeInto(Dictionary<string, AudioClip> map, List<NamedClip> list, string label)
    {
        if (list == null) return;

        foreach (var e in list)
        {
            if (string.IsNullOrWhiteSpace(e.Name) || e.Clip == null) continue;

            if (map.ContainsKey(e.Name))
            {
                Debug.LogWarning($"[SfxManager] Duplicate key '{e.Name}' in {label}. (first one kept)");
                continue;
            }
            map.Add(e.Name, e.Clip);
        }
    }

    private void InitPlayers()
    {
        var obj = new GameObject("SfxPlayers");
        obj.transform.SetParent(transform);

        int count = Mathf.Max(1, channels);
        sfxPlayers = new AudioSource[count];

        for (int i = 0; i < count; i++)
        {
            var src = obj.AddComponent<AudioSource>();
            src.playOnAwake = false;
            src.loop = false;
            sfxPlayers[i] = src;
        }

        ApplyVolumes();
    }

    private void LoadVolumes()
    {
        masterVolume = PlayerPrefs.GetFloat(PREF_MASTER, masterVolume);
        sfxVolume = PlayerPrefs.GetFloat(PREF_SFX, sfxVolume);
        ApplyVolumes();
        Debug.Log($"[SfxManager] master={masterVolume}, sfx={sfxVolume}, final={masterVolume * sfxVolume}");

    }

    private void ApplyVolumes()
    {
        if (sfxPlayers == null) return;

        float final = masterVolume * sfxVolume;
        for (int i = 0; i < sfxPlayers.Length; i++)
            sfxPlayers[i].volume = final;
    }

    public void ChangeMasterVolume(float value)
    {
        masterVolume = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat(PREF_MASTER, masterVolume);
        ApplyVolumes();
    }

    public void ChangeSfxVolume(float value)
    {
        sfxVolume = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat(PREF_SFX, sfxVolume);
        ApplyVolumes();
    }

    public void Play(string name)
    {
        if (sfxPlayers == null || sfxMap == null) return;

        if (!sfxMap.TryGetValue(name, out var clip) || clip == null)
        {
            Debug.LogWarning($"[SfxManager] SFX not found: '{name}'");
            return;
        }

        for (int i = 0; i < sfxPlayers.Length; i++)
        {
            int idx = (channelIndex + i) % sfxPlayers.Length;
            if (sfxPlayers[idx].isPlaying) continue;

            channelIndex = (idx + 1) % sfxPlayers.Length;
            sfxPlayers[idx].PlayOneShot(clip);
            return;
        }

        // 모두 재생 중이면 무시(원하면 가장 오래된 채널 덮어쓰기 가능)
        sfxPlayers[channelIndex].PlayOneShot(clip);
    }
}
