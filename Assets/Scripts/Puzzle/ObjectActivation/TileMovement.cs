using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMovement : MonoBehaviour
{
    [Header("필수 할당 요소")]
    public Grid puzzleGrid;
    public Transform player;
    bool isMove = false;
    Vector3Int currentCell => puzzleGrid.WorldToCell(transform.position);
    Vector3 nextTileVec;

    void Update()
    {
        if(isMove)
        {
            if ((transform.position - nextTileVec).magnitude<0.05f)
            {
                transform.position = nextTileVec;
                isMove = false;
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, nextTileVec, Time.deltaTime*6f);
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isMove)
            return; 
        if (collision.CompareTag("Player"))
        {
            var dir = PlayerMoveDir();

            if (IsWallAhead(dir))
                return;

            nextTileVec = puzzleGrid.GetCellCenterWorld(NextTileVec(dir));
            isMove = true;
        }
    }

    Vector3Int NextTileVec(Vector3 dir)
    {
        Vector3Int gridDir = Mathf.Abs(dir.x) < Mathf.Abs(dir.y) ?
            new Vector3Int(0, Mathf.RoundToInt(dir.y), 0) :
            new Vector3Int(Mathf.RoundToInt(dir.x), 0, 0);

        Vector3Int nextCell = currentCell + gridDir;

        return nextCell;
    }
    Vector3 PlayerMoveDir()
    {
        return (transform.position- player.position).normalized;
    }
    bool IsWallAhead(Vector3 dir)
    {
        float checkDistance = 0.8f;

        Vector3 rayStart = transform.position + (dir * 0.6f);
        
        RaycastHit2D hit = Physics2D.Raycast(rayStart, dir, checkDistance);

        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("PZObstacle"))
            {
                return true;
            }
        }

        return false;
    }
}
