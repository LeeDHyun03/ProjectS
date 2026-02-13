using UnityEngine;
public class MCWater : PZInteraction
{
    public GameObject interactionText;
    Collider2D col;
    bool isPlayerInteraction, CanInteraction;
    public override void Awake()
    {
        base.Awake();
        col = GetComponent<Collider2D>();
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
            }    
            else if (CanInteraction)
            {
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
