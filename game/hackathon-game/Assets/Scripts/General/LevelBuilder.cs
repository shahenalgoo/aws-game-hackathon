using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class LevelBuilder : MonoBehaviour
{
    public static LevelBuilder Instance { get; private set; }

    [SerializeField] private int _tileSize;
    public int TileSize { get => _tileSize; }

    [SerializeField] private GameObject[] levelObjects;
    [SerializeField] private GameObject _modifiedInvisibleWall;

    private List<Vector2Int> modifiedWallsGridPos;
    [SerializeField] private GameObject _extractionArea;
    [SerializeField] private float _yAdjustObject;
    private int[,] _grid;
    public int[,] Grid { get => _grid; }

    private Vector2Int _startingGrid = new Vector2Int(4, 0);
    [SerializeField] private Vector2Int _endGrid = new Vector2Int(1, 1);
    public Minimap _minimap;

    private ExtractionController _extractionController;
    public ExtractionController ExtractionController { get { return _extractionController; } }


    private int _targetCounter;

    // 0 - void
    // 1 - floor - default
    // 2 - floor - target
    // 3 - floor - spike trap
    // 4 - floor - saw blades
    // 5 - floor - pitfall
    // 6 - floor - boulder/launcher
    // 7 - floor - turning blades
    // 8 - floor - extraction
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
        _grid = GetGridData();

        _tileSize = (int)levelObjects[0].transform.localScale.x;

        // _grid = new int[,] {

        //     {0, 0, 0, 2, 1, 4, 1, 2, 0, 0, 0, 2, 1, 7, 0, 0, 0, 0, 0, 0},
        //     {0, 2, 3, 1, 0, 0, 0, 1, 4, 1, 0, 0, 0, 1, 2, 1, 4, 2, 0, 0},
        //     {0, 1, 0, 5, 1, 2, 0, 3, 0, 5, 1, 2, 0, 0, 0, 0, 0, 1, 0, 0},
        //     {2, 4, 0, 1, 0, 1, 0, 1, 0, 0, 0, 1, 4, 1, 2, 3, 0, 5, 2, 0},
        //     {1, 1, 6, 2, 0, 3, 7, 2, 1, 6, 1, 0, 0, 0, 1, 1, 0, 1, 3, 1},
        //     {0, 0, 0, 7, 0, 1, 0, 0, 0, 0, 2, 1, 3, 0, 5, 2, 0, 0, 0, 0},
        //     {0, 2, 3, 2, 0, 4, 1, 2, 0, 0, 0, 0, 1, 0, 1, 4, 1, 2, 0, 0},
        //     {0, 1, 0, 5, 1, 0, 0, 1, 4, 1, 2, 0, 5, 0, 0, 0, 0, 7, 0, 0},
        //     {0, 2, 0, 1, 2, 0, 0, 0, 0, 0, 1, 3, 2, 1, 4, 2, 0, 2, 1, 0}


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
        // { 3, 1, 1, 1, 1, 0, 0, 0, 0, 2, 4, 1, 5, 0, 1, 3, 1, 2, 6, 0 },
        // { 1, 4, 0, 0, 5, 1, 3, 6, 0, 0, 0, 0, 1, 4, 2, 0, 0, 0, 2, 1 },
        // { 0, 0, 0, 0, 0, 0, 1, 5, 1, 5, 1, 0, 0, 0, 1, 3, 1, 0, 0, 0 },
        // { 0, 1, 4, 2, 0, 0, 0, 0, 2, 0, 1, 3, 1, 0, 0, 0, 2, 1, 0, 2 },
        // { 0, 0, 0, 3, 1, 2, 1, 0, 4, 0, 0, 0, 2, 4, 1, 0, 0, 5, 1, 1 },
        // { 0, 0, 0, 1, 0, 0, 2, 5, 6, 3, 2, 0, 0, 0, 2, 1, 0, 0, 2, 0 },
        // };

        // { 0, 0, 0, 2, 4, 1, 0, 0, 0, 6, 2, 0, 0, 0, 1, 5, 1, 0, 2, 7 },
        // { 0, 1, 0, 1, 0, 2, 3, 1, 0, 3, 0, 0, 2, 1, 2, 0, 1, 2, 1, 0 },
        // { 0, 2, 0, 0, 0, 0, 0, 1, 4, 1, 0, 0, 1, 0, 0, 0, 0, 5, 0, 0 },
        // { 0, 1, 4, 2, 1, 0, 0, 0, 0, 2, 4, 1, 5, 0, 1, 3, 1, 2, 6, 0 },
        // { 1, 2, 0, 0, 5, 1, 3, 6, 0, 0, 0, 0, 1, 4, 2, 0, 0, 0, 2, 1 },
        // { 0, 0, 0, 0, 0, 0, 1, 5, 1, 5, 1, 0, 0, 0, 1, 3, 1, 0, 0, 0 },
        // { 0, 1, 4, 2, 0, 0, 0, 0, 2, 0, 1, 3, 1, 0, 0, 0, 2, 1, 0, 8 },
        // { 0, 0, 0, 3, 1, 2, 1, 0, 4, 0, 0, 0, 2, 4, 1, 0, 0, 5, 1, 1 },
        // { 0, 0, 0, 1, 0, 0, 2, 5, 6, 3, 2, 0, 0, 0, 2, 1, 0, 0, 2, 0 },
        // };


        BuildLevel();
    }

    void BuildLevel()
    {
        if (_grid[_startingGrid.x, _startingGrid.y] != 1)
        {
            _grid[_startingGrid.x, _startingGrid.y] = 1;
        }

        GridResolver resolver = new GridResolver(_grid);
        _grid = resolver.FixIsolatedRegions(4, 0);

        // Set up starting floor 
        Vector3 startingFloorPos = new Vector3(_startingGrid.x * _tileSize, _yAdjustObject, _startingGrid.y * _tileSize);
        CreateObject(levelObjects[1], startingFloorPos, Quaternion.identity);
        AddWallInExtremity(_startingGrid.x, _startingGrid.y);

        // create 2d array for loop
        for (int i = 0; i < _grid.GetLength(0); i++)
        {
            for (int j = 0; j < _grid.GetLength(1); j++)
            {
                // skip starting floor
                if (i == _startingGrid.x && j == _startingGrid.y) continue;
                // if (i == _endGrid.x && j == _endGrid.y) continue;

                int objectId = _grid[i, j];

                if (objectId == 0)
                {

                    // check if at least 1 neighbor is not zero, place invisible wall so player cannot fall.
                    if (Helpers.HasNonZeroNeighbor(_grid, i, j))
                    {
                        Vector3 wallPos = new Vector3(i * _tileSize, 0, j * _tileSize);
                        CreateObject(levelObjects[objectId], wallPos, Quaternion.identity);
                    }
                    continue;
                }


                Vector3 pos = new Vector3(i * _tileSize, _yAdjustObject, j * _tileSize);
                GameObject obj = CreateObject(levelObjects[objectId], pos, Quaternion.identity);

                // Count targets
                if (objectId == 2) _targetCounter++;

                // If extraction floor, get a reference to the controller
                if (objectId == 8) _extractionController = obj.GetComponentInChildren<ExtractionController>();

                // If object is in extremity, Add wall in extreme row and column
                if (i == 0 || i == _grid.GetLength(0) - 1 || j == 0 || j == _grid.GetLength(1) - 1) AddWallInExtremity(i, j);


            }
        }

        FixDiagonallyConnectedWalls();
    }

    public void InitializeMinimap()
    {
        // Initialize minimap
        _minimap.Init(_grid, _tileSize);
    }

    public void Start()
    {
        if (GameManager.Instance != null) GameManager.Instance.TotalTargets = _targetCounter;

        // initialize loot counter on hud  
        HUDManager._lootUpdater?.Invoke(0);

        // Create minimap if player entrance animation is skipped
        if (GameManager.Instance != null && !GameManager.Instance.UsePlayerEntranceAnimation) InitializeMinimap();

    }

    private void AddWallInExtremity(int row, int col)
    {
        Vector3 extremeRowWallPos = Vector3.zero;
        Vector3 extremeColWallPos = Vector3.zero;
        if (row == 0)
        {
            extremeRowWallPos = new Vector3(-_tileSize, 0, col * _tileSize);
        }
        else if (row == _grid.GetLength(0) - 1)
        {
            extremeRowWallPos = new Vector3(_grid.GetLength(0) * _tileSize, 0, col * _tileSize);
        }

        if (col == 0)
        {
            extremeColWallPos = new Vector3(row * _tileSize, 0, -_tileSize);
        }
        else if (col == _grid.GetLength(1) - 1)
        {
            extremeColWallPos = new Vector3(row * _tileSize, 0, (_grid.GetLength(1) * _tileSize));
        }


        if (extremeRowWallPos != Vector3.zero) CreateObject(levelObjects[0], extremeRowWallPos, Quaternion.identity);
        if (extremeColWallPos != Vector3.zero) CreateObject(levelObjects[0], extremeColWallPos, Quaternion.identity);
    }

    private void FixDiagonallyConnectedWalls()
    {
        // We could technically put the modified walls directly everywhere but this way minimizes the use of mesh colliders

        modifiedWallsGridPos = new List<Vector2Int>();

        // we skip rows and columns at extremities
        for (int i = 1; i < _grid.GetLength(0) - 1; i++)
        {
            for (int j = 1; j < _grid.GetLength(1) - 1; j++)
            {
                //ignore void spaces
                if (_grid[i, j] == 0) continue;

                // check left, top 
                if (_grid[i, j - 1] == 0 && _grid[i - 1, j] == 0 && _grid[i - 1, j - 1] != 0)
                {
                    ReplaceInvisibleWall(i, j - 1);
                    ReplaceInvisibleWall(i - 1, j);
                }

                // check left, bottom
                if (_grid[i, j - 1] == 0 && _grid[i + 1, j] == 0 && _grid[i + 1, j - 1] != 0)
                {
                    ReplaceInvisibleWall(i, j - 1);
                    ReplaceInvisibleWall(i + 1, j);
                }

                // check right, top
                if (_grid[i, j + 1] == 0 && _grid[i - 1, j] == 0 && _grid[i - 1, j + 1] != 0)
                {
                    ReplaceInvisibleWall(i, j + 1);
                    ReplaceInvisibleWall(i - 1, j);
                }

                // check right, bottom
                if (_grid[i, j + 1] == 0 && _grid[i + 1, j] == 0 && _grid[i + 1, j + 1] != 0)
                {
                    ReplaceInvisibleWall(i, j + 1);
                    ReplaceInvisibleWall(i + 1, j);
                }
            }
        }
    }

    private void ReplaceInvisibleWall(int row, int col)
    {
        // replace if not already replaced
        if (modifiedWallsGridPos.Contains(new Vector2Int(row, col)))
        {
            // Debug.Log("Already replaced at _grid pos: [" + row + ", " + col + "]");
            return;
        }

        // Debug.Log("Need to replace at _grid pos: [" + row + ", " + col + "]");
        Vector3 worldPos = new Vector3(row * _tileSize, 0, col * _tileSize);
        GameObject invisibleWall = FindObjectAtPosition(worldPos);
        // replace if found
        if (invisibleWall != null)
        {
            Destroy(invisibleWall);
            Quaternion rot = Quaternion.identity;
            GameObject modifiedWall = CreateObject(_modifiedInvisibleWall, worldPos, rot);

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

    #region Grid Data Load
    public int[,] GetGridData()
    {
        // Loading playlist
        string playlistString = PlayerPrefs.GetString(PlayerConstants.PLAYLIST_PREF_KEY, "");
        string[] playlist = playlistString.Split(new[] { "###" }, StringSplitOptions.None);
        int playlistIndex = PlayerPrefs.GetInt(PlayerConstants.PLAYLIST_TRACKER_PREF_KEY, 0);

        // First parse the JSON string into a JObject
        JObject jsonObject = JObject.Parse(playlist[playlistIndex]);

        // Get the grid array and convert it to int[,]
        int[,] grid = jsonObject["grid"].ToObject<int[,]>();

        return grid;

    }

    #endregion

    void OnDisable()
    {
        Instance = null;
    }

}
