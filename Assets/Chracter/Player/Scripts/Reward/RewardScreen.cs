using Roguelike.Items;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardScreen : MonoBehaviour
{
    [SerializeField] private Button closeButton;
    [SerializeField] private Transform tooltipContainer;
    [SerializeField] private GameObject clickableItemTooltipPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        closeButton.onClick.AddListener(Close);
    }

    void Close()
    {
        gameObject.SetActive(false);
    }

    public void SetRewards(List<ItemData> rewardItemIDList)
    {
        Dbg.L("개수세봐", rewardItemIDList.Count);
        tooltipContainer.DestroyChildren();

        foreach (ItemData itemData in rewardItemIDList)
        {
            GameObject tooltip = Instantiate(clickableItemTooltipPrefab, tooltipContainer);
            tooltip.TryGetComponent<RewardItemButton>(out RewardItemButton button);
            Time.timeScale = 0;
            if (button != null)
                button.Initialize(itemData);
            else
                Debug.Log("RewardItemButton is Null");
        }
    }
}
