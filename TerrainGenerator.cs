using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TerrainGenerator : MonoBehaviour
{

    public RectInt renderedRect = new RectInt(0, 0, 0, 0);
    Camera mainCamera;
    Vector3 cameraPosition;
    public Grid grid;

    public GameObject heatmap;
    List<Tilemap> tilemaps = new List<Tilemap>();
    Dictionary<TileBase, int> tileToIndex = new Dictionary<TileBase, int>();
    List<TileBase> tiles = new List<TileBase>();
    int[,] tileHeatmap;

    public int worldLoadPadding = 10;

    public int hotnessSeed = 0;
    public int solinessSeed = 0;
    public float hotnessScale = 1;
    public float solidnessScale = 1;

    public int seedRange = 1000000;

    void Start()
    {
        mainCamera = Camera.main;
        cameraPosition = mainCamera.transform.position;

        if(hotnessSeed == 0)
            hotnessSeed = Random.Range(-seedRange, seedRange);
        if(solinessSeed == 0)
            solinessSeed = Random.Range(-seedRange, seedRange);

        LoadHeatmap();
    }

    void Update()
    {
        Vector3Int topLeftCorner = grid.WorldToCell(mainCamera.ScreenToWorldPoint(new Vector3(0, 0, cameraPosition.z)));
        Vector3Int bottomRightCorner = grid.WorldToCell(mainCamera.ScreenToWorldPoint(new Vector3(mainCamera.pixelWidth, mainCamera.pixelHeight, cameraPosition.z)));
        
        LoadTiles(topLeftCorner, bottomRightCorner);
        UnloadTiles(topLeftCorner, bottomRightCorner);

        renderedRect = new RectInt(topLeftCorner.x, topLeftCorner.y, bottomRightCorner.x - topLeftCorner.x, bottomRightCorner.y - topLeftCorner.y);
    }

    int AddTilemap(string name)
    {
        GameObject obj = new GameObject(name);
        obj.transform.parent = grid.transform;

        tilemaps.Add(obj.AddComponent<Tilemap>());
        TilemapRenderer tilemapRenderer = obj.AddComponent<TilemapRenderer>();
        tilemapRenderer.sortingOrder = tilemaps.Count - 1;

        return tilemaps.Count - 1;
    }

    void LoadHeatmap()
    {
        Tilemap heatTiles = heatmap.GetComponentInChildren<Tilemap>();
        heatTiles.CompressBounds();
        BoundsInt bounds = heatTiles.cellBounds;

        int width = bounds.max.x - bounds.min.x;
        int height = bounds.max.y - bounds.min.y - 1;

        for(int i = bounds.min.x; i <= bounds.max.x; i++) {
            TileBase tile = heatTiles.GetTile(new Vector3Int(i, bounds.max.y - 1, 0));
            if (tile == null)
                continue;

            int index = AddTilemap(tile.name);
            tileToIndex.Add(tile, index);
            tiles.Add(tile);

        }


        tileHeatmap = new int[width, height];

        for(int j = 0; j < height; j++) {
            for(int i = 0; i < width; i++) {
                TileBase tile = heatTiles.GetTile(new Vector3Int(bounds.min.x + i, bounds.min.y + j, 0));
                if (tile == null)
                    continue;

                tileHeatmap[i, j] = tileToIndex[tile];
            }
        }
    }

    void UnloadTiles(Vector3Int topLeftCorner, Vector3Int bottomRightCorner)
    {
        foreach(Tilemap tilemap in tilemaps) {
            Bounds bounds = tilemap.localBounds;

            for(int j = (int) bounds.min.y; j <= bounds.max.y; j++)
            {
                for(int i = (int) bounds.min.x; i <= bounds.max.x; i++)
                {
                    if (i >= topLeftCorner.x - 1 && i <= bottomRightCorner.x + 1 && j >= topLeftCorner.y - 1 && j <= bottomRightCorner.y + 1)
                        continue;

                    tilemap.SetTile(new Vector3Int(i, j, (int)cameraPosition.z), null);
                }
            }

            tilemap.CompressBounds();
        }
    }

    void LoadTiles(Vector3Int topLeftCorner, Vector3Int bottomRightCorner)
    {
        for (int j = topLeftCorner.y - 1 - worldLoadPadding; j < bottomRightCorner.y + 2 + worldLoadPadding; j++)
        {
            for (int i = topLeftCorner.x - 1 - worldLoadPadding; i < bottomRightCorner.x + 2 + worldLoadPadding; i++)
            {
                if (i >= renderedRect.x && i < renderedRect.x + renderedRect.width && j >= renderedRect.y && j < renderedRect.y + renderedRect.height)
                    continue;

                int tileIndex = GetTileIndex(i, j);

                tilemaps[tileIndex].SetTile(new Vector3Int(i - 1, j - 1, (int)cameraPosition.z), tiles[tileIndex]);
                tilemaps[tileIndex].SetTile(new Vector3Int(i - 1, j, (int)cameraPosition.z), tiles[tileIndex]);
                tilemaps[tileIndex].SetTile(new Vector3Int(i, j - 1, (int)cameraPosition.z), tiles[tileIndex]);
                tilemaps[tileIndex].SetTile(new Vector3Int(i, j, (int)cameraPosition.z), tiles[tileIndex]);
            }
        }
    }

    int GetTileIndex(int x, int y)
    {
        float hotPerVal = Mathf.Clamp(Mathf.PerlinNoise((hotnessSeed + x) * hotnessScale, (hotnessSeed + y) * hotnessScale), 0, 1);
        float solPerVal = Mathf.Clamp(Mathf.PerlinNoise((solinessSeed + x) * solidnessScale, (solinessSeed + y) * solidnessScale), 0, 1);
        
        return tileHeatmap[(int) (hotPerVal * (tileHeatmap.GetLength(0) - 1)), (int) (solPerVal * (tileHeatmap.GetLength(1) - 1))];
    }

}