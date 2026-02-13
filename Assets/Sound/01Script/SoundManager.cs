using System;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1000)]
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    // =========================
    // Inspector Data
    // =========================

    [Serializable]
    public struct NamedClip
    {
        public string Name;
        public AudioClip Clip;
    }

    [Serializable]
    public class BgmGroup
    {
        public List<NamedClip> Bgm = new();
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

    [Header("BGM")]
    [SerializeField] private BgmGroup BGM = new();

    [Header("SFX")]
    [SerializeField] private SfxGroups SFXS = new();
    [SerializeField, Min(1)] private int Channels = 25;

    [Header("Volume")]
    [SerializeField, Range(0f, 1f)] private float masterVolume = 1f;
    [SerializeField, Range(0f, 1f)] private float bgmVolume = 1f;
    [SerializeField, Range(0f, 1f)] private float sfxVolume = 1f;

    // =========================
    // Players
    // =========================
    [SerializeField] private AudioSource bgmPlayer;
    private AudioSource[] sfxPlayers;
    private int channelIndex;

    // =========================
    // Runtime Maps
    // =========================
    private Dictionary<string, AudioClip> bgmMap;
    private Dictionary<string, AudioClip> sfxMap;

    private const string PREF_MASTER = "MasterVolume";
    private const string PREF_BGM = "BgmVolume";
    private const string PREF_SFX = "SfxVolume";

    // =========================
    // Unity
    // =========================
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        BuildMaps();
        InitPlayers();
        LoadVolumes();
    }

    // =========================
    // Map Build
    // =========================
    private void BuildMaps()
    {
        bgmMap = BuildMap(BGM?.Bgm, "BGM");
        sfxMap = new Dictionary<string, AudioClip>(256, StringComparer.Ordinal);

        MergeInto(sfxMap, SFXS?.Character_Sfx, "SFX/Character");
        MergeInto(sfxMap, SFXS?.Monster_Sfx, "SFX/Monster");
        MergeInto(sfxMap, SFXS?.Puzzle_Sfx, "SFX/Puzzle");
        MergeInto(sfxMap, SFXS?.Item_Sfx, "SFX/Item");
        MergeInto(sfxMap, SFXS?.UI_Sfx, "SFX/UI");
    }

    private Dictionary<string, AudioClip> BuildMap(List<NamedClip> list, string label)
    {
        var map = new Dictionary<string, AudioClip>(list?.Count ?? 0, StringComparer.Ordinal);
        MergeInto(map, list, label);
        return map;
    }

    private void MergeInto(Dictionary<string, AudioClip> map, List<NamedClip> list, string label)
    {
        if (list == null) return;

        for (int i = 0; i < list.Count; i++)
        {
            var e = list[i];

            if (string.IsNullOrWhiteSpace(e.Name) || e.Clip == null)
                continue;

            if (map.ContainsKey(e.Name))
            {
                Debug.LogWarning($"[SoundManager] Duplicate key '{e.Name}' in {label}. (first one kept)");
                continue;
            }

            map.Add(e.Name, e.Clip);
        }
    }

    // =========================
    // Player Init
    // =========================
    private void InitPlayers()
    {
        // BGM Player
        if (!bgmPlayer)
        {
            var bgmObj = new GameObject("BgmPlayer");
            bgmObj.transform.SetParent(transform);
            bgmPlayer = bgmObj.AddComponent<AudioSource>();
            bgmPlayer.playOnAwake = false;
            bgmPlayer.loop = true;
        }

        // SFX Players
        var sfxObj = new GameObject("SfxPlayers");
        sfxObj.transform.SetParent(transform);

        sfxPlayers = new AudioSource[Mathf.Max(1, Channels)];
        for (int i = 0; i < sfxPlayers.Length; i++)
        {
            var src = sfxObj.AddComponent<AudioSource>();
            src.playOnAwake = false;
            src.loop = false;
            sfxPlayers[i] = src;
        }

        ApplyVolumes();
    }

    // =========================
    // Volume
    // =========================
    private void LoadVolumes()
    {
        masterVolume = PlayerPrefs.GetFloat(PREF_MASTER, masterVolume);
        bgmVolume = PlayerPrefs.GetFloat(PREF_BGM, bgmVolume);
        sfxVolume = PlayerPrefs.GetFloat(PREF_SFX, sfxVolume);
        ApplyVolumes();
    }

    private void ApplyVolumes()
    {
        if (bgmPlayer)
            bgmPlayer.volume = masterVolume * bgmVolume;

        if (sfxPlayers != null)
        {
            float finalSfx = masterVolume * sfxVolume;
            for (int i = 0; i < sfxPlayers.Length; i++)
                sfxPlayers[i].volume = finalSfx;
        }
    }

    public void ChangeMasterVolume(float value)
    {
        masterVolume = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat(PREF_MASTER, masterVolume);
        ApplyVolumes();
    }

    public void ChangeBgmVolume(float value)
    {
        bgmVolume = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat(PREF_BGM, bgmVolume);
        ApplyVolumes();
    }

    public void ChangeSfxVolume(float value)
    {
        sfxVolume = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat(PREF_SFX, sfxVolume);
        ApplyVolumes();
    }

    // =========================
    // Play
    // =========================
    public void PlayBgm(string name)
    {
        if (bgmPlayer == null || bgmMap == null) return;

        if (!bgmMap.TryGetValue(name, out var clip) || clip == null)
        {
            Debug.LogWarning($"[SoundManager] BGM not found: '{name}'");
            return;
        }

        if (bgmPlayer.isPlaying && bgmPlayer.clip == clip)
            return;

        bgmPlayer.clip = clip;
        bgmPlayer.Play();
    }

    public void PlaySfx(string name)
    {
        if (sfxPlayers == null || sfxMap == null) return;

        if (!sfxMap.TryGetValue(name, out var clip) || clip == null)
        {
            Debug.LogWarning($"[SoundManager] SFX not found: '{name}'");
            return;
        }

        for (int i = 0; i < sfxPlayers.Length; i++)
        {
            int idx = (channelIndex + i) % sfxPlayers.Length;

            if (sfxPlayers[idx].isPlaying)
                continue;

            channelIndex = (idx + 1) % sfxPlayers.Length;
            sfxPlayers[idx].PlayOneShot(clip);
            return;
        }

        // 전부 재생 중이면 무시
        // sfxPlayers[channelIndex].PlayOneShot(clip);
    }
}
