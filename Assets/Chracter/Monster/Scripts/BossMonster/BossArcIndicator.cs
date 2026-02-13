using UnityEngine;

public class BossArcIndicator : MonoBehaviour
{
    [SerializeField] private Transform fillVisual; // 채움 표현(스케일 x)
    [SerializeField] private SpriteRenderer baseSr;
    [SerializeField] private SpriteRenderer fillSr;

    private bool committed;

    private void Awake()
    {
        SetVisible(false);
        SetFill01(0f);
    }

    public void SetVisible(bool on)
    {
        gameObject.SetActive(on);
        committed = false;
        SetCommittedVisual(false);
        SetFill01(0f);
    }

    public void UpdateAim(Vector2 dir)
    {
        if (committed) return;
        if (dir.sqrMagnitude < 0.0001f) return;

        float a = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, a);
    }

    public void Commit(Vector2 dir)
    {
        committed = true;
        if (dir.sqrMagnitude >= 0.0001f)
        {
            float a = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, a);
        }
        SetCommittedVisual(true);
    }

    public void SetFill01(float t01)
    {
        if (fillVisual == null) return;
        t01 = Mathf.Clamp01(t01);

        Vector3 s = fillVisual.localScale;
        s.x = t01;
        fillVisual.localScale = s;
    }

    private void SetCommittedVisual(bool isCommitted)
    {
        if (baseSr != null)
            baseSr.color = isCommitted ? new Color(1f, 0.3f, 0.3f, 0.75f) : new Color(1f, 0f, 0f, 0.35f);

        if (fillSr != null)
            fillSr.color = isCommitted ? new Color(1f, 0.45f, 0.45f, 0.85f) : new Color(1f, 0.2f, 0.2f, 0.55f);
    }
}
