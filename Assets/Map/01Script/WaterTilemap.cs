using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class WaterTilemap : MonoBehaviour
{
    public static Tilemap tilemap;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Awake()
    {
        tilemap = GetComponent<Tilemap>();
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
