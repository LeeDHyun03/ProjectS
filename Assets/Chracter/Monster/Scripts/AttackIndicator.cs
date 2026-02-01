using System;
using System.Collections;
using UnityEngine;

public class AttackIndicator : MonoBehaviour
{
    [SerializeField] private SpriteRenderer fillSprite;
    [SerializeField] private SpriteRenderer borderSprite;

    private Vector2 fillSpriteSize;

    public event Action OnIndicatorComplete;

    public void StartIndicator(Vector2 size, float duration)
    {
        Vector2 reverseSize = new Vector2(size.y, size.x);
        borderSprite.size = reverseSize;
        fillSpriteSize = reverseSize;

        fillSprite.size = new Vector2(fillSpriteSize.x, 0);

        StartCoroutine(FillRoutine(duration));
    }

    private IEnumerator FillRoutine(float duration)
    {
        float t = 0;
        while (t < fillSpriteSize.y)
        {
            t += Time.deltaTime / duration;
            fillSprite.size = new Vector2(fillSpriteSize.x, t);
            yield return null;
        }
        transform.parent = null;
        OnIndicatorComplete?.Invoke();
        OnIndicatorComplete = null;
        ObjectPooler.ReturnToPool(gameObject);
    }
} 