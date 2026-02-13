using System.Collections.Generic;
using System.Linq;
using Roguelike.Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemIcon : MonoBehaviour
{
    [SerializeField] private Image IconImage;
    [SerializeField] private Image IconFrame;

    private string itemName;
    private string itemDescription;

    public void SetItemInfo(string name, string description, Sprite iconSprite, Sprite iconFrameSprite)
    {
        itemName = name;
        itemDescription = description;
        IconImage.sprite = iconSprite;
        IconFrame.sprite = iconFrameSprite;
    }
    public void OnDescriptionDisplay(bool isActivate)
    {
        ItemTooltip itemTooltip = PlayerUI.Instance.itemTooltip;

        itemTooltip.gameObject.SetActive(isActivate);

        if (isActivate)
        {
            itemTooltip.SetData(itemName, itemDescription, IconImage.sprite, IconFrame.sprite);
            itemTooltip.gameObject.ShowAtRightOfUI(transform);
        }
    }
}
