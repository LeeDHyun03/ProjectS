using UnityEngine;

public class KBCart : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {

    }
    void CalculateKnockback(Vector3 attackDir)
    {
/*        Vector3 currentDir = GetSnappedDirection(attackDir);
        float moveDist = 1.0f; // 한 칸 단위 이동

        // 벽 감지
        RaycastHit2D hit = Physics2D.Raycast(transform.position, currentDir, moveDist, LayerMask.GetMask("Obstacle"));

        if (hit.collider != null)
        {
            // [반사각 로직] 부딪힌 면의 법선(hit.normal)을 기준으로 반사 방향 계산
            Vector3 reflectDir = Vector3.Reflect(currentDir, hit.normal);
            nextTileVec = puzzleGrid.GetCellCenterWorld(currentCell + Vector3Int.RoundToInt(reflectDir));
        }
        else
        {
            nextTileVec = puzzleGrid.GetCellCenterWorld(currentCell + Vector3Int.RoundToInt(currentDir));
        }
        isMove = true;*/
    }
}
