using UnityEditor.VersionControl;
using UnityEngine;
public enum OAActTileColor
{
    Red,
    Blue
}

public class OAActTile : OATileController
{
    public OAActTileColor myColor;
    public bool isAct;
    public Sprite able, unable;
    SpriteRenderer sr;
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        ActiveChange();
    }

    public void ToggleActive()
    {
        isAct = !isAct;
        ActiveChange();
    }
    void ActiveChange()
    {
        GetComponent<Collider2D>().enabled = isAct;
        SpriteChange();
    }
    void SpriteChange()
    {
        if (isAct)
        {
            sr.sprite = able;
        }
        else
        {
            sr.sprite = unable;
        }
    }
}
