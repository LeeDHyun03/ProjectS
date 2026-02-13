using Roguelike.Items;
using UnityEngine;
using UnityEngine.UI;

public class RewardItemButton : MonoBehaviour
{
    private string itemID;

    public void Initialize(ItemData itemData)
    {
        this.itemID = itemData.ItemId;

        // TODO: .SetData를 내용 대신 id 형태로
        Sprite iconSprite = ItemDataManager.Instance.LoadIcon(itemData);
        Sprite iconFrameSprite = ItemDataManager.Instance.LoadItemFrame(itemData);
        GetComponent<ItemTooltip>().SetData(itemData.NameKr, itemData.Description, iconSprite, iconFrameSprite);
    }

    public void SelectReward()
    {
        // TODO: 시스템쪽으로 로직 분리
        PlayerCharacter player = FindFirstObjectByType<PlayerCharacter>();
        player.TryGetComponent<PlayerItemStatController>(out var itemStatController);
        if (itemStatController != null)
            itemStatController.AddItem(itemID);
        else
            Debug.Log("itemStatController is Null");


        PlayerUI.Instance.DeactivateRewardScreen();
    }
}
