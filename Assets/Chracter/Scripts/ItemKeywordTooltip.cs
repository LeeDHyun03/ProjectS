using TMPro;
using UnityEngine;

public class ItemKeywordTooltip : MonoBehaviour
{
    [SerializeField] private TMP_Text keywordName;
    [SerializeField] private TMP_Text keywordDesc;

    private bool shoudFollowMouse = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {
        if (!shoudFollowMouse) return;
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 1;

        gameObject.ShowAtRightOfPos(Input.mousePosition, 30);
        return;
    }
    private void OnEnable()
    {
        UpdatePosition();
    }
    private void OnDisable()
    {
        shoudFollowMouse = false;
    }
    public void FollowMousePos()
    {
        shoudFollowMouse = true;
    }
    public ItemKeywordTooltip SetData(string name, string desc)
    {
        keywordName.text = name;
        keywordDesc.text = desc;

        return this;
    }
}
