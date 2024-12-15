using System.Collections.Generic;
using UnityEngine;

public class LevelBuilder : MonoBehaviour
{
    public static LevelBuilder Instance { get; private set; }

    [SerializeField] private int tileSize;
    public int TileSize { get => tileSize; }

    [SerializeField] private GameObject[] levelObjects;
    [SerializeField] private GameObject modifiedInvisibleWall;

    private List<Vector2Int> modifiedWallsGridPos;
    [SerializeField] private GameObject exitArea;
    [SerializeField] private float yAdjustObject;
    [SerializeField] private float yAdjustTarget;
    private int[,] grid;
    public int[,] Grid { get => grid; }


    private Vector2Int startingGrid = new Vector2Int(4, 0);
    private Vector2Int endGrid = new Vector2Int(1, 1);
    public Minimap minimap2;

    private int _targetCounter;

    // 0 - void
    // 1 - floor - default
    // 2 - floor - target
    // 3 - floor - spike trap
    // 4 - floor - saw blades
    // 5 - floor - pitfall
    // 6 - floor - boulder/launcher
    void SingletonCheck()
    {
        // If there is an instance, and it's not this one, delete this one
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Set the instance
        Instance = this;
    }
    void Awake()
    {
        SingletonCheck();

        tileSize = (int)levelObjects[0].transform.localScale.x;

        grid = new int[,] {
            {1, 0, 0, 1, 2, 1, 0, 0, 2, 1, 4, 1, 0, 0, 0, 0, 2, 1, 0, 0},
            {0, 2, 3, 1, 0, 4, 1, 0, 0, 3, 0, 5, 1, 2, 1, 0, 0, 4, 0, 0},
            {0, 1, 0, 2, 0, 1, 2, 1, 0, 1, 0, 0, 0, 0, 5, 6, 0, 1, 0, 0},
            {0, 5, 0, 1, 0, 0, 0, 3, 0, 4, 1, 4, 1, 0, 0, 2, 1, 3, 1, 0},
            {1, 0, 0, 6, 2, 1, 4, 1, 1, 0, 0, 0, 2, 0, 2, 1, 2, 0, 2, 1},
            {1, 0, 0, 0, 0, 3, 0, 0, 2, 1, 5, 0, 0, 0, 0, 3, 5, 0, 0, 0},
            {0, 2, 6, 4, 1, 1, 0, 0, 0, 2, 1, 6, 1, 2, 1, 1, 2, 5, 0, 0},
            {1, 0, 3, 0, 0, 5, 1, 2, 0, 0, 0, 0, 0, 0, 3, 2, 0, 2, 0, 0},
            {0, 0, 2, 2, 0, 0, 0, 1, 4, 1, 2, 5, 2, 0, 1, 0, 0, 0, 0, 0}


            // {0, 0, 0, 1, 2, 1, 0, 0, 2, 1, 4, 1, 0, 0, 0, 0, 2, 1, 0, 0},
            // {0, 2, 3, 1, 0, 4, 1, 0, 0, 3, 0, 5, 1, 2, 1, 0, 0, 4, 0, 0},
            // {0, 1, 0, 2, 0, 1, 2, 1, 0, 1, 0, 0, 0, 0, 5, 6, 0, 1, 0, 0},
            // {0, 5, 0, 1, 0, 0, 0, 3, 0, 4, 1, 4, 1, 0, 0, 2, 1, 3, 1, 0},
            // {1, 1, 0, 6, 2, 1, 4, 1, 1, 0, 0, 0, 2, 0, 2, 1, 2, 0, 2, 1},
            // {0, 0, 0, 0, 0, 3, 0, 0, 2, 1, 5, 0, 0, 0, 0, 3, 5, 0, 0, 0},
            // {0, 2, 6, 4, 1, 1, 0, 0, 0, 2, 1, 6, 1, 2, 1, 1, 2, 5, 0, 0},
            // {0, 0, 3, 0, 0, 5, 1, 2, 0, 0, 0, 0, 0, 0, 3, 2, 0, 2, 0, 0},
            // {0, 0, 2, 2, 0, 0, 0, 1, 4, 1, 2, 5, 2, 0, 1, 0, 0, 0, 0, 0}



            // { 0, 0, 0, 2, 4, 1, 0, 0, 0, 6, 2, 0, 0, 0, 1, 5, 1, 0, 2, 2 },
            // { 0, 1, 0, 1, 0, 2, 3, 1, 0, 3, 0, 0, 2, 1, 2, 0, 1, 2, 1, 0 },
            // { 0, 1, 0, 0, 0, 0, 0, 1, 4, 1, 0, 0, 1, 0, 0, 0, 0, 5, 0, 0 },
            // { 0, 1, 4, 1, 1, 0, 0, 0, 0, 2, 4, 1, 5, 0, 1, 3, 1, 2, 6, 0 },
            // { 1, 2, 0, 0, 5, 1, 3, 6, 0, 0, 0, 0, 1, 4, 2, 0, 0, 0, 2, 1 },
            // { 0, 0, 0, 0, 0, 0, 1, 5, 1, 5, 1, 0, 0, 0, 1, 3, 1, 0, 0, 0 },
            // { 0, 1, 4, 2, 0, 0, 0, 0, 2, 0, 1, 3, 1, 0, 0, 0, 2, 1, 0, 2 },
            // { 0, 0, 0, 3, 1, 2, 1, 0, 4, 0, 0, 0, 2, 4, 1, 0, 0, 5, 1, 1 },
            // { 0, 0, 0, 1, 0, 0, 2, 5, 6, 3, 2, 0, 0, 0, 2, 1, 0, 0, 2, 0 },
        };

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

        // GridResolver resolver = new GridResolver(grid);
        // grid = resolver.FixIsolatedRegions(4, 0);

        // Initialize minimap
        // minimap2.Init(grid, tileSize);

        // Set up starting floor 
        Vector3 startingFloorPos = new Vector3(startingGrid.x * tileSize, yAdjustObject, startingGrid.y * tileSize);
        CreateObject(levelObjects[1], startingFloorPos, Quaternion.identity);
        AddWallInExtremity(startingGrid.x, startingGrid.y);

        // Set up end floor
        Vector3 endFloorPos = new Vector3(endGrid.x * tileSize, yAdjustObject, endGrid.y * tileSize);
        CreateObject(levelObjects[1], endFloorPos, Quaternion.identity);
        AddWallInExtremity(endGrid.x, endGrid.y);
        // add exit 
        Vector3 exitAreaPos = endFloorPos - new Vector3(0, yAdjustObject, 0);
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


                Vector3 pos = new Vector3(i * tileSize, yAdjustObject, j * tileSize);
                GameObject obj = CreateObject(levelObjects[objectId], pos, Quaternion.identity);

                // If a target
                if (objectId == 2)
                {
                    // Add a floor underneath
                    GameObject floor = CreateObject(levelObjects[1], pos, Quaternion.identity);


                    // Adjust y of obj
                    obj.transform.position = new Vector3(pos.x, yAdjustTarget + yAdjustObject, pos.z);
                    obj.transform.SetParent(floor.transform);

                    // Count targets
                    if (objectId == 2) _targetCounter++;
                }

                // If object is in extremity, Add wall in extreme row and column
                if (i == 0 || i == grid.GetLength(0) - 1 || j == 0 || j == grid.GetLength(1) - 1) AddWallInExtremity(i, j);


            }
        }

        FixDiagonallyConnectedWalls();
    }

    public void Start()
    {
        GameManager.Instance.TotalTargets = _targetCounter;

        // initialize loot counter on hud  
        HUDManager._lootUpdater?.Invoke(0);

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

    private void FixDiagonallyConnectedWalls()
    {
        // We could technically put the modified walls directly everywhere but this way minimizes the use of mesh colliders

        modifiedWallsGridPos = new List<Vector2Int>();

        // we skip rows and columns at extremities
        for (int i = 1; i < grid.GetLength(0) - 1; i++)
        {
            for (int j = 1; j < grid.GetLength(1) - 1; j++)
            {
                //ignore void spaces
                if (grid[i, j] == 0) continue;

                // check left, top 
                if (grid[i, j - 1] == 0 && grid[i - 1, j] == 0 && grid[i - 1, j - 1] != 0)
                {
                    ReplaceInvisibleWall(i, j - 1, 0f);
                    ReplaceInvisibleWall(i - 1, j, 180f);
                }

                // check left, bottom
                if (grid[i, j - 1] == 0 && grid[i + 1, j] == 0 && grid[i + 1, j - 1] != 0)
                {
                    ReplaceInvisibleWall(i, j - 1, 90f);
                    ReplaceInvisibleWall(i + 1, j, -90f);
                }

                // check right, top
                if (grid[i, j + 1] == 0 && grid[i - 1, j] == 0 && grid[i - 1, j + 1] != 0)
                {
                    ReplaceInvisibleWall(i, j + 1, -90f);
                    ReplaceInvisibleWall(i - 1, j, 90f);
                }

                // check right, bottom
                if (grid[i, j + 1] == 0 && grid[i + 1, j] == 0 && grid[i + 1, j + 1] != 0)
                {
                    ReplaceInvisibleWall(i, j + 1, 180f);
                    ReplaceInvisibleWall(i + 1, j, 0f);
                }
            }
        }
    }

    private void ReplaceInvisibleWall(int row, int col, float rotation)
    {
        // replace if not already replaced
        if (modifiedWallsGridPos.Contains(new Vector2Int(row, col)))
        {
            // Debug.Log("Already replaced at grid pos: [" + row + ", " + col + "]");
            return;
        }

        // Debug.Log("Need to replace at grid pos: [" + row + ", " + col + "]");
        Vector3 worldPos = new Vector3(row * tileSize, 0, col * tileSize);
        GameObject invisibleWall = FindObjectAtPosition(worldPos);
        // replace if found
        if (invisibleWall != null)
        {
            Destroy(invisibleWall);
            Quaternion rot = Quaternion.Euler(0, rotation, 0);
            GameObject modifiedWall = CreateObject(modifiedInvisibleWall, worldPos, rot);

            // keep record
            modifiedWallsGridPos.Add(new Vector2Int(row, col));
        }

    }
    public GameObject FindObjectAtPosition(Vector3 position, float radius = 0.1f)
    {
        Collider[] hitColliders = Physics.OverlapSphere(position, radius);
        if (hitColliders.Length > 0)
        {
            return hitColliders[0].gameObject;
        }
        return null;
    }

    private GameObject CreateObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        GameObject obj = Instantiate(prefab, position, rotation);

        // Set parent to this object
        obj.transform.parent = transform;

        return obj;
    }

}
