using UnityEngine;

public class Arrow : MonoBehaviour, IPooledObject
{
    private float lifeTime = 5f;
    private float lifeTimer = 0f;
    private float speed = 10f;
    private float damage = 10f;
    private bool isPlayerSide = false;
    private Vector3 dir = Vector3.zero;

    bool isFire = false;

    public void SetDefaultValue(Vector3 newDir, float newLifeTime, float newSpeed, float newDamage, bool newIsPlayerSide)
    {
        dir = newDir;
        transform.rotation = Quaternion.LookRotation(Vector3.forward, dir);
        lifeTime = newLifeTime;
        speed = newSpeed;
        damage = newDamage;
        isPlayerSide = newIsPlayerSide;
        isFire = true;
    }

    public void OnObjectSpawn()
    {
        isPlayerSide = false;
        isFire = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.TryGetComponent<Character>(out Character character);
        if (character == null) return;
        if (isPlayerSide && character is PlayerCharacter) return;
        if (!isPlayerSide && character is Monster) return;

        character.TakeDamage(damage);
        ObjectPooler.ReturnToPool(gameObject);
    }

    private void OnEnable()
    {
        OnObjectSpawn();
    }

    private void Update()
    {
        if (!isFire) return;
        transform.position += dir * speed * Time.deltaTime;
        lifeTimer += Time.deltaTime;
        if(lifeTimer >= lifeTime)
        {
            lifeTimer = 0f;
            ObjectPooler.ReturnToPool(gameObject);
        }
    }
}
