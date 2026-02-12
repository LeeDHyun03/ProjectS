using UnityEngine;

public class MCLever : PZInteraction
{
    public GameObject Text;
    public GameObject Wall;
    public Sprite onLever, offLever;
    SpriteRenderer sr;
    bool isActive;
    bool isInteractable;
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if(isActive)
            {
                isActive = false;
            }
            else if(isInteractable)
            {
                isActive = true;
            }
            Wall.SetActive(isActive);
            sr.sprite = isActive ? onLever : offLever;
        }
    }
    public override void Interaction(bool enable)
    {
        Text.SetActive(enable);
        isInteractable = enable;
    }
}
