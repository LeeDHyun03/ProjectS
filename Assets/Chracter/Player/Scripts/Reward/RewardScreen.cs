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
    // Update is called once per frame
    void Update()
    {

    }

    public void SetRewards(List<string> rewardItemIDList)
    {
        tooltipContainer.DestroyChildren();

        foreach (string id in rewardItemIDList)
        {
            GameObject tooltip = Instantiate(clickableItemTooltipPrefab, tooltipContainer);
            tooltip.GetComponent<RewardItemButton>().Initialize(id);
        }
    }
}
