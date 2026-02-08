using System;
using System.Collections;
using UnityEngine;

public class AttackCircleIndicator : MonoBehaviour, IAttackIndicator
{
    [SerializeField] private SpriteRenderer fillSprite;
    [SerializeField] private SpriteRenderer borderSprite;

    private Vector2 fillSpriteSize;
    public event Action OnIndicatorComplete;

    private Coroutine filling;

    public void StartIndicator(Vector2 size, float duration)
    {
        float diameter = Mathf.Max(size.x, size.y);
        fillSpriteSize = new Vector2(diameter, diameter);

        borderSprite.size = fillSpriteSize;
        fillSprite.size = Vector2.zero;

        if (filling != null) 
            StopCoroutine(filling);
        filling = StartCoroutine(FillRoutine(duration));
    }

    private IEnumerator FillRoutine(float duration)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            float currentFill = Mathf.Lerp(0f, fillSpriteSize.x, t);
            fillSprite.size = new Vector2(currentFill, currentFill);
            yield return null;
        }

        fillSprite.size = fillSpriteSize;
        transform.parent = null;

        OnIndicatorComplete?.Invoke();
        OnIndicatorComplete = null;

        filling = null;
        ObjectPooler.ReturnToPool(gameObject);
    }

    private void OnDisable()
    {
        if (filling != null)
        {
            StopCoroutine(filling);
            filling = null;
        }
        OnIndicatorComplete = null;
    }
}
