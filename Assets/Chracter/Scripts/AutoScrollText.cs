using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AutoScrollText : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private float baseScrollSpeed = 20f;
    [SerializeField] private float resetDelay = 1f;
    [SerializeField] private float startDelay = 1f;

    private RectTransform content;
    private RectTransform viewport;
    private float scrollSpeed;
    private bool scrolling = false;

    private void Awake()
    {
        content = scrollRect.content;
        viewport = scrollRect.viewport;
    }

    private void OnEnable()
    {
        ResetState();
    }

    public void ResetState()
    {
        ResetScroll();
        CalculateScrollSpeed();
        if (content.rect.height > viewport.rect.height)
            StartCoroutine(AutoScroll());
    }

    private void CalculateScrollSpeed()
    {
        float hiddenHeight = content.rect.height - viewport.rect.height;
        if (hiddenHeight <= 0)
        {
            scrollSpeed = 0;
            return;
        }
        scrollSpeed = baseScrollSpeed + hiddenHeight * 0.05f;
    }

    private void ResetScroll()
    {
        scrollRect.verticalNormalizedPosition = 1f;
    }

    private IEnumerator AutoScroll()
    {
        scrolling = true;

        yield return new WaitForSeconds(startDelay);

        while (scrolling)
        {
            float hiddenHeight = content.rect.height - viewport.rect.height;
            if (hiddenHeight <= 0) yield break;

            float scrollAmount = scrollSpeed / hiddenHeight * Time.deltaTime;
            scrollRect.verticalNormalizedPosition -= scrollAmount;

            if (scrollRect.verticalNormalizedPosition <= 0f)
            {
                scrollRect.verticalNormalizedPosition = 0f;
                yield return new WaitForSeconds(resetDelay);
                ResetScroll();

                yield return new WaitForSeconds(startDelay);
            }

            yield return null;
        }
    }
}
