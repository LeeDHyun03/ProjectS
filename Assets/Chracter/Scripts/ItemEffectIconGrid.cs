using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemEffectIconGrid : MonoBehaviour
{
    private struct EffectIconInfo
    {
        public Image iconImage;
        public ItemEffectType effectType;

        public bool EqualityEffectType(EffectIconInfo other)
        {
            if (effectType == other.effectType)
                return true;
            else 
                return false;
        }
    }
    public enum ItemEffectType
    {
        Positive,
        Negative,
        Normal,
        None
    }

    Coroutine effectDurationCoroutine;

    private int EffectIconCount = 0;

    [SerializeField]
    private Image[] DeactivateIconArray = new Image[15];
    private List<EffectIconInfo> ActivateIconList = new();

    private void Start()
    {

    }

    public void AddEffectIcon(Sprite iconSprite, ItemEffectType effectType, float duration)
    {
        if (EffectIconCount >= DeactivateIconArray.Length)
            return;
        EffectIconInfo newEffectIconInfo = new EffectIconInfo
        {
            iconImage = DeactivateIconArray[EffectIconCount],
            effectType = effectType
        };
        newEffectIconInfo.iconImage.sprite = iconSprite;
        newEffectIconInfo.iconImage.gameObject.SetActive(true);
        ActivateIconList.Add(newEffectIconInfo);
        EffectIconCount++;
        if (effectDurationCoroutine != null)
            StopCoroutine(effectDurationCoroutine);
        effectDurationCoroutine = StartCoroutine(EffectDurationCoroutine(duration, newEffectIconInfo));
    }
    IEnumerator EffectDurationCoroutine(float duration, EffectIconInfo newEffectIconInfo)
    {
        yield return new WaitForSeconds(duration);
        ActivateIconList.RemoveAll(info => info.EqualityEffectType(newEffectIconInfo));
        newEffectIconInfo.iconImage.gameObject.SetActive(false);
        EffectIconCount--;
    }
}
