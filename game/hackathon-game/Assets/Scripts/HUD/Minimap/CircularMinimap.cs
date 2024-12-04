using UnityEngine;
using UnityEngine.UI;

public class CircularMinimap : MonoBehaviour
{
    [SerializeField] private RawImage minimapImage;
    [SerializeField] private RawImage playerMarker;
    [SerializeField] private float mapSize = 216f;
    [SerializeField] private Transform player;
    [SerializeField] private Color backgroundColor = Color.black;
    [SerializeField] private Color pathColor = Color.white;

    private Texture2D mapTexture;
    private int textureSize = 512; // Increased for better resolution
    private float cellSize; // Size of each cell in texture space
    private Vector2Int gridDimensions;
    private RectTransform minimapRect;
    private RectTransform markerRect;

    public void Init(int[,] gridInstructions, float worldCellSize)
    {
        gridDimensions = new Vector2Int(gridInstructions.GetLength(0), gridInstructions.GetLength(1));
        InitializeMinimapUI();
        CreateMapTexture(gridInstructions);
        InitializePlayerMarker();
    }

    private void InitializeMinimapUI()
    {
        minimapRect = minimapImage.GetComponent<RectTransform>();
        minimapRect.sizeDelta = new Vector2(mapSize, mapSize);
        minimapRect.anchorMin = new Vector2(1, 0);
        minimapRect.anchorMax = new Vector2(1, 0);
        minimapRect.pivot = new Vector2(1, 0);
        minimapRect.anchoredPosition = new Vector2(-25, 25);
    }

    private void InitializePlayerMarker()
    {
        if (playerMarker == null)
        {
            GameObject markerObj = new GameObject("PlayerMarker");
            playerMarker = markerObj.AddComponent<RawImage>();
            markerObj.transform.SetParent(minimapImage.transform, false);

            // Create a triangle marker texture
            Texture2D markerTex = new Texture2D(16, 16);
            Color[] colors = new Color[256];
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = Color.clear;
            }
            // Draw a red triangle
            for (int y = 0; y < 8; y++)
            {
                for (int x = y; x < 16 - y; x++)
                {
                    colors[y * 16 + x] = Color.red;
                }
            }
            markerTex.SetPixels(colors);
            markerTex.Apply();

            playerMarker.texture = markerTex;

            markerRect = playerMarker.GetComponent<RectTransform>();
            markerRect.sizeDelta = new Vector2(16, 16);
            markerRect.pivot = new Vector2(0.5f, 0.5f);
        }
    }

    private void CreateMapTexture(int[,] gridInstructions)
    {
        mapTexture = new Texture2D(textureSize, textureSize);
        mapTexture.filterMode = FilterMode.Point;

        // Calculate cell size to maintain square aspect ratio
        cellSize = textureSize / Mathf.Max(gridDimensions.x, gridDimensions.y);

        // Calculate the offset to center the grid
        float offsetX = (textureSize - (cellSize * gridDimensions.x)) * 0.5f;
        float offsetY = (textureSize - (cellSize * gridDimensions.y)) * 0.5f;

        // Fill background
        for (int x = 0; x < textureSize; x++)
        {
            for (int y = 0; y < textureSize; y++)
            {
                mapTexture.SetPixel(x, y, Color.clear);
            }
        }

        // Draw grid cells
        for (int gridX = 0; gridX < gridDimensions.x; gridX++)
        {
            for (int gridY = 0; gridY < gridDimensions.y; gridY++)
            {
                int startX = Mathf.RoundToInt(offsetX + (gridX * cellSize));
                int startY = Mathf.RoundToInt(offsetY + (gridY * cellSize));

                Color cellColor = gridInstructions[gridX, gridY] == 0 ? backgroundColor : pathColor;

                // Draw square cell
                for (int x = 0; x < cellSize - 1; x++)
                {
                    for (int y = 0; y < cellSize - 1; y++)
                    {
                        int pixelX = startX + x;
                        int pixelY = startY + y;

                        if (pixelX < textureSize && pixelY < textureSize)
                        {
                            float distanceFromCenter = Vector2.Distance(
                                new Vector2(pixelX, pixelY),
                                new Vector2(textureSize / 2, textureSize / 2)
                            );

                            if (distanceFromCenter < textureSize / 2)
                            {
                                mapTexture.SetPixel(pixelX, pixelY, cellColor);
                            }
                        }
                    }
                }
            }
        }

        mapTexture.Apply();
        minimapImage.texture = mapTexture;
    }

    void Update()
    {
        if (player == null || playerMarker == null) return;

        // Convert world position to grid position
        Vector2 gridPos = WorldToGridPosition(player.position);

        // Calculate viewport position (0-1 range)
        Vector2 viewportPos = new Vector2(
            gridPos.x / gridDimensions.x,
            gridPos.y / gridDimensions.y
        );

        // Apply position to minimap image
        minimapImage.uvRect = new Rect(
            Mathf.Clamp01(viewportPos.x - 0.15f), // Center horizontally with some padding
            Mathf.Clamp01(viewportPos.y - 0.15f), // Center vertically with some padding
            0.3f, // Show 30% of the map width
            0.3f  // Show 30% of the map height
        );

        // Keep player marker in center
        markerRect.anchoredPosition = new Vector2(
            minimapRect.rect.width * 0.5f,
            minimapRect.rect.height * 0.5f
        );
    }

    private Vector2 WorldToGridPosition(Vector3 worldPos)
    {
        // Assuming the player starts at world position (0,0,0) and should be at grid position (4,0)
        // And each grid cell is 5 units in world space
        float gridX = (worldPos.x / 5f) + 4f; // Offset by 4 to start at row 4
        float gridY = (worldPos.z / 5f); // Z position maps to Y in grid coordinates

        return new Vector2(gridX, gridY);
    }
}
