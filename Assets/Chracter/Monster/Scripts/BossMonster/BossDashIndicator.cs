using UnityEngine;

public class BossDashIndicator : MonoBehaviour
{
    [Header("Visual")]
    [SerializeField] private SpriteRenderer baseSr; // optional
    [SerializeField] private SpriteRenderer fillSr; // optional
    [SerializeField] private Transform bossMonster;

    [Header("Collider")]
    [SerializeField] private BoxCollider2D boxCollider;

    [Header("Size")]
    [SerializeField] private float width = 1.2f;
    [SerializeField] private float length = 6f;

    private bool committed;

    private void Awake()
    {
        ApplySize(length);
        SetVisible(false);
    }

    public void SetVisible(bool on)
    {
        gameObject.SetActive(on);
        committed = false;
        SetCommittedVisual(false);
        SetFill01(0f);
    }

    public void SetWidth(float newWidth)
    {
        width = newWidth;
        ApplySize(length);
    }

    public void SetLength(float newLength)
    {
        length = Mathf.Max(0.1f, newLength);
        ApplySize(length);
    }

    private void ApplySize(float len)
    {
        if (baseSr != null)
            baseSr.size = new Vector2(width, len);

        if (fillSr != null)
            fillSr.size = new Vector2(width, 0);
    }

    public void UpdateAim(Vector2 dir)
    {
        if (committed) return;
        if (dir.sqrMagnitude < 0.0001f) return;
        float a = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, a + 90);
    }

    public void Commit(Vector2 dir)
    {
        committed = true;

        if (dir.sqrMagnitude >= 0.0001f)
        {
            float a = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, a + 90);
        }

        if (boxCollider != null)
        {
            boxCollider.size = new Vector2(width, length);

            boxCollider.offset = new Vector2(0, -length / 2f);
        }

        SetCommittedVisual(true);
        SetFill01(1f);
        transform.parent = null;
    }

    public void SetFill01(float t01)
    {
        if (fillSr == null) return;
        transform.parent = bossMonster.transform;
        transform.localPosition = Vector3.zero;
        t01 = Mathf.Clamp01(t01);
        fillSr.size = new Vector2(width, length * t01);
    }

    private void SetCommittedVisual(bool isCommitted)
    {
        if (baseSr != null)
            baseSr.color = isCommitted ? new Color(1f, 0.25f, 0.25f, 0.75f) : new Color(1f, 0f, 0f, 0.35f);

        if (fillSr != null)
            fillSr.color = isCommitted ? new Color(1f, 0.35f, 0.35f, 0.85f) : new Color(1f, 0.2f, 0.2f, 0.55f);
    }
}
