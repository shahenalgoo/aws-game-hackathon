using UnityEngine;
using UnityEngine.UI;

// By Amazon Q
public class Minimap : MonoBehaviour
{
    [SerializeField] private RectTransform minimapContainer; // Assign your container in inspector
    public RectTransform MinimapContainer { get { return minimapContainer; } }
    [SerializeField] private GameObject cellPrefab; // Create a prefab with a RawImage component
    [SerializeField] private Color backgroundColor = Color.black;
    [SerializeField] private Color pathColor = Color.white;
    [SerializeField] private Color extractionColor = Color.blue;
    [SerializeField] private Color playerColor = Color.blue;
    [SerializeField] private Color enemyColor = Color.blue;
    [SerializeField] private float mapSize = 216f;
    [SerializeField] private float padding = 20f;
    [SerializeField] private Transform player;  // Assign the player's transform in inspector
    [SerializeField] private float cellSize;
    [SerializeField] private int gridStartingX;
    private Vector2Int prevPlayerGridPos = new Vector2Int(-1, -1);
    private RawImage[,] gridCells;
    private Vector2Int gridDimensions;
    private int[,] gridInstructions;

    public void ToggleMap()
    {
        if (Time.timeScale == 0) return;

        if (!gameObject.activeSelf) return;

        minimapContainer.gameObject.SetActive(!minimapContainer.gameObject.activeSelf);
    }

    private void Update()
    {
        if (player == null) return;

        // Update minimap only when player moves to another grid
        Vector2Int currentPlayerGridPos = WorldToGridPosition(player.position);
        if (prevPlayerGridPos != currentPlayerGridPos)
        {
            UpdateCell(prevPlayerGridPos.x, prevPlayerGridPos.y);
            gridCells[currentPlayerGridPos.x, currentPlayerGridPos.y].color = playerColor;
            prevPlayerGridPos = currentPlayerGridPos;
        }
    }


    public Vector2Int WorldToGridPosition(Vector3 worldPos)
    {

        // Starting position of the grid (28, 0) corresponds to grid (4, 0)
        float startX = (cellSize * gridStartingX) - (cellSize / 2f); // 24.5f - starting X of the first cell in this row
        float startZ = 0f - (cellSize / 2f);  // -3.5f - starting Z of the first cell in this column

        // Calculate relative position from the grid start
        float relativeX = worldPos.x - startX;
        float relativeZ = worldPos.z - startZ;

        return new Vector2Int(
            4 + Mathf.FloorToInt(relativeX / cellSize),
            Mathf.FloorToInt(relativeZ / cellSize)
        );
    }

    public void Init(int[,] gridData, int tileSize)
    {
        cellSize = tileSize;
        gridInstructions = gridData;
        gridDimensions = new Vector2Int(gridData.GetLength(0), gridData.GetLength(1));
        player = GameObject.FindGameObjectWithTag("Player").transform;
        CreateGrid();
    }

    private void CreateGrid()
    {
        // Calculate available space and cell size
        float availableWidth = mapSize - (padding * 2);
        float cellSize = availableWidth / gridDimensions.y;

        // Calculate total dimensions
        float totalWidth = mapSize;
        float totalHeight = (cellSize * gridDimensions.x) + (padding * 2);

        // Set container size
        minimapContainer.sizeDelta = new Vector2(totalWidth, totalHeight);

        // Create grid cells
        gridCells = new RawImage[gridDimensions.x, gridDimensions.y];

        // Create a single white texture for all cells
        Texture2D cellTexture = new Texture2D(1, 1);
        cellTexture.SetPixel(0, 0, Color.white);
        cellTexture.Apply();

        for (int row = 0; row < gridDimensions.x; row++)
        {
            for (int col = 0; col < gridDimensions.y; col++)
            {
                GameObject cellObj = Instantiate(cellPrefab, minimapContainer);
                RawImage cellImage = cellObj.GetComponent<RawImage>();
                gridCells[row, col] = cellImage;

                // Set cell position and size
                RectTransform rectTransform = cellImage.rectTransform;
                rectTransform.sizeDelta = new Vector2(cellSize, cellSize);

                float xPos = padding + (col * cellSize);
                float yPos = padding + ((gridDimensions.x - 1 - row) * cellSize);

                rectTransform.anchorMin = new Vector2(0, 0);
                rectTransform.anchorMax = new Vector2(0, 0);
                rectTransform.pivot = new Vector2(0, 0);
                rectTransform.anchoredPosition = new Vector2(xPos, yPos);

                // Set initial color based on grid data
                cellImage.texture = cellTexture;
                cellImage.color = GetColorForCell(row, col);
            }
        }

        Vector2Int currentPlayerGridPos = WorldToGridPosition(player.position);
        gridCells[currentPlayerGridPos.x, currentPlayerGridPos.y].color = playerColor;


    }

    public void UpdateCell(int row, int col)
    {
        if (gridCells != null && row >= 0 && row < gridDimensions.x && col >= 0 && col < gridDimensions.y)
        {
            gridCells[row, col].color = GetColorForCell(row, col);
        }
    }

    public Color GetColorForCell(int row, int col)
    {
        int objectId = gridInstructions[row, col];
        switch (objectId)
        {
            case 0:
                return backgroundColor;
            case 2:
                return enemyColor;
            case 8:
                return extractionColor;
            default:
                return pathColor;
        }

    }

    public void AmendGrid(Vector2Int gridPos, int newId)
    {
        gridInstructions[gridPos.x, gridPos.y] = newId;

        Vector2Int currentPlayerGridPos = WorldToGridPosition(player.position);
        // if player not on here, update
        if (gridPos != currentPlayerGridPos)
        {
            UpdateCell(gridPos.x, gridPos.y);
        }
    }
}
