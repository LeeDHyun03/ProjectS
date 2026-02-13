using UnityEngine;

public class PZResetUI : PZElement
{
    Animator anim;
    bool mapChanged = false;
    [SerializeField]bool isFading = false;
    public override void Awake()
    {
        base.Awake();
        anim = GetComponent<Animator>();
        if (puzzleManager is MCPuzzleManager)
        {
            mapChanged = true;
        }
    }
    private void Update()
    {
        if (!isFading && Input.GetKeyDown(KeyCode.R))
        {
            anim.SetTrigger("Fade");
        }

        if (mapChanged && Input.GetKeyDown(KeyCode.T))
        {
            ((MCPuzzleManager)puzzleManager).ChangedMap();
        }
    }
    public void FadeInOut()
    {
        puzzleManager.PuzzleReset();
    }
    public void FadingStop()
    {
        isFading = false;
    }
    public void FadingStart()
    {
        isFading = true;
    }
}