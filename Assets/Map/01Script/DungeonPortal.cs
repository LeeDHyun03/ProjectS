using UnityEngine;

public class DungeonPortal : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private SpriteRenderer portalEntrance;
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private PortalEntranceUI portalEntranceUI;
    [SerializeField] private GameObject Entrance;

    [Header("Portal Id")]
    [SerializeField] private int portalId = -1;

    private bool isActivatePortal = false;

    private bool isAlreadyEnterScene = false;

    public struct PuzzleInfo
    {
        public SceneFlowManager.GameScene puzzleScene;
        public string puzzleName;
        public string puzzleDifficult;
    }

    private PuzzleInfo puzzleInfo;

    public int PortalId => portalId;

    public void SetPuzzleInfo(SceneFlowManager.GameScene puzzle, string name, string difficult)
    {
        puzzleInfo = new PuzzleInfo
        {
            puzzleScene = puzzle,
            puzzleName = name,
            puzzleDifficult = difficult
        };

        if (portalEntranceUI != null)
            portalEntranceUI.SetPuzzleInfo(name, difficult);
    }

    public void SetActivePortal(bool active)
    {
        isActivatePortal = active;
        if (portalEntrance != null) portalEntrance.enabled = isActivatePortal;
        if (boxCollider != null) boxCollider.enabled = isActivatePortal;

        if (portalEntranceUI != null)
            portalEntranceUI.gameObject.SetActive(false);
        if (Entrance != null) Entrance.SetActive(isActivatePortal);
        isAlreadyEnterScene = false;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isAlreadyEnterScene) return;

        // TODO: 비활할것
        // if (!GameManager.Instance.CanReceiveNightBenefit()) return;

        if (!collision.TryGetComponent<PlayerCharacter>(out var character)) return;
        if (!character.isInteracting) return;

        isAlreadyEnterScene = true;

        SceneFlowManager.Instance.EnterPuzzleFromPortal(puzzleInfo.puzzleScene, portalId);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isActivatePortal) return;
        collision.TryGetComponent<PlayerCharacter>(out var character);
        if (!character) return;
        if (portalEntranceUI != null)
            portalEntranceUI.gameObject.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!isActivatePortal) return;
        collision.TryGetComponent<PlayerCharacter>(out var character);
        if (!character) return;
        if (portalEntranceUI != null)
            portalEntranceUI.gameObject.SetActive(false);
    }
}