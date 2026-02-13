using System;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1000)]
public class BgmManager : MonoBehaviour
{
    public static BgmManager Instance { get; private set; }

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

    [Header("BGM")]
    [SerializeField] private BgmGroup BGM = new();

    [Header("Volume")]
    [SerializeField, Range(0f, 1f)] private float masterVolume = 1f;
    [SerializeField, Range(0f, 1f)] private float bgmVolume = 1f;

    [Header("Player")]
    [SerializeField] private AudioSource bgmPlayer;


    public float GetBgmVolume() => bgmVolume;
    public float GetMasterVolume() => masterVolume;


    private Dictionary<string, AudioClip> bgmMap;

    private const string PREF_MASTER = "MasterVolume";
    private const string PREF_BGM = "BgmVolume";

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        BuildMap();
        InitPlayer();
        LoadVolumes();
    }

    private void BuildMap()
    {
        bgmMap = new Dictionary<string, AudioClip>(BGM?.Bgm?.Count ?? 0, StringComparer.Ordinal);

        if (BGM?.Bgm == null) return;

        foreach (var e in BGM.Bgm)
        {
            if (string.IsNullOrWhiteSpace(e.Name) || e.Clip == null) continue;

            if (bgmMap.ContainsKey(e.Name))
            {
                Debug.LogWarning($"[BgmManager] Duplicate key '{e.Name}' (first one kept)");
                continue;
            }
            bgmMap.Add(e.Name, e.Clip);
        }
    }

    private void InitPlayer()
    {
        if (!bgmPlayer)
        {
            var obj = new GameObject("BgmPlayer");
            obj.transform.SetParent(transform);
            bgmPlayer = obj.AddComponent<AudioSource>();
        }

        bgmPlayer.playOnAwake = false;
        bgmPlayer.loop = true;

        ApplyVolumes();
    }

    private void LoadVolumes()
    {
        masterVolume = PlayerPrefs.GetFloat(PREF_MASTER, masterVolume);
        bgmVolume = PlayerPrefs.GetFloat(PREF_BGM, bgmVolume);
        ApplyVolumes();
        Debug.Log($"[BgmManager] master={masterVolume}, bgm={bgmVolume}, final={masterVolume * bgmVolume}");

    }

    private void ApplyVolumes()
    {
        if (bgmPlayer)
            bgmPlayer.volume = masterVolume * bgmVolume;
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

    public void Play(string name)
    {
        if (bgmPlayer == null || bgmMap == null) return;

        if (!bgmMap.TryGetValue(name, out var clip) || clip == null)
        {
            Debug.LogWarning($"[BgmManager] BGM not found: '{name}'");
            return;
        }

        if (bgmPlayer.isPlaying && bgmPlayer.clip == clip) return;

        bgmPlayer.clip = clip;
        bgmPlayer.Play();
    }

    public void Stop()
    {
        if (bgmPlayer) bgmPlayer.Stop();
    }
}
