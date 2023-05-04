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

    public int seed = 0;
    public double hotnessScale = 1;
    public double solidnessScale = 1;

    void Start()
    {
        mainCamera = Camera.main;
        cameraPosition = mainCamera.transform.position;

        initTileData();
        initTileMap();
        loadHeatmap();
    }

    void Update()
    {
        Vector3Int topLeftCorner = grid.WorldToCell(mainCamera.ScreenToWorldPoint(new Vector3(0, 0, cameraPosition.z)));
        Vector3Int bottomRightCorner = grid.WorldToCell(mainCamera.ScreenToWorldPoint(new Vector3(mainCamera.pixelWidth, mainCamera.pixelHeight, cameraPosition.z)));
        //TileBase tile = tilemap.GetTile(cellPosition);
        loadTiles(topLeftCorner, bottomRightCorner);
        unloadTiles(topLeftCorner, bottomRightCorner);

        renderedRect = new RectInt(topLeftCorner.x, topLeftCorner.y, bottomRightCorner.x - topLeftCorner.x, bottomRightCorner.y - topLeftCorner.y);
    }

    void initTileData()
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

    void initTileMap()
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

    void loadHeatmap()
    {
        Dictionary<string, int> colorToTileIndex = new Dictionary<string, int>();
        for(int i = 0; i < tileData.Count; i++)
        {
            colorToTileIndex.Add(tileData[i].color, i);
        }

        Texture2D texture = Resources.Load<Texture2D>("Textures/Heatmap.png");
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

    void unloadTiles(Vector3Int topLeftCorner, Vector3Int bottomRightCorner)
    {
        foreach(Tilemap tilemap in tilemaps) {
            Bounds bounds = tilemap.localBounds;

            /*
            for (int j = (int)bounds.min.y; j < topLeftCorner.y - 1; j++)
            {
                for (int i = (int) bounds.min.x; i < topLeftCorner.x - 1; i++)
                {
                    tilemap.SetTile(new Vector3Int(i, j, (int)cameraPosition.z), null);
                }
            }

            for (int j = bottomRightCorner.y + 1; j <= bounds.max.y; j++)
            {
                for (int i = bottomRightCorner.x + 1; i <= bounds.max.x; i++)
                {
                    tilemap.SetTile(new Vector3Int(i, j, (int)cameraPosition.z), null);
                }
            }
            */

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

    void loadTiles(Vector3Int topLeftCorner, Vector3Int bottomRightCorner)
    {
        /*
        for (int j = topLeftCorner.y - 1; j < renderedRect.y; j++)
        {
            for (int i = topLeftCorner.x - 1; i < renderedRect.x; i++)
            {
                int tileIndex = getTileIndex(i, j);
                tilemaps[tileIndex].SetTile(new Vector3Int(i - 1, j - 1, (int)cameraPosition.z), tileData[tileIndex].tile);
                tilemaps[tileIndex].SetTile(new Vector3Int(i - 1, j, (int)cameraPosition.z), tileData[tileIndex].tile);
                tilemaps[tileIndex].SetTile(new Vector3Int(i, j - 1, (int)cameraPosition.z), tileData[tileIndex].tile);
                tilemaps[tileIndex].SetTile(new Vector3Int(i, j, (int)cameraPosition.z), tileData[tileIndex].tile);
            }
        }

        for (int j = renderedRect.y + renderedRect.height; j < bottomRightCorner.y + 2; j++)
        {
            for (int i = renderedRect.x + renderedRect.width; i < bottomRightCorner.x + 2; i++)
            {
                int tileIndex = getTileIndex(i, j);
                tilemaps[tileIndex].SetTile(new Vector3Int(i - 1, j - 1, (int)cameraPosition.z), tileData[tileIndex].tile);
                tilemaps[tileIndex].SetTile(new Vector3Int(i - 1, j, (int)cameraPosition.z), tileData[tileIndex].tile);
                tilemaps[tileIndex].SetTile(new Vector3Int(i, j - 1, (int)cameraPosition.z), tileData[tileIndex].tile);
                tilemaps[tileIndex].SetTile(new Vector3Int(i, j, (int)cameraPosition.z), tileData[tileIndex].tile);
            }
        }*/
        for (int j = topLeftCorner.y - 1; j < bottomRightCorner.y + 2; j++)
        {
            for (int i = topLeftCorner.x - 1; i < bottomRightCorner.x + 2; i++)
            {
                if (i >= renderedRect.x && i < renderedRect.x + renderedRect.width && j >= renderedRect.y && j < renderedRect.y + renderedRect.height)
                    continue;

                int tileIndex = getTileIndex(i, j);

                tilemaps[tileIndex].SetTile(new Vector3Int(i - 1, j - 1, (int)cameraPosition.z), tileData[tileIndex].tile);
                tilemaps[tileIndex].SetTile(new Vector3Int(i - 1, j, (int)cameraPosition.z), tileData[tileIndex].tile);
                tilemaps[tileIndex].SetTile(new Vector3Int(i, j - 1, (int)cameraPosition.z), tileData[tileIndex].tile);
                tilemaps[tileIndex].SetTile(new Vector3Int(i, j, (int)cameraPosition.z), tileData[tileIndex].tile);
            }
        }
    }

    int getTileIndex(int x, int y)
    {
        int newX = ((x / 5) % tilemaps.Length + 5) % tilemaps.Length;
        int newY = ((y / 5) % tilemaps.Length + 5) % tilemaps.Length;
        return (newX + newY) % tilemaps.Length;
    }

    /*
    // Width and height of the texture in pixels.
    public int pixWidth;
    public int pixHeight;

    // The origin of the sampled area in the plane.
    public float xOrg;
    public float yOrg;

    // The number of cycles of the basic noise pattern that are repeated
    // over the width and height of the texture.
    public float scale = 1.0F;

    private Texture2D noiseTex;
    private Color[] pix;
    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();

        // Set up the texture and a Color array to hold pixels during processing.
        noiseTex = new Texture2D(pixWidth, pixHeight);
        pix = new Color[noiseTex.width * noiseTex.height];
        rend.material.mainTexture = noiseTex;
    }

    void CalcNoise()
    {
        // For each pixel in the texture...
        float y = 0.0F;

        while (y < noiseTex.height)
        {
            float x = 0.0F;
            while (x < noiseTex.width)
            {
                float xCoord = xOrg + x / noiseTex.width * scale;
                float yCoord = yOrg + y / noiseTex.height * scale;
                float sample = Mathf.PerlinNoise(xCoord, yCoord);
                pix[(int)y * noiseTex.width + (int)x] = new Color(sample, sample, sample);
                x++;
            }
            y++;
        }

        // Copy the pixel data to the texture and load it into the GPU.
        noiseTex.SetPixels(pix);
        noiseTex.Apply();
    }

    void Update()
    {
        CalcNoise();
    }*/
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
