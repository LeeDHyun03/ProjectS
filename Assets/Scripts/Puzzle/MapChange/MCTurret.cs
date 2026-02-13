using System;
using UnityEngine;

public class MCTurret : PZInteraction
{
    [SerializeField]bool isLeft;
    bool isAttacking;
    const float attackDelay = 0.25f;
    const float onTurretDelay = 0.15f;
    float currentTime;
    [SerializeField] Vector3 bulletDir;
    public GameObject myBullet;
    public Sprite shotTurret, defaultTurret;
    SpriteRenderer sr;

    public event Action<Vector3> OnShotBullet;
    public override void Awake()
    {
        base.Awake();
        sr = GetComponent<SpriteRenderer>();
    }
    void Update()
    {
        if (isAttacking)
        {
            AttackRoutine();
        }
    }
    void AttackRoutine()
    {
        currentTime += Time.deltaTime;

        if (currentTime > attackDelay)
        {
            currentTime = 0;
            sr.sprite = defaultTurret;
            SoundManager.Instance.PlaySfx("MC_Cannon");
            myBullet.SetActive(true);
            OnShotBullet?.Invoke(RandomDir());
        }
        else if (currentTime > onTurretDelay&&
                sr.sprite !=shotTurret)
        {
            sr.sprite = shotTurret;            
        }
    }
    Vector3 RandomDir()
    {
        float xDir = isLeft ? -1f : 1f;
        float yDir = UnityEngine.Random.Range(-1f, 1f);

        bulletDir = new Vector3(xDir, yDir, 0f).normalized;

        return bulletDir;
    }

    public override void Interaction(bool enable)
    {
        isAttacking = enable;
    }
}
