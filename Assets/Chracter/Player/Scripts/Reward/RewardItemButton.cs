using Roguelike.Items;
using UnityEngine;
using UnityEngine.UI;

public class RewardItemButton : MonoBehaviour
{
    private string itemID;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(SelectReward);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Initialize(string itemID)
    {
        this.itemID = itemID;
        ItemDataManager.Instance.TryGetItem(itemID, out ItemData item);

        // TODO: .SetData를 내용 대신 id 형태로
        Sprite iconSprite = ItemDataManager.Instance.LoadIcon(item);
        Sprite iconFrameSprite = ItemDataManager.Instance.LoadItemFrame(item);
        GetComponent<ItemTooltip>().SetData(item.NameKr, item.Description, iconSprite, iconFrameSprite);
    }

    private void SelectReward()
    {
        // TODO: 시스템쪽으로 로직 분리
        PlayerCharacter player = FindFirstObjectByType<PlayerCharacter>();
        player.GetComponent<PlayerItemStatController>().AddItem(itemID);
    }
}
