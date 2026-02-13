using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemTooltip : MonoBehaviour
{
    [SerializeField] private bool isAutoShowingTooltip = true;
    [SerializeField] private Image DescriptionIconImage;
    [SerializeField] private Image DescriptionIconFrame;
    [SerializeField] private TMP_Text ItemName;
    [SerializeField] private TMP_Text DescriptionText;
    [SerializeField] private Image DescriptionPanel;

    [SerializeField] private Transform KeywordInfoList;
    [SerializeField] private GameObject KeywordInfoPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void SetData(string itemName, string description, Sprite iconSprite, Sprite iconFrameSprite)
    {
        ItemName.text = itemName;
        DescriptionText.text = description;
        DescriptionIconImage.sprite = iconSprite;
        DescriptionIconFrame.sprite = iconFrameSprite;

        if (isAutoShowingTooltip) ShowKeywordInfoTooltips();
    }
    private void ShowKeywordInfoTooltips()
    {
        KeywordInfoList.DestroyChildren();

        DescriptionText.textInfo.ClearAllMeshInfo();
        DescriptionText.ForceMeshUpdate(true, true);

        List<string> seenID = new();

        TMP_TextInfo textInfo = DescriptionText.textInfo;

        for (int i = 0; i < textInfo.linkCount; i++)
        {
            TMP_LinkInfo linkInfo = textInfo.linkInfo[i];

            string id = linkInfo.GetLinkID();
            if (string.IsNullOrEmpty(id)) continue;

            ItemKeywordDataManager.KeywordInfo data = ItemKeywordDataManager.Instance.GetKeywordInfo(id);

            if (data != null && !seenID.Contains(id))
            {
                GameObject keywordInfo = Instantiate(KeywordInfoPrefab, KeywordInfoList);

                ItemKeywordInfo keywordInfoComponent = keywordInfo.GetComponent<ItemKeywordInfo>();
                keywordInfoComponent.SetData(data.name, data.description);

                seenID.Add(id);
            }
        }
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(
            KeywordInfoList as RectTransform
        );
    }
}
