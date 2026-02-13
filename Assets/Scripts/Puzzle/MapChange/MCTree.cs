using System;
using UnityEngine;

public class MCTree : PZInteraction
{
    public enum status
    { 
        Live,
        Die,
        Bridge
    }

    private status _myStatus;
    public status MyStatus
    {
        get => _myStatus;
        set
        {
            _myStatus = value;
            switch (value)
            { 
                case status.Live:
                    sr.sprite = livingSprite;
                    sr.sortingOrder = 1;
                    col.enabled = true;
                    break;
                
                case status.Die:
                    sr.sprite = deadSprite;
                    sr.sortingOrder = 1;
                    col.enabled = false;
                    break;
                
                case status.Bridge:
                    col.enabled = false;
                    sr.sortingOrder = 0;
                    //SoundManager.Instance.PlaySfx("MC_FallTree");
                    BridgeFormChange(true);
                    break;
            }
        }
    }
    public MCSeed mySeed;
    public Sprite livingSprite, deadSprite, bridgeSprite;
    Collider2D col;
    public GameObject wallCollider;
    public GameObject cuttingText;
    public GameObject cuttingTreeObj;
    
    private SpriteRenderer sr;
    private bool isCanTreeCutting;
    private void OnEnable()
    {
        mySeed.OnGrewTrigger += StatusChange;
    }

    public override void Awake()
    {
        base.Awake();
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
    }
    private void Start()
    {
        StatusChange(false);
    }

    void Update()
    {
        if(isCanTreeCutting && MyStatus == status.Live && Input.GetKeyDown(KeyCode.Z))
        {
            MyStatus = status.Bridge;
            Interaction(false);
        }
    }

    void StatusChange(bool stat)
    {
        if (stat)
        {
            MyStatus = status.Live;
        }
        else
        {
            MyStatus = status.Die;
            BridgeFormChange(false);
        }
    }
    void BridgeFormChange(bool changed)
    {
        sr.sprite = changed ? bridgeSprite : deadSprite;
        wallCollider.SetActive(!changed);
        cuttingTreeObj.SetActive(changed);
    }

    public override void Interaction(bool enable)
    {
        isCanTreeCutting = enable;
        cuttingText.SetActive(enable);
    }

    private void OnDisable()
    {
        mySeed.OnGrewTrigger -= StatusChange;
    }
}
