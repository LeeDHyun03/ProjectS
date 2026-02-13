using System;
using UnityEngine;
public class MCWater : PZInteraction
{
    public GameObject interactionText;
    Collider2D col;
    SpriteRenderer sr;
    bool isPlayerInteraction, CanInteraction;
    public event Action OnPutDownWater;
    public override void Awake()
    {
        base.Awake();
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (isPlayerInteraction)
        {
            MoveToPlayerVec();
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if(isPlayerInteraction)
            {
                isPlayerInteraction = false;
                col.enabled = true;
                sr.sortingOrder = 1;
                OnPutDownWater?.Invoke();
            }    
            else if (CanInteraction)
            {
                sr.sortingOrder = 2;
                isPlayerInteraction = true;
                col.enabled = false;
                Interaction(false);
            }
        }

    }
    public override void Interaction(bool enable)
    {
        interactionText.SetActive(enable);
        CanInteraction = enable;
    }
    public void MoveToPlayerVec()
    {
        if (((MCPuzzleManager)puzzleManager).isPast)
            transform.position = player.transform.position;
    }
}
