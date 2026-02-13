using UnityEngine;

public abstract class PZInteraction : PZElement
{
    public GameObject player;
    public override void Awake()
    {
        base.Awake();
        player = GameObject.FindGameObjectWithTag("Player");
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Interaction(true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Interaction(false);
        }
    }
    public abstract void Interaction(bool enable);
}
