using UnityEngine;

public class MCLever : PZInteraction
{
    public GameObject Text;
    public GameObject Wall;
    public Sprite onLever, offLever;
    SpriteRenderer sr;
    bool isActive;
    bool isInteractable;
    public override void Awake()
    {
        base.Awake();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (isInteractable && Input.GetKeyDown(KeyCode.Z))
        {
            isActive = !isActive;
            Wall.SetActive(!isActive);
            sr.sprite = isActive ? onLever : offLever;
            SfxManager.Instance.Play("Switch_Button");
        }
    }
    public override void Interaction(bool enable)
    {
        Text.SetActive(enable);
        isInteractable = enable;
    }
}
