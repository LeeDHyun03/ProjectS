using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class SceneFlowManager : MonoBehaviour
{
    public static SceneFlowManager Instance { get; private set; }

    public enum GameScene
    {
        Title,
        Field,
        KnockBack,
        MapChange,
        Minesweeper,
        ObjectActivation
    }

    [Header("Scene Names")]
    [SerializeField] private string titleScene = "Title";
    [SerializeField] private string fieldScene = "Field";
    [SerializeField] private string knockBackScene = "KnockBack";
    [SerializeField] private string mapChangeScene = "MapChange";
    [SerializeField] private string minesweeperScene = "Minesweeper";
    [SerializeField] private string objectActivationScene = "ObjectActivation";

    private int lastEnteredPortalId = -1;
    private bool inPuzzle = false;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StartGame() => SceneManager.LoadScene(fieldScene);

    public void EnterPuzzleFromPortal(GameScene puzzleScene, int portalId)
    {
        if (!IsPuzzle(puzzleScene))
        {
            Debug.LogWarning("EnterPuzzleFromPortal: not a puzzle scene.");
            return;
        }

        lastEnteredPortalId = portalId;
        inPuzzle = true;

        // 퍼즐 진입 전 상태 저장(시간 등)
        if (GameManager.Instance != null)
            GameManager.Instance.SaveCurrentStateBeforeEnterPuzzle();

        SceneManager.LoadScene(GetSceneName(puzzleScene));
    }

    /// <summary>
    /// 퍼즐에서 Field로 복귀. clear=true면 "방금 사용한 포탈 비활성화 + 비활성 중 1개 활성화" 교체 수행.
    /// </summary>
    public void ExitPuzzleToField(bool clear)
    {
        // 퍼즐 클리어 처리(씬 로드 전에 처리해도 되고 후에 처리해도 됨)
        if (clear && lastEnteredPortalId >= 0)
        {
            var portalMgr = FindFirstObjectByType<PortalManager>();
            if (portalMgr != null)
                portalMgr.OnPuzzleCleared(lastEnteredPortalId);
        }

        inPuzzle = false;
        SceneManager.LoadScene(fieldScene);
    }

    private bool IsPuzzle(GameScene scene)
    {
        return scene == GameScene.KnockBack
            || scene == GameScene.MapChange
            || scene == GameScene.Minesweeper
            || scene == GameScene.ObjectActivation;
    }

    private string GetSceneName(GameScene scene)
    {
        return scene switch
        {
            GameScene.Title => titleScene,
            GameScene.Field => fieldScene,
            GameScene.KnockBack => knockBackScene,
            GameScene.MapChange => mapChangeScene,
            GameScene.Minesweeper => minesweeperScene,
            GameScene.ObjectActivation => objectActivationScene,
            _ => fieldScene
        };
    }
}