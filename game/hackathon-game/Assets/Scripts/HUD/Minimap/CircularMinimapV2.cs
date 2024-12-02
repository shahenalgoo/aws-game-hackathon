using UnityEngine;
using UnityEngine.UI;
public class CircularMinimapV2 : MonoBehaviour
{
    [SerializeField] private RawImage minimapImage;
    [SerializeField] private float mapSize = 216f;
    [SerializeField] private Transform player;
    [SerializeField] private Color backgroundColor;
    [SerializeField] private Color pathColor;
    [SerializeField] private Vector2Int gridSize = new Vector2Int(9, 20); // Your grid dimensions

    // Add these new serialized fields for configuration
    [SerializeField] private int startingGridX = 4;
    [SerializeField] private int viewRange = 4;
    private float cellSizeX;
    private float cellSizeY;
    private Texture2D minimapTexture;
    private int textureSize = 256;
    private int[,] gridData;
    private Vector2 cellSize; // Size of each grid cell in world units

    private Vector2Int prevPlayerGridPos;

    void Start()
    {

    }

    public void Init(int[,] gridInstructions, float cellDimension)
    {
        InitializeMinimapUI();
        CreateCircularMask();

        // Initialize your grid data here
        gridData = new int[gridSize.x, gridSize.y];
        // Populate gridData with your values
        gridData = gridInstructions;

        cellSizeX = cellSizeY = cellDimension;


        // Calculate cell size based on your world space
        cellSize = new Vector2(
            cellSizeX,
            cellSizeY
        );

        prevPlayerGridPos = new Vector2Int(startingGridX, 0);

        // Create minimap on init
        UpdateMinimap();
    }

    private void InitializeMinimapUI()
    {
        minimapImage.material = new Material(Shader.Find("UI/Default"));
        minimapImage.color = Color.white;

        RectTransform rectTransform = minimapImage.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(mapSize, mapSize);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.pivot = new Vector2(0, 0);
        rectTransform.anchoredPosition = new Vector2(-25, 125);

        // Rotate the RawImage 45 degrees clockwise
        rectTransform.localRotation = Quaternion.Euler(0, 0, -45);

    }

    void CreateCircularMask()
    {
        minimapTexture = new Texture2D(textureSize, textureSize);
        minimapTexture.filterMode = FilterMode.Bilinear;

        float center = textureSize / 2f;
        float radius = textureSize / 2f;

        for (int y = 0; y < textureSize; y++)
        {
            for (int x = 0; x < textureSize; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), new Vector2(center, center));
                minimapTexture.SetPixel(x, y, distance < radius ? backgroundColor : Color.clear);
            }
        }

        minimapTexture.Apply();
        minimapImage.texture = minimapTexture;
    }

    void Update()
    {
        if (player == null) return;

        // Update minimap only when player moves to another grid
        Vector2Int currentPlayerGridPos = WorldToGridPosition(player.position);
        if (prevPlayerGridPos != currentPlayerGridPos)
        {
            prevPlayerGridPos = currentPlayerGridPos;
            UpdateMinimap();
        }
    }

    void UpdateMinimap()
    {
        if (player == null || minimapTexture == null) return;

        // Clear the texture with background color
        ClearMinimapTexture();

        // Get player's grid position
        Vector2Int playerGridPos = WorldToGridPosition(player.position);

        // Draw visible grid cells
        for (int y = -viewRange; y <= viewRange; y++)
        {
            for (int x = -viewRange; x <= viewRange; x++)
            {
                Vector2Int gridPos = new Vector2Int(
                    playerGridPos.x + x,
                    playerGridPos.y + y
                );

                if (IsValidGridPosition(gridPos))
                {
                    Vector2 minimapPos = GridToMinimapPosition(gridPos, playerGridPos);
                    int gridValue = gridData[gridPos.x, gridPos.y];

                    DrawGridCellOnMinimap(minimapPos, GetColorForGridValue(gridValue));
                }
            }
        }

        // Draw player
        Vector2 playerMinimapPos = new Vector2(textureSize / 2, textureSize / 2);
        DrawObjectOnMinimap(playerMinimapPos, Color.red, 6);

        minimapTexture.Apply();
    }

    private Vector2Int WorldToGridPosition(Vector3 worldPos)
    {

        // Starting position of the grid (28, 0) corresponds to grid (4, 0)
        float startX = (cellSizeX * startingGridX) - (cellSizeX / 2f); // 24.5f - starting X of the first cell in this row
        float startZ = 0f - (cellSizeY / 2f);  // -3.5f - starting Z of the first cell in this column

        // Calculate relative position from the grid start
        float relativeX = worldPos.x - startX;
        float relativeZ = worldPos.z - startZ;

        return new Vector2Int(
            4 + Mathf.FloorToInt(relativeX / cellSize.x),
            Mathf.FloorToInt(relativeZ / cellSize.y)
        );
    }

    private Vector2 GridToMinimapPosition(Vector2Int gridPos, Vector2Int playerGridPos)
    {
        Vector2 relativePos = new Vector2(
            gridPos.x - playerGridPos.x,
            gridPos.y - playerGridPos.y
        );

        float scaleFactor = textureSize / (viewRange * 2f);


        return new Vector2(
            (relativePos.x * scaleFactor) + (textureSize / 2f),
            (relativePos.y * scaleFactor) + (textureSize / 2f)
        );
    }

    private bool IsValidGridPosition(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < gridSize.x &&
               pos.y >= 0 && pos.y < gridSize.y;
    }

    private Color GetColorForGridValue(int value)
    {
        // Define colors for different grid values
        switch (value)
        {
            case 0: return backgroundColor; // Empty space
            default: return pathColor; // Other
        }
    }

    private void ClearMinimapTexture()
    {
        float center = textureSize / 2f;
        float radius = textureSize / 2f;

        for (int y = 0; y < textureSize; y++)
        {
            for (int x = 0; x < textureSize; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), new Vector2(center, center));
                if (distance < radius)
                {
                    minimapTexture.SetPixel(x, y, backgroundColor);
                }
            }
        }
    }

    void DrawGridCellOnMinimap(Vector2 position, Color color)
    {
        int size = textureSize / (viewRange * 2) + 1;

        for (int y = -size / 2; y <= size / 2; y++)
        {
            for (int x = -size / 2; x <= size / 2; x++)
            {
                int pixelX = Mathf.RoundToInt(position.x + x);
                int pixelY = Mathf.RoundToInt(position.y + y);

                if (pixelX >= 0 && pixelX < textureSize &&
                    pixelY >= 0 && pixelY < textureSize)
                {
                    float distanceFromCenter = Vector2.Distance(
                        new Vector2(pixelX, pixelY),
                        new Vector2(textureSize / 2, textureSize / 2)
                    );

                    if (distanceFromCenter <= textureSize / 2)
                    {
                        minimapTexture.SetPixel(pixelX, pixelY, color);
                    }
                }
            }
        }
    }

    void DrawObjectOnMinimap(Vector2 position, Color color, int size)
    {
        for (int y = -size; y <= size; y++)
        {
            int rowWidth = size - Mathf.Abs(y);
            for (int x = -rowWidth; x <= rowWidth; x++)
            {
                // Rotate the point 45 degrees clockwise
                float rotatedX = (x - y) * 0.7071f;
                float rotatedY = (x + y) * 0.7071f;

                int pixelX = Mathf.RoundToInt(position.x + rotatedX);
                int pixelY = Mathf.RoundToInt(position.y + rotatedY);

                if (pixelX >= 0 && pixelX < textureSize &&
                    pixelY >= 0 && pixelY < textureSize)
                {
                    float distanceFromCenter = Vector2.Distance(
                        new Vector2(pixelX, pixelY),
                        new Vector2(textureSize / 2, textureSize / 2)
                    );

                    if (distanceFromCenter <= textureSize / 2)
                    {
                        minimapTexture.SetPixel(pixelX, pixelY, color);
                    }
                }
            }
        }
    }
}
