using UnityEngine;

public class SpriteMirror : MonoBehaviour
{
    [SerializeField] private SpriteRenderer targetRenderer;
    private SpriteRenderer mirrorRenderer;

    void Start()
    {
        targetRenderer = transform.parent.GetComponent<SpriteRenderer>();
        mirrorRenderer = GetComponent<SpriteRenderer>();
    }

    void LateUpdate()
    {
        if (targetRenderer != null && mirrorRenderer != null)
        {
            mirrorRenderer.sprite = targetRenderer.sprite;

            mirrorRenderer.flipX = targetRenderer.flipX;
        }
    }
}