using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class TerrainGenerator : MonoBehaviour
{

    public RectInt renderedRect = new RectInt(0, 0, 0, 0);
    Camera mainCamera;
    Vector3 cameraPosition;
    public Grid grid;
    Tilemap[] tilemaps;

    List<TileData> tileData;

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

        hotnessSeed = Random.Range(-seedRange, seedRange);
        solinessSeed = Random.Range(-seedRange, seedRange);

        InitTileData();
        InitTileMap();
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

    void InitTileData()
    {
        tileData = new List<TileData>()
        {
            new TileData("LavaCave", "ED7D31"),
            new TileData("Cave", "808080"),
            new TileData("Desert", "FFFF00"),
            new TileData("Field", "92D050"),
            new TileData("Winter", "00B0F0"),
        };
    }

    void InitTileMap()
    {
        tilemaps = new Tilemap[tileData.Count];
        for(int i = 0; i < tileData.Count; i++)
        {
            TileData data = tileData[i];

            GameObject obj = new GameObject(data.tileName);
            obj.transform.parent = grid.transform;

            tilemaps[i] = obj.AddComponent<Tilemap>();
            TilemapRenderer tilemapRenderer = obj.AddComponent<TilemapRenderer>();
            tilemapRenderer.sortingOrder = i;
        }
    }

    void LoadHeatmap()
    {
        Dictionary<string, int> colorToTileIndex = new Dictionary<string, int>();
        for(int i = 0; i < tileData.Count; i++)
        {
            colorToTileIndex.Add(tileData[i].color, i);
        }

        Texture2D texture = Resources.Load<Texture2D>("Textures/Heatmap");
        int width = texture.width;
        int height = texture.height;
        tileHeatmap = new int[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tileHeatmap[x, y] = colorToTileIndex[ColorUtility.ToHtmlStringRGB(texture.GetPixel(x, y))];
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

                tilemaps[tileIndex].SetTile(new Vector3Int(i - 1, j - 1, (int)cameraPosition.z), tileData[tileIndex].tile);
                tilemaps[tileIndex].SetTile(new Vector3Int(i - 1, j, (int)cameraPosition.z), tileData[tileIndex].tile);
                tilemaps[tileIndex].SetTile(new Vector3Int(i, j - 1, (int)cameraPosition.z), tileData[tileIndex].tile);
                tilemaps[tileIndex].SetTile(new Vector3Int(i, j, (int)cameraPosition.z), tileData[tileIndex].tile);
            }
        }
    }

    int GetTileIndex(int x, int y)
    {
        float hotPerVal = Mathf.PerlinNoise((hotnessSeed + x) * hotnessScale, (hotnessSeed + y) * hotnessScale);
        float solPerVal = Mathf.PerlinNoise((solinessSeed + x) * solidnessScale, (solinessSeed + y) * solidnessScale);
        
        return tileHeatmap[(int) (hotPerVal * tileHeatmap.GetLength(0)), (int) (solPerVal * tileHeatmap.GetLength(1))];
    }

}

class TileData
{
    public readonly string tileName;
    public readonly string color;
    public readonly RuleTile tile;

    public TileData(string tileName, string color)
    {
        this.tileName = tileName;
        this.color = color;
        this.tile = Resources.Load<RuleTile>("Tiles/" + tileName + "RuleTile");
    }
}
