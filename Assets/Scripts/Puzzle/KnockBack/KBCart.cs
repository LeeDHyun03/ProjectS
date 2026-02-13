using System;
using UnityEngine;

public class KBCart : MonoBehaviour
{
    [SerializeField] Vector2 startVec;
    public event Action OnReAct;
    public Vector2 currentDir;
    const int defualtDurability = 4;
    float moveSpeed = 15f;
    float currentMoveSpeed;
    int _durability;
    public int Durability
    {
        get=> _durability;
        set
        {
            _durability = value;
            if(Durability <=0)
            {
                CartReset();
            }
        }
    }
    private void Start()
    {
        currentMoveSpeed = moveSpeed;
        Durability = defualtDurability;
        startVec = transform.position;
    }
    private void FixedUpdate()
    {
        if (Durability <= 0)
            return;
        
        if(currentMoveSpeed <=0f)
        {
            currentDir = Vector2.zero;
            currentMoveSpeed = moveSpeed;
            OnReAct?.Invoke();
            return;
        }

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

        transform.Translate(currentMoveSpeed * Time.deltaTime * currentDir);
        currentMoveSpeed -= 0.3f;
    }
    public void CartReset()
    {
        currentDir = Vector2.zero;
        transform.position = startVec;
        Durability = defualtDurability;
        OnReAct?.Invoke();
    }
}
