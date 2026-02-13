using UnityEngine;

public class KBMouseInput : PZInteraction
{
    Collider2D arrowCol;
    SpriteRenderer arrowSr;
    public KBCart cart;
    bool isMouseUse = false;
    public override void Awake()
    {
        base.Awake();
        cart = FindAnyObjectByType<KBCart>();
        arrowCol = gameObject.GetComponent<Collider2D>();
        arrowSr = gameObject.GetComponent<SpriteRenderer>();
    }
    void Start()
    {
        
    }

    void Update()
    {
        SetMouseArrowDir();

        if (Input.GetMouseButtonDown(0) && isMouseUse)
        {
            cart.currentDir = PlayerToMouseDir();
            isMouseUse = false;
            ToggleVisibility(false);
        }
    }
    Vector3 GetMouseVec()
    {
        Vector3 mousePos = Input.mousePosition;

        mousePos.z = 10f;
            
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            
        return worldPos;
    }
    Vector3 PlayerToMouseDir()
    {
        return (GetMouseVec() - cart.transform.position).normalized;
    }
    void SetMouseArrowDir()
    {
        if (!isMouseUse)
            return;
        transform.right = PlayerToMouseDir();
    }
    private void OnEnable()
    {
        cart.OnReAct += MouseDirChangeModeTrigger;
    }
    void MouseDirChangeModeTrigger()
    {
        transform.position = cart.transform.position;
    }
    private void OnDisable()
    {
        cart.OnReAct -= MouseDirChangeModeTrigger;
    }
    public override void Interaction(bool enable)
    {
        isMouseUse = enable;
        ToggleVisibility(enable);
    }
    void ToggleVisibility(bool toggle)
    {
        arrowSr.enabled = toggle;
    }
}
