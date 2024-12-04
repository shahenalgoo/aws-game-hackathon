using UnityEngine;
using UnityEngine.UI;

public class CircularMinimap : MonoBehaviour
{
    [SerializeField] private RawImage minimapImage;
    [SerializeField] private Color backgroundColor = Color.black;
    [SerializeField] private Color pathColor = Color.white;
    [SerializeField] private Color playerColor = Color.red;
    [SerializeField] private float mapSize = 216f;

    [SerializeField] private Transform player;  // Assign the player's transform in inspector
    private RawImage playerMarker;
    private Texture2D mapTexture;
    private int textureSize = 512;
    private Vector2Int gridDimensions;
    [SerializeField] private int cellSize;
    [SerializeField] private int gridStartingX;
    [SerializeField] private int offsetX, offsetY;
    private Vector2Int lastPlayerGridPos = new Vector2Int(-1, -1); // Track last position to avoid unnecessary updates

    public void Init(int[,] gridData)
    {
        gridDimensions = new Vector2Int(gridData.GetLength(0), gridData.GetLength(1));
        InitializeMinimapUI();
        CreateMapTexture(gridData);
        // CreatePlayerMarker();
    }

    private void InitializeMinimapUI()
    {
        RectTransform rectTransform = minimapImage.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(mapSize, mapSize);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.pivot = new Vector2(0, 0);
        rectTransform.anchoredPosition = new Vector2(0, 0);
    }

    // private void CreatePlayerMarker()
    // {
    //     GameObject markerObj = new GameObject("PlayerMarker");
    //     playerMarker = markerObj.AddComponent<RawImage>();
    //     playerMarker.transform.SetParent(minimapImage.transform, false);
    //     RectTransform rectTransform = playerMarker.GetComponent<RectTransform>();
    //     rectTransform.anchorMin = new Vector2(0, 0);
    //     rectTransform.anchorMax = new Vector2(0, 0);
    //     rectTransform.pivot = new Vector2(0, 0);

    //     // Create a small red square for the player marker
    //     Texture2D markerTexture = new Texture2D(4, 4);
    //     Color[] colors = new Color[16];
    //     for (int i = 0; i < colors.Length; i++)
    //     {
    //         colors[i] = Color.red;
    //     }
    //     markerTexture.SetPixels(colors);
    //     markerTexture.Apply();

    //     playerMarker.texture = markerTexture;

    //     RectTransform markerRect = playerMarker.GetComponent<RectTransform>();
    //     markerRect.sizeDelta = new Vector2(10, 10); // Size of marker on minimap
    //     markerRect.pivot = new Vector2(0.5f, 0.5f); // Center pivot point
    // }

    private void CreateMapTexture(int[,] gridData)
    {
        int rows = gridData.GetLength(0);    // 9 rows
        int cols = gridData.GetLength(1);    // 20 columns

        mapTexture = new Texture2D(textureSize, textureSize);
        mapTexture.filterMode = FilterMode.Point;

        // Calculate cell size to maintain square cells
        int cellSize = Mathf.FloorToInt(textureSize / Mathf.Max(rows, cols));

        // Calculate offsets to center the grid
        int offsetX = Mathf.FloorToInt((textureSize - (cellSize * cols)) * 0.5f);
        int offsetY = Mathf.FloorToInt((textureSize - (cellSize * rows)) * 0.5f);

        // Fill background
        for (int x = 0; x < textureSize; x++)
        {
            for (int y = 0; y < textureSize; y++)
            {
                mapTexture.SetPixel(x, y, backgroundColor);
            }
        }

        // Draw grid cells
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                int startX = offsetX + (col * cellSize);
                int startY = offsetY + ((rows - 1 - row) * cellSize);

                Color cellColor = gridData[row, col] == 0 ? backgroundColor : pathColor;

                // Draw cell
                for (int x = 0; x < cellSize; x++)
                {
                    for (int y = 0; y < cellSize; y++)
                    {
                        int pixelX = startX + x;
                        int pixelY = startY + y;

                        if (pixelX < textureSize && pixelY < textureSize)
                        {
                            mapTexture.SetPixel(pixelX, pixelY, cellColor);
                        }
                    }
                }
            }
        }

        mapTexture.Apply();
        minimapImage.texture = mapTexture;
    }

    private void Update()
    {
        if (player == null) return;

        // Convert world position to grid position
        // Vector2 gridPos = WorldToGridPosition(player.position);

        // Convert grid position to minimap position
        // Since the minimap image starts from bottom-left, we need to adjust Y accordingly
        // float minimapX = gridPos.x * (mapSize / 19f);  // Start at left (0) for column 0
        // float minimapY = mapSize / 2f - (gridPos.y - 4f) * (mapSize / 8f);  // Center at row 4
        // float minimapX = -4f * mapSize / 19f;
        // float minimapY = 0;
        // float minimapX = (gridPos.x / gridDimensions.y) * mapSize;
        // float minimapY = ((gridDimensions.x - gridPos.y - 1) / gridDimensions.x) * mapSize;
        // float minimapX = (gridPos.x / gridDimensions.y) * mapSize;
        // float minimapY = (gridPos.y / gridDimensions.x) * mapSize;
        // Convert grid position to minimap position with inverted coordinates
        // float minimapX = -gridPos.x * (mapSize / 19f);  // Invert X
        // float minimapY = (gridPos.y - 4f) * (mapSize / 8f);  // Invert Y offset from center

        // float minimapX = 0;
        // float minimapY = 0;


        // Update player marker position
        // RectTransform markerRect = playerMarker.GetComponent<RectTransform>();
        // markerRect.anchoredPosition = new Vector2(minimapX, minimapY);

        // Convert world position to grid position
        Vector2 gridPos = WorldToGridPosition(player.position);
        Vector2Int currentGridPos = new Vector2Int(Mathf.RoundToInt(gridPos.x), Mathf.RoundToInt(gridPos.y));

        // Only update if position changed
        if (currentGridPos != lastPlayerGridPos)
        {
            // Clear previous position if valid
            if (lastPlayerGridPos.x >= 0 && lastPlayerGridPos.y >= 0)
            {
                DrawGridCell(lastPlayerGridPos.y, lastPlayerGridPos.x, pathColor);
            }

            // Draw new position
            DrawGridCell(currentGridPos.y, currentGridPos.x, playerColor);
            mapTexture.Apply();

            lastPlayerGridPos = currentGridPos;
        }
    }

    private void DrawGridCell(int row, int col, Color color)
    {
        int startX = offsetX + (col * cellSize);
        int startY = offsetY + ((gridDimensions.x - 1 - row) * cellSize);

        for (int x = 0; x < cellSize - 1; x++)
        {
            for (int y = 0; y < cellSize - 1; y++)
            {
                int pixelX = startX + x;
                int pixelY = startY + y;

                if (pixelX < textureSize && pixelY < textureSize)
                {
                    mapTexture.SetPixel(pixelX, pixelY, color);
                }
            }
        }
    }

    private Vector2 WorldToGridPosition(Vector3 worldPos)
    {
        // Grid cell (4,0) center is at world position (28,0,0)
        // Each grid cell is 7x7 units
        // Need to offset by -28 on X to align with starting position

        float gridX = (worldPos.x - 28f) / 7f;  // Offset by starting position, then divide by cell size
        float gridY = 4f - (worldPos.z / 7f);   // Start at row 4, subtract normalized Z position


        return new Vector2(gridX, gridY);


    }
}
