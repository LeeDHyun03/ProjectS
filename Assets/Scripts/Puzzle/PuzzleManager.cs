using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class PuzzleManager : MonoBehaviour
{
    public Transform player;
    protected int difficulty;
    public virtual void Awake()
    {
        PuzzleDataManager.Instance?.SetCurrentManager(this);
        SetPuzzleLevel(2);  //юс╫ц
    }
    public void GiveReward(int difficulty)
    {

    }
    public void Clear()
    {
        Debug.Log("Clear");
        GiveReward(difficulty);
    }
    public void SetPuzzleLevel(int level)
    {
        difficulty = level;
        Init(level);
        Debug.Log(level+" start");
    }
    public void PuzzleReset()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
    public abstract void Init(int level);
    private void OnEnable()
    {
        player.GetComponent<PZPlayer>().OnClear += Clear;
    }
    private void OnDisable()
    {
        player.GetComponent<PZPlayer>().OnClear -= Clear;
    }
}
