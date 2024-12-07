using System.Runtime.Serialization;
using UnityEngine;

public class LevelBuilder : MonoBehaviour
{
    [SerializeField] private int tileSize;

    [SerializeField] private GameObject[] levelObjects;

    [SerializeField] private GameObject exitArea;

    // private float yAdjustObject = -50f;
    [SerializeField] private float yAdjustObjectFinal = -2.5f;
    [SerializeField] private float yAdjustTarget = 3.5f;
    private int[,] grid;
    private bool[,] isCellTriggered;

    private Vector2Int startingGrid = new Vector2Int(4, 0);
    private Vector2Int endGrid = new Vector2Int(0, 1);

    // public CircularMinimapV2 minimap;
    public Minimap minimap2;
    // [SerializeField] private Transform player;
    // private Vector2Int prevPlayerPos;


    // Start is called before the first frame update
    void Awake()
    {
        yAdjustObjectFinal = levelObjects[1].transform.localScale.y / -2f;
        yAdjustTarget = 1f - yAdjustObjectFinal;

        grid = new int[,] {
            { 0, 0, 0, 2, 4, 1, 0, 0, 0, 6, 2, 0, 0, 0, 1, 5, 1, 0, 2, 2 },
            { 0, 1, 0, 1, 0, 2, 3, 1, 0, 3, 0, 0, 2, 1, 2, 0, 1, 2, 1, 0 },
            { 0, 1, 0, 0, 0, 0, 0, 1, 4, 1, 0, 0, 1, 0, 0, 0, 0, 5, 0, 0 },
            { 0, 5, 4, 1, 1, 0, 0, 0, 0, 2, 4, 1, 5, 0, 1, 3, 1, 2, 6, 0 },
            { 1, 1, 0, 0, 5, 1, 3, 6, 0, 0, 0, 0, 1, 4, 2, 0, 0, 0, 2, 1 },
            { 0, 0, 0, 0, 0, 0, 1, 5, 1, 5, 1, 0, 0, 0, 1, 3, 1, 0, 0, 0 },
            { 0, 1, 4, 2, 0, 0, 0, 0, 2, 0, 1, 3, 1, 0, 0, 0, 2, 1, 0, 2 },
            { 0, 0, 0, 3, 1, 2, 1, 0, 4, 0, 0, 0, 2, 4, 1, 0, 0, 5, 1, 1 },
            { 0, 0, 0, 1, 0, 0, 2, 5, 6, 3, 2, 0, 0, 0, 2, 1, 0, 0, 2, 0 },
        };

        isCellTriggered = new bool[grid.GetLength(0), grid.GetLength(1)];
        // prevPlayerPos = startingGrid;


        //     { 0, 0, 0, 2, 4, 1, 0, 0, 0, 6, 2, 0, 0, 0, 1, 5, 1, 0, 2, 7 },
        //     { 0, 1, 0, 1, 0, 2, 3, 1, 0, 3, 0, 0, 2, 1, 2, 0, 1, 2, 1, 0 },
        //     { 0, 2, 0, 0, 0, 0, 0, 1, 4, 1, 0, 0, 1, 0, 0, 0, 0, 5, 0, 0 },
        //     { 0, 1, 4, 2, 1, 0, 0, 0, 0, 2, 4, 1, 5, 0, 1, 3, 1, 2, 6, 0 },
        //     { 1, 2, 0, 0, 5, 1, 3, 6, 0, 0, 0, 0, 1, 4, 2, 0, 0, 0, 2, 1 },
        //     { 0, 0, 0, 0, 0, 0, 1, 5, 1, 5, 1, 0, 0, 0, 1, 3, 1, 0, 0, 0 },
        //     { 0, 1, 4, 2, 0, 0, 0, 0, 2, 0, 1, 3, 1, 0, 0, 0, 2, 1, 0, 8 },
        //     { 0, 0, 0, 3, 1, 2, 1, 0, 4, 0, 0, 0, 2, 4, 1, 0, 0, 5, 1, 1 },
        //     { 0, 0, 0, 1, 0, 0, 2, 5, 6, 3, 2, 0, 0, 0, 2, 1, 0, 0, 2, 0 },

        BuildLevel();
    }

    void BuildLevel()
    {
        if (grid[startingGrid.x, startingGrid.y] != 1)
        {
            grid[startingGrid.x, startingGrid.y] = 1;
        }

        if (grid[endGrid.x, endGrid.y] != 1)
        {
            grid[endGrid.x, endGrid.y] = 1;
        }

        GridResolver resolver = new GridResolver(grid);
        grid = resolver.FixIsolatedRegions(4, 0);

        // Initialize minimap
        // minimap.Init(grid, tileSize);
        minimap2.Init(grid);

        // Set up starting floor 
        Vector3 startingFloorPos = new Vector3(startingGrid.x * tileSize, yAdjustObjectFinal, startingGrid.y * tileSize);
        CreateObject(levelObjects[1], startingFloorPos, Quaternion.identity);
        AddWallInExtremity(startingGrid.x, startingGrid.y);
        // isCellTriggered[startingGrid.x, startingGrid.y] = true;

        // Set up end floor
        Vector3 endFloorPos = new Vector3(endGrid.x * tileSize, yAdjustObjectFinal, endGrid.y * tileSize);
        CreateObject(levelObjects[1], endFloorPos, Quaternion.identity);
        AddWallInExtremity(endGrid.x, endGrid.y);
        // add exit 
        Vector3 exitAreaPos = endFloorPos - new Vector3(0, yAdjustObjectFinal, 0);
        Instantiate(exitArea, exitAreaPos, Quaternion.identity);


        // create 2d array for loop
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                // skip starting & end floor
                if (i == startingGrid.x && j == startingGrid.y) continue;
                if (i == endGrid.x && j == endGrid.y) continue;

                int objectId = grid[i, j];

                if (objectId == 0)
                {

                    // check if at least 1 neighbor is not zero, place invisible wall so player cannot fall.
                    if (Helpers.HasNonZeroNeighbor(grid, i, j))
                    {
                        Vector3 wallPos = new Vector3(i * tileSize, 0, j * tileSize);
                        CreateObject(levelObjects[objectId], wallPos, Quaternion.identity);
                    }
                    continue;
                }


                Vector3 pos = new Vector3(i * tileSize, yAdjustObjectFinal, j * tileSize);
                GameObject obj = CreateObject(levelObjects[objectId], pos, Quaternion.identity);

                // If a target or bonus teleporter
                if (objectId == 2 || objectId == 7 || objectId == 8)
                {
                    // Add a floor underneath
                    GameObject floor = CreateObject(levelObjects[1], pos, Quaternion.identity);


                    // Adjust y of obj
                    obj.transform.position = new Vector3(pos.x, yAdjustTarget + yAdjustObjectFinal, pos.z);
                    obj.transform.SetParent(floor.transform);
                }

                // If object is in extremity, Add wall in extreme row and column
                if (i == 0 || i == grid.GetLength(0) - 1 || j == 0 || j == grid.GetLength(1) - 1) AddWallInExtremity(i, j);


            }
        }
    }

    private void AddWallInExtremity(int row, int col)
    {
        Vector3 extremeRowWallPos = Vector3.zero;
        Vector3 extremeColWallPos = Vector3.zero;
        if (row == 0)
        {
            extremeRowWallPos = new Vector3(-tileSize, 0, col * tileSize);
        }
        else if (row == grid.GetLength(0) - 1)
        {
            extremeRowWallPos = new Vector3(grid.GetLength(0) * tileSize, 0, col * tileSize);
        }

        if (col == 0)
        {
            extremeColWallPos = new Vector3(row * tileSize, 0, -tileSize);
        }
        else if (col == grid.GetLength(1) - 1)
        {
            extremeColWallPos = new Vector3(row * tileSize, 0, (grid.GetLength(1) * tileSize));
        }


        if (extremeRowWallPos != Vector3.zero) CreateObject(levelObjects[0], extremeRowWallPos, Quaternion.identity);
        if (extremeColWallPos != Vector3.zero) CreateObject(levelObjects[0], extremeColWallPos, Quaternion.identity);
    }

    private GameObject CreateObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        GameObject obj = Instantiate(prefab, position, rotation);

        // Set parent to this object
        obj.transform.parent = transform;

        return obj;
    }

    // void Update()
    // {
    //     if (player == null) return;

    //     Vector2Int gridPos = minimap2.WorldToGridPosition(player.position);
    //     if (gridPos != prevPlayerPos)
    //     {
    //         Debug.Log("Player is in new cell");
    //         prevPlayerPos = gridPos;
    //     }
    // }

}
