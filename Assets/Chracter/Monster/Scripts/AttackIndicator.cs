using System;
using System.Collections;
using UnityEngine;

public class AttackIndicator : MonoBehaviour, IAttackIndicator
{
    [SerializeField] private SpriteRenderer fillSprite;
    [SerializeField] private SpriteRenderer borderSprite;

    private Vector2 fillSpriteSize;
    public event Action OnIndicatorComplete;

    private Coroutine filling;

    public void StartIndicator(Vector2 size, float duration)
    {

        Vector2 reverseSize = new Vector2(size.y, size.x);
        borderSprite.size = reverseSize;
        fillSpriteSize = reverseSize;

        fillSprite.size = new Vector2(fillSpriteSize.x, 0);

        if(filling != null)
            StopCoroutine(filling);
        filling = StartCoroutine(FillRoutine(duration));
    }

    private IEnumerator FillRoutine(float duration)
    {
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / duration;

            float currentFill = Mathf.Lerp(0, fillSpriteSize.y, t);

            fillSprite.size = new Vector2(fillSpriteSize.x, currentFill);
            yield return null;
        }
        fillSprite.size = fillSpriteSize;
        transform.parent = null;

        OnIndicatorComplete?.Invoke();
        OnIndicatorComplete = null;

        filling = null;
        ObjectPooler.ReturnToPool(gameObject);
    }

    public Vector2 GetBaseSize() => fillSprite.size;

    private void OnDisable()
    {
        if(filling != null)
        {
            StopCoroutine(filling);
            filling = null;
        }
        OnIndicatorComplete = null;
    }
} 