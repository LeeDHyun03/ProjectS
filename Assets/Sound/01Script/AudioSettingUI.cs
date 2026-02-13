using UnityEngine;
using UnityEngine.UI;

public class AudioSettingUI : MonoBehaviour
{
    [SerializeField] private Scrollbar masterSlider;
    [SerializeField] private Scrollbar bgmSlider;
    [SerializeField] private Scrollbar sfxSlider;

    private void Start()
    {
        // 현재 저장된 값 반영
        masterSlider.value = BgmManager.Instance.GetMasterVolume();
        masterSlider.value = SfxManager.Instance.GetMasterVolume();
        bgmSlider.value = BgmManager.Instance.GetBgmVolume();
        sfxSlider.value = SfxManager.Instance.GetSfxVolume();

        masterSlider.onValueChanged.AddListener(OnMasterChanged);
        bgmSlider.onValueChanged.AddListener(OnBgmChanged);
        sfxSlider.onValueChanged.AddListener(OnSfxChanged);
    }

    private void OnMasterChanged(float value)
    {
        BgmManager.Instance.ChangeMasterVolume(value);
        SfxManager.Instance.ChangeMasterVolume(value);
    }

    private void OnBgmChanged(float value)
    {
        BgmManager.Instance.ChangeBgmVolume(value);
    }

    private void OnSfxChanged(float value)
    {
        SfxManager.Instance.ChangeSfxVolume(value);
    }
}
