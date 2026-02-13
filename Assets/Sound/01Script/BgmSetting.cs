using UnityEngine;

public class BgmSetting : MonoBehaviour
{

    [SerializeField] private string bgmName;

    private void Start()
    {
        BgmManager.Instance.Play(bgmName);
    }
}
