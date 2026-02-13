using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
using UnityEngine.SceneManagement;

public class ResultScreen : MonoBehaviour
{
    [SerializeField] private TMP_Text title;

    [SerializeField] private TMP_Text stage5Text;

    [SerializeField] private Image progressFill;
    [SerializeField] private RectTransform progressBarRect;
    [SerializeField] private RectTransform characterIcon;
    [SerializeField] private TMP_Text progressText;

    [SerializeField] private Button restartButton;

    [SerializeField] private Button returnToMainButton;

    [SerializeField] private float duration = 1.5f;

    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color overColor = new Color(1f, 0.5f, 0f);

    private Action _onComplete;
    private Vector2 characterInitialPos;

    private float progress;

    void Start()
    {
        restartButton.onClick.AddListener(() =>
        {
            Time.timeScale = 1;
            SceneManager.LoadScene("Field");
            gameObject.SetActive(false);
        });
        returnToMainButton.onClick.AddListener(() =>
        {
            Time.timeScale = 1;
            SceneManager.LoadScene("Title");
        });
    }
    void OnEnable()
    {
        Time.timeScale = 0;
        characterInitialPos = characterIcon.anchoredPosition;

        // progress = (현재 스테이지 x 5 + 현재 웨이브) / 25;
        progress = 1;

        if (progress >= 1)
        {
            title.text = "Clear";
        }
        Play(progress, OnProgressFinished);
    }

    void OnProgressFinished()
    {
        if (progress > 1)
        {
            stage5Text.color = overColor;
            stage5Text.text = $"Stage\n{10}"; // 10 -> 현재 스테이지로
        }
        Debug.Log("애니메이션 종료");
    }

    public void Play(float targetProgress, Action onComplete = null)
    {
        _onComplete = onComplete;

        bool isOver = targetProgress > 1f;
        Color targetColor = isOver ? overColor : normalColor;

        progressFill.color = targetColor;
        progressText.color = targetColor;

        StopAllCoroutines();
        StartCoroutine(AnimateProgress(targetProgress));
    }

    private IEnumerator AnimateProgress(float target)
    {
        float start = 0f;
        float time = 0f;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            float t = time / duration;

            float eased = 1f - Mathf.Pow(1f - t, 3f);
            float current = Mathf.Lerp(start, target, eased);

            UpdateVisual(current);

            yield return null;
        }

        UpdateVisual(target);
        _onComplete?.Invoke();
    }

    private void UpdateVisual(float actualProgress)
    {
        float fill = Mathf.Clamp01(actualProgress);
        progressFill.fillAmount = fill;

        UpdateCharacterPosition(fill);
        UpdateText(actualProgress);
    }

    private void UpdateCharacterPosition(float fillAmount)
    {
        float width = progressBarRect.rect.width;
        float xPos = characterInitialPos.x + width * fillAmount;

        Vector2 pos = characterIcon.anchoredPosition;
        pos.x = xPos;
        characterIcon.anchoredPosition = pos;
    }

    private void UpdateText(float actualProgress)
    {
        int percent = Mathf.RoundToInt(actualProgress * 100f);
        progressText.text = $"진행률: {percent}%";
    }
}
