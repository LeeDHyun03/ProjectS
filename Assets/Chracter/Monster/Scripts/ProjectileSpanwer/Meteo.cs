using System.Collections;
using UnityEngine;

public class Meteo : MonoBehaviour, IPooledObject
{
    [Header("메테오 시작 위치")]
    [SerializeField] private float startHeight = 10f;
    [SerializeField] private float startWidth = 5f;

    [SerializeField] private LayerMask targetLayer;

    [SerializeField] private ParticleSystem fallEffect;
    [SerializeField] private ParticleSystem boomEffect;

    private Vector3 targetPos = Vector3.zero;
    private float damage = 10f;
    private float radius = 5f;
    private bool isPlayerSide;
    private float fallingTime;

    bool isFire = false;

    private Coroutine routine;

    public void SetDefaultValue(Vector3 targetPos, float damage, float radius, bool isPlayerSide, float fallingTime)
    {
        this.targetPos = targetPos;
        this.damage = damage;
        this.radius = radius;
        this.fallingTime = fallingTime;
        this.isPlayerSide = isPlayerSide;
        isFire = true;
    }

    public void OnObjectSpawn()
    {
        if(routine != null)
        {
            StopCoroutine(routine);
            routine = null;
        }
        isFire = false;


    }

    public void StartFall()
    {
        if (!isFire)
            return;

        Vector3 startOffset = new Vector3(startWidth, startHeight, 0f);
        transform.position = targetPos + startOffset;

        if (fallEffect != null)
            fallEffect.Play();

        routine = StartCoroutine(FallRoutine());
    }

    private IEnumerator FallRoutine()
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = targetPos;

        float t = 0f;
        while(t < 1f)
        {
            t += Time.deltaTime / fallingTime;
            transform.position = Vector3.LerpUnclamped(startPos, endPos, t);
            yield return null;
        }

        transform.position = endPos;

        if (boomEffect != null)
            boomEffect.Play();
        SfxManager.Instance.Play("Mage2");
        DoDamage();


        routine = null;
        ObjectPooler.ReturnToPool(gameObject);
    }

    private void DoDamage()
    {
        var hits = Physics2D.OverlapCircleAll(targetPos, radius, targetLayer);

        foreach(var col in hits)
        {
            if (!col.TryGetComponent<Character>(out var character))
                continue;
            if (isPlayerSide && character is PlayerCharacter)
                continue;
            if(!isPlayerSide && character is Monster)
                continue;

            character.TakeDamage(damage);
        }
    }

}
