using UnityEngine;

public class KBMagicJin : PuzzleElement
{
    public Sprite OnMagic;
    SpriteRenderer sr;
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        sr.sprite = OnMagic;
    }
}
