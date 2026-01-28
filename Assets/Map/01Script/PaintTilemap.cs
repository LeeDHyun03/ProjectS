using UnityEngine;
using UnityEngine.Tilemaps;

public class PaintTilemap : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;

    [Header("맵 크기")]
    public int width = 100;
    public int height = 100;

    [SerializeField] private TileBase tileBase;

    private Vector3Int GetCentorOffset(int w, int h)
        => new Vector3Int(-(w / 2), -(h / 2), 0);

    private void Start()
    {
        if (tilemap == null)
        {
            Debug.LogError("Tilemap == null - PaintTile");
            return;
        }
        if (tileBase == null)
        {
            Debug.LogError("TileBase == null - PaintTile");
            return;
        }

        Paint(tileBase, width, height);
    }

    private void Paint(TileBase tile, int w, int h)
    {
        tilemap.ClearAllTiles();

        Vector3Int offset = GetCentorOffset(width, height);

        // BoundsInt = TIlemap 내 유효한 cell 영역을 나타낸다. 각 cell은 하나의 tile을 나타낸다.
        var bounds = new BoundsInt(offset, new Vector3Int(w, h, 1));

        var tiles = new TileBase[w * h];

        for (int i = 0; i < tiles.Length; i++)
            tiles[i] = tile;

        tilemap.SetTilesBlock(bounds, tiles);

    }

    // Possion 용 //
    public Vector3Int CentorOffset
        => new Vector3Int(-(width / 2), -(height / 2), 0);
}
