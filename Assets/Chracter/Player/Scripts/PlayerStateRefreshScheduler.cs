using UnityEngine;

[DisallowMultipleComponent]
public sealed class PlayerStatRefreshScheduler : MonoBehaviour
{
    private PlayerItemStatController _itemStats;
    private PlayerConditionalStatController _cond;

    private bool _dirty;
    private int _dirtyFrame;

    private void Awake()
    {
        _itemStats = GetComponent<PlayerItemStatController>();
        _cond = GetComponent<PlayerConditionalStatController>();
    }

    public void MarkDirty(float cur, float max)
    {
        _dirty = true;
        _dirtyFrame = Time.frameCount;
    }

    private void LateUpdate()
    {
        if (!_dirty) return;

        if (Time.frameCount == _dirtyFrame)
        {
            _dirty = false;

            if (_cond) _cond.Reevaluate();
            if (_itemStats) _itemStats.RebuildAndApply();
        }
    }
}
