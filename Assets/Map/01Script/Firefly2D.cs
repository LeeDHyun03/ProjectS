using UnityEngine;

public class Firefly2D : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 0.5f;
    public float moveRange = 1.2f;

    [Header("Blinking")]
    public bool useBlinking = true;
    public float blinkSpeed = 2f;

    [Header("Ref")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private UnityEngine.Rendering.Universal.Light2D light2D; // URP 사용 시
    [SerializeField] private UnityEngine.Rendering.Universal.Light2D light2D_mirror; // URP 사용 시

    private Vector2 startPos;
    private float seedX, seedY;
    
    
    void Start()
    {
        startPos = transform.position;
        seedX = Random.value * 100f;
        seedY = Random.value * 100f;
    }

    void Update()
    {
        // 1. 탑뷰 평면 이동 (Perlin Noise로 부드러운 유영)
        float x = (Mathf.PerlinNoise(seedX, Time.time * moveSpeed) * 2 - 1) * moveRange;
        float y = (Mathf.PerlinNoise(seedY, Time.time * moveSpeed) * 2 - 1) * moveRange;
        transform.position = startPos + new Vector2(x, y);

        // 2. 깜빡임 효과 (알파값이나 빛의 강도 조절)
        float alpha = (Mathf.Sin(Time.time * blinkSpeed) + 1f) / 2f; // 0~1 반복

        if (spriteRenderer != null)
        {
            Color c = spriteRenderer.color;
            c.a = alpha;
            spriteRenderer.color = c;
        }

        if (light2D != null)
        {
            light2D.intensity = alpha * 1.5f; // 빛 강도 조절
        }
        if(light2D_mirror != null)
        {
            light2D_mirror.intensity = alpha * 1.5f;
        }
    }
}