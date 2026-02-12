using System;
using UnityEngine;

public class MCBullet : PuzzleElement
{
    public MCTurret myTurret;
    [SerializeField] float moveSpeed = 7.55f;
    Vector3 moveDir;
    public event Action OnTriggerEnterBullet;
    bool isCanMove;
    public override void Awake()
    {
        base.Awake();
        myTurret = transform.parent.GetComponent<MCTurret>();
    }
    private void OnEnable()
    {
        myTurret.OnShotBullet += ShotToDir;
    }

    void Update()
    {
        if (isCanMove)
        {
            transform.Translate(moveDir * Time.deltaTime * moveSpeed);
        }
    }
    void ShotToDir(Vector3 dir)
    {
        transform.position = myTurret.transform.position;
        moveDir = dir;
        isCanMove = true;
    }
    private void OnDisable()
    {
        myTurret.OnShotBullet -= ShotToDir;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isCanMove = false;
            OnTriggerEnterBullet?.Invoke();
            gameObject.SetActive(false);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.name == "Turret")
        {
            gameObject.SetActive(false);
        }
    }
}
