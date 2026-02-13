using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class WaterWalkEffect : MonoBehaviour, IPooledObject
{
    [SerializeField] private float targetScaleMultiplier = 2f; // 초기 크기의 N배
    [SerializeField] private float scaleSpeed = 1f;            // 커지는 속도
    [SerializeField] private float fadeSpeed = 1f;             // 알파 감소 속도

    private Color initialColor;
    private Vector3 initialScale;
    private Vector3 targetScale;

    private SpriteRenderer spriteRenderer;

    public void OnObjectSpawn()
    {
        transform.localScale = initialScale;
        spriteRenderer.color = initialColor;
    }

    void OnEnable()
    {
        OnObjectSpawn();
    }

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        initialScale = transform.localScale;
        initialColor = spriteRenderer.color;
        targetScale = initialScale * targetScaleMultiplier;
    }

    void Update()
    {
        transform.localScale = Vector3.MoveTowards(
            transform.localScale,
            targetScale,
            scaleSpeed * Time.deltaTime
        );
        Color c = spriteRenderer.color;
        c.a -= fadeSpeed * Time.deltaTime;
        spriteRenderer.color = c;
        if (c.a <= 0f)
        {
            ObjectPooler.ReturnToPool(gameObject);
        }
    }
}
