using System.Collections;
using System.Collections.Generic;
using Roguelike.Items;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class InteractableText : MonoBehaviour
{

    private TextMeshProUGUI tmpro;

    private int currentActiveLink;


    // Start is called before the first frame update
    void Start()
    {
        tmpro = GetComponent<TextMeshProUGUI>();
    }
    // Update is called once per frame
    void Update()
    {
        CheckMouseHover();
    }
    void CheckMouseHover()
    {
        Vector3 mousePos = Input.mousePosition;

        Canvas canvas = tmpro.canvas;
        Camera camera = canvas.worldCamera;

        bool isOverText = TMP_TextUtilities.IsIntersectingRectTransform(
            tmpro.rectTransform,
            mousePos,
            camera
        );

        if (!isOverText)
        {
            DisableTooltip();
            return;
        }

        int targetLink = TMP_TextUtilities.FindIntersectingLink(
            tmpro,
            mousePos,
            camera
        );

        if (currentActiveLink != targetLink) DisableTooltip();

        if (targetLink == -1) return;

        TMP_LinkInfo linkInfo = tmpro.textInfo.linkInfo[targetLink];
        string id = linkInfo.GetLinkID();

        ItemKeywordDataManager.KeywordInfo keywordInfo = ItemKeywordDataManager.Instance.GetKeywordInfo(id);
        if (keywordInfo == null) return;

        ItemKeywordTooltip tooltip = PlayerUI.Instance.itemKeywordTooltip;

        tooltip.gameObject.SetActive(true);
        tooltip.SetData(
            keywordInfo.name,
            keywordInfo.description
        ).FollowMousePos();

        currentActiveLink = targetLink;
    }

    void DisableTooltip()
    {
        var tooltip = PlayerUI.Instance.itemKeywordTooltip.gameObject;
        if (!tooltip.activeSelf) return;
        tooltip.SetActive(false);
    }
}
