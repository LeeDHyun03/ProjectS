using UnityEngine;

public class KBMagicJin : PZElement
{
    public Sprite OnMagic;
    SpriteRenderer sr;
    public override void Awake()
    {
        base.Awake();
        sr = GetComponent<SpriteRenderer>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        sr.sprite = OnMagic;
        //SoundManager.Instance.PlaySfx("KB_On");
    }
}
