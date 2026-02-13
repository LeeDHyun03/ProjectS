using UnityEngine;

public class KBMouseInput : MonoBehaviour
{
    public GameObject arrow;
    public KBCart cart;
    bool isMouseUse = true;
    private void Awake()
    {
        cart = FindAnyObjectByType<KBCart>();
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
            arrow.SetActive(false);
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
        arrow.transform.right = PlayerToMouseDir();
    }
    private void OnEnable()
    {
        cart.OnReAct += MouseDirChangeModeTrigger;
    }
    void MouseDirChangeModeTrigger()
    {
        isMouseUse = true;
        arrow.transform.position = cart.transform.position;
        arrow.SetActive(true);
    }
    private void OnDisable()
    {
        cart.OnReAct -= MouseDirChangeModeTrigger;
    }
}
