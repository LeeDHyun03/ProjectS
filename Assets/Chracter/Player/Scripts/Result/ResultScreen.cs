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
    [SerializeField] private Color overColor = new(1f, 0.5f, 0f);

    private Action _onComplete;
    private Vector2 characterInitialPos;

    private float progress;

    void Start()
    {
        restartButton.onClick.AddListener(() =>
        {
            Reset();
            SceneManager.LoadScene("Field");
            gameObject.SetActive(false);
        });
        returnToMainButton.onClick.AddListener(() =>
        {
            Reset();
            SceneManager.LoadScene("Title");
            gameObject.SetActive(false);
        });
    }
    void Reset()
    {
        Time.timeScale = 1;
        PlayerCharacter player = FindFirstObjectByType<PlayerCharacter>();
        player.transform.position = new Vector2(0, 0);
        PlayerItemStatController statController = player.GetComponent<PlayerItemStatController>();
        statController.ResetRunItems();
        statController.ClearItemIcons();

        GameManager.Instance.ResetGameState();

        Dbg.L("asdasdasd", GameManager.Instance.currentStage);
        PlayerUI.Instance.GetRewardScreen().gameObject.SetActive(false);
        // ^ 죽는 타이밍에 레벨업해서 재시작 시 아이템 보상을 획득할 수 있는 현상 방지
    }
    void OnEnable()
    {
        Act();
    }

    void Act()
    {
        characterIcon.anchoredPosition = new Vector2(-486, 132);
        Time.timeScale = 0;
        characterInitialPos = characterIcon.anchoredPosition;

        Debug.Log(GameManager.Instance.currentStage);
        Debug.Log(GameManager.Instance.GetElapsedTotalWave());
        progress = GameManager.Instance.GetElapsedTotalWave() / 25f;

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
            stage5Text.text = $"Stage\n{GameManager.Instance.currentStage}";
        }
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
