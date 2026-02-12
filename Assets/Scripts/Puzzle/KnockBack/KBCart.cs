using System;
using UnityEngine;

public class KBCart : PuzzleElement
{
    [SerializeField] Vector2 startVec;
    public event Action OnRestart;
    public Vector2 currentDir;
    const int defualtDurability = 4;
    public float moveSpeed = 5f;
    int _durability;
    public int Durability
    {
        get=> _durability;
        set
        {
            _durability = value;
            if(Durability <=0)
            {
                OnRestart?.Invoke();
                CartReset();
            }
        }
    }
    private void Start()
    {
        Durability = defualtDurability;
        startVec = transform.position;
    }
    private void Update()
    {
        if (Durability <= 0)
            return;

        MoveToMoveVec();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.name =="Log")
        {
            Durability--;
        }

        Vector3 wallNormal = collision.contacts[0].normal;

        Vector3 reflectDir = Vector3.Reflect(currentDir, wallNormal);

        currentDir = reflectDir;
    }
    void MoveToMoveVec()
    {
        if (currentDir == Vector2.zero)
            return;

        transform.Translate(moveSpeed * Time.deltaTime * currentDir);
    }
    void CartReset()
    {
        currentDir = Vector2.zero;
        transform.position = startVec;
        Durability = defualtDurability;
    }
}
