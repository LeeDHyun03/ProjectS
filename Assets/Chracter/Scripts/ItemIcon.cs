using UnityEngine;
using UnityEngine.UI;

public class ItemIcon : MonoBehaviour
{
    [SerializeField] private Image DescriptionPanel;
    [SerializeField] private Text ItemName;
    [SerializeField] private Text DescriptionText;

    public void SetItemInfo(string itemName, string description)
    {
        ItemName.text = itemName;
        DescriptionText.text = description;
    }

    public void OnDescriptionDisplay(bool isActivate)
    {
        Debug.Log("Description Display: " + isActivate);
        DescriptionPanel.gameObject.SetActive(isActivate);
    }
}
