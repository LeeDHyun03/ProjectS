using System;
using UnityEngine;
using TMPro;

public class MSMineChecking : MonoBehaviour
{
    [SerializeField]Grid puzzleGrid;
    public Transform player;
    public TextMeshPro mineCountText;

    [SerializeField]bool isPlayerMove = true;
    Collider2D col2D;
    Vector3Int playerCellVec;
    int mineCount = 0;

    public event Action OnMineTriggered;
    private void Awake()
    {
        col2D = GetComponent<Collider2D>();
    }
    private void Start()
    {
    }
    void Update()
    {
        if (IsCellVecDifferent())
        {
            MoveToPlayerVector();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("MSMine"))
        {
            mineCount++;
            SetMineCountText();

            if(isPlayerOnCollision(collision))
            {
                OnMineTriggered?.Invoke();
                //SoundManager.Instance.PlaySfx("MS_Explosion");
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    { 
        mineCount = 0;
        SetMineCountText();
    }
    void MoveToPlayerVector()
    {
        if(puzzleGrid !=null)
        {
            col2D.enabled = false;
            Vector3 moveVec = puzzleGrid.GetCellCenterWorld(playerCellVec);
            transform.position = moveVec;
            col2D.enabled = true;
        }
    }
    void SetMineCountText()
    {
        if(mineCount == 0)
        {
            mineCountText.text = "";
            return;
        }
        string text = mineCount.ToString();

        mineCountText.text = text;
    }
    bool IsCellVecDifferent()
    {
        if(isPlayerMove && puzzleGrid != null)
        {
            Vector3Int currentCellVec = puzzleGrid.WorldToCell(transform.position);
            playerCellVec = puzzleGrid.WorldToCell(player.position);
            return currentCellVec != playerCellVec;
        }
        return default;
    }
    public void SetPuzzleGrid(Grid grid)
    {
        puzzleGrid = grid;
    }
    public bool isPlayerOnCollision(Collider2D col)
    {
        return col.transform.position == transform.position;
    }
}
