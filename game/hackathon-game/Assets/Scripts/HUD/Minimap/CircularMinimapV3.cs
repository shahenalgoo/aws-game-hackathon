using UnityEngine;
using UnityEngine.UI;

public class CircularMinimapV3 : MonoBehaviour
{
    [SerializeField] private RawImage mapImage; // Reference to the Map Image
    [SerializeField] private float mapSize = 216f;
    [SerializeField] private Transform player;
    [SerializeField] private Color backgroundColor;
    [SerializeField] private Color pathColor;
    [SerializeField] private Vector2Int gridSize = new Vector2Int(9, 20); // Your grid dimensions

    // Add these new serialized fields for configuration
    [SerializeField] private int startingGridX = 4;
    [SerializeField] private int viewRange = 4;
    [SerializeField] private RectTransform maskRectTransform; // Reference to the Mask
    [SerializeField] private Image playerMarker; // Reference to the Player Marker

    private float cellSizeX;
    private float cellSizeY;
    private int textureSize = 256;
    private int[,] gridData;
    private Vector2 cellSize; // Size of each grid cell in world units
    private Vector2Int prevPlayerGridPos;
    private Texture2D fullMapTexture; // Stores the complete map texture
    private Vector2 currentMapOffset;
    private Vector2 targetMapOffset;
    private float lerpSpeed = 8f;

    public void Init(int[,] gridInstructions, float cellDimension)
    {
        InitializeMinimapUI();
        CreateCircularMask();

        gridData = gridInstructions;
        cellSizeX = cellSizeY = cellDimension;
        cellSize = new Vector2(cellSizeX, cellSizeY);

        // Create the full map texture
        CreateFullMapTexture();

        prevPlayerGridPos = new Vector2Int(startingGridX, 0);

        // Set initial position
        UpdateMinimapPosition();

        // Initialize player marker
        playerMarker.color = Color.red;
        playerMarker.rectTransform.anchoredPosition = Vector2.zero;
    }

    private void InitializeMinimapUI()
    {
        mapImage.material = new Material(Shader.Find("UI/Default"));
        mapImage.color = Color.white;

        maskRectTransform.sizeDelta = new Vector2(mapSize, mapSize);
        maskRectTransform.anchorMin = new Vector2(0, 0);
        maskRectTransform.anchorMax = new Vector2(0, 0);
        maskRectTransform.pivot = new Vector2(0, 0);
        maskRectTransform.anchoredPosition = new Vector2(-25, 125);
        maskRectTransform.localRotation = Quaternion.Euler(0, 0, -45);

        // Setup the map image
        RectTransform mapRect = mapImage.rectTransform;
        mapRect.anchorMin = Vector2.zero;
        mapRect.anchorMax = Vector2.one;
        mapRect.sizeDelta = Vector2.zero;
        mapRect.anchoredPosition = Vector2.zero;
    }

    private void CreateFullMapTexture()
    {
        // Calculate the size needed for the full map texture
        int pixelsPerCell = textureSize / (viewRange * 2);
        int fullTextureWidth = gridSize.x * pixelsPerCell;
        int fullTextureHeight = gridSize.y * pixelsPerCell;

        fullMapTexture = new Texture2D(fullTextureWidth, fullTextureHeight);
        fullMapTexture.filterMode = FilterMode.Bilinear;

        // Draw each grid cell
        for (int y = 0; y < gridSize.y; y++)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                Color cellColor = GetColorForGridValue(gridData[x, y]);

                // Fill the cell with color
                for (int py = 0; py < pixelsPerCell; py++)
                {
                    for (int px = 0; px < pixelsPerCell; px++)
                    {
                        int pixelX = (x * pixelsPerCell) + px;
                        int pixelY = (y * pixelsPerCell) + py;
                        fullMapTexture.SetPixel(pixelX, pixelY, cellColor);
                    }
                }
            }
        }

        fullMapTexture.Apply();
        mapImage.texture = fullMapTexture;

        // Set initial UV rect to show the full map
        float aspectRatio = (float)fullTextureWidth / fullTextureHeight;
        float uvWidth = (float)textureSize / fullTextureWidth;
        float uvHeight = (float)textureSize / fullTextureHeight;
        mapImage.uvRect = new Rect(0, 0, uvWidth, uvHeight);
    }

    void Update()
    {
        if (player == null) return;

        Vector2Int currentPlayerGridPos = WorldToGridPosition(player.position);
        if (prevPlayerGridPos != currentPlayerGridPos)
        {
            prevPlayerGridPos = currentPlayerGridPos;
            UpdateMinimapPosition();
        }

        // Calculate UV dimensions
        float uvWidth = (float)textureSize / fullMapTexture.width;
        float uvHeight = (float)textureSize / fullMapTexture.height;

        // Smooth movement of the map
        currentMapOffset = Vector2.Lerp(currentMapOffset, targetMapOffset, Time.deltaTime * lerpSpeed);
        mapImage.uvRect = new Rect(currentMapOffset, new Vector2(textureSize / (float)fullMapTexture.width,
                                                                textureSize / (float)fullMapTexture.height));
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

    private void UpdateMinimapPosition()
    {
        Vector2Int playerGridPos = WorldToGridPosition(player.position);

        // Calculate UV coordinates based on player position
        float uvX = (float)playerGridPos.x / gridSize.x;
        float uvY = (float)playerGridPos.y / gridSize.y;


        float uvWidth = (float)textureSize / fullMapTexture.width;
        float uvHeight = (float)textureSize / fullMapTexture.height;

        // Center the view on the player
        targetMapOffset = new Vector2(
            uvX - (textureSize / (2f * fullMapTexture.width)),
            uvY - (textureSize / (2f * fullMapTexture.height))
        );

        // Clamp the offset to prevent showing areas outside the map
        targetMapOffset.x = Mathf.Clamp(targetMapOffset.x, 0, 1 - uvWidth);
        targetMapOffset.y = Mathf.Clamp(targetMapOffset.y, 0, 1 - uvHeight);
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

    private void CreateCircularMask()
    {
        // Create a mask texture
        Texture2D maskTexture = new Texture2D(textureSize, textureSize);
        maskTexture.filterMode = FilterMode.Bilinear;

        float center = textureSize / 2f;
        float radius = textureSize / 2f;

        for (int y = 0; y < textureSize; y++)
        {
            for (int x = 0; x < textureSize; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), new Vector2(center, center));
                maskTexture.SetPixel(x, y, distance < radius ? Color.white : Color.clear);
            }
        }

        maskTexture.Apply();

        // Create a mask material
        Material maskMaterial = new Material(Shader.Find("UI/Default"));
        maskMaterial.mainTexture = maskTexture;

        // Apply the mask
        mapImage.material = maskMaterial;
    }
}
