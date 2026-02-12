using Roguelike.Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemIcon : MonoBehaviour
{
    [SerializeField] private Image IconImage;
    [SerializeField] private Image IconFrame;


    [SerializeField] private Image DescriptionIconImage;
    [SerializeField] private Image DescriptionIconFrame;
    [SerializeField] private TMP_Text ItemName;
    [SerializeField] private TMP_Text DescriptionText;
    [SerializeField] private Image DescriptionPanel;

    public void SetItemInfo(string itemName, string description, Sprite iconSprite, Sprite iconFrameSprite)
    {
        IconImage.sprite = iconSprite;
        IconFrame.sprite = iconFrameSprite;
        ItemName.text = itemName;
        DescriptionText.text = description;
        DescriptionIconImage.sprite = iconSprite;
        DescriptionIconFrame.sprite = iconFrameSprite;

    }

    public void OnDescriptionDisplay(bool isActivate)
    {
        DescriptionPanel.gameObject.SetActive(isActivate);
    }
}
