using System;
using TMPro;
using UnityEngine;
public class MCWater : PZInteraction
{
    public TextMeshPro interactionText;
    Collider2D col;
    SpriteRenderer sr;
    bool isPlayerInteraction, CanInteraction;
    public event Action OnPutDownWater;

    Vector3 offset = new Vector3(0f, 0.5f, 0f);

    string pickUp = "들기\n< Z >";
    string putDown = "내려놓기\n< Z >";

    public override void Awake()
    {
        base.Awake();
        interactionText = GetComponentInChildren<TextMeshPro>();
        interactionText.gameObject.SetActive(false);
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
                interactionText.text = pickUp;

                isPlayerInteraction = false;
                col.enabled = true;
                transform.position = player.transform.position;
                OnPutDownWater?.Invoke();
                //SoundManager.Instance.PlaySfx("MC_PickUp");
            }
            else if (CanInteraction)
            {
                interactionText.text = putDown;

                isPlayerInteraction = true;
                col.enabled = false;
                CanInteraction = false;
                //SoundManager.Instance.PlaySfx("MC_PutDown");
            }
        }
    }
    public override void Interaction(bool enable)
    {
        interactionText.gameObject.SetActive(enable);
        CanInteraction = enable;
    }
    public void MoveToPlayerVec()
    {
        if (((MCPuzzleManager)puzzleManager).isPast)
            transform.position = player.transform.position + offset;
    }
}
