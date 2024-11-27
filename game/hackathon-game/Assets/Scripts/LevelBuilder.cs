using UnityEngine;

public class LevelBuilder : MonoBehaviour
{
    [SerializeField]
    private int tileSize = 2;

    [SerializeField]
    private GameObject[] levelObjects;

    private float yAdjustObject = -5f;
    private float yAdjustTarget = 1f;


    private int[,] grid;


    // Start is called before the first frame update
    void Start()
    {
        grid = new int[,] {
            { 0, 0, 0, 2, 4, 1, 0, 0, 0, 6, 2, 0, 0, 0, 1, 5, 1, 0, 2, 7 },
            { 0, 1, 0, 1, 0, 2, 3, 1, 0, 3, 0, 0, 2, 1, 2, 0, 1, 2, 1, 0 },
            { 0, 2, 0, 0, 0, 0, 0, 1, 4, 1, 0, 0, 1, 0, 0, 0, 0, 5, 0, 0 },
            { 0, 1, 4, 2, 1, 0, 0, 0, 0, 2, 4, 1, 5, 0, 1, 3, 1, 2, 6, 0 },
            { 1, 2, 0, 0, 5, 1, 3, 6, 0, 0, 0, 0, 1, 4, 2, 0, 0, 0, 2, 1 },
            { 0, 0, 0, 0, 0, 0, 1, 5, 1, 5, 1, 0, 0, 0, 1, 3, 1, 0, 0, 0 },
            { 0, 1, 4, 2, 0, 0, 0, 0, 2, 0, 1, 3, 1, 0, 0, 0, 2, 1, 0, 8 },
            { 0, 0, 0, 3, 1, 2, 1, 0, 4, 0, 0, 0, 2, 4, 1, 0, 0, 5, 1, 1 },
            { 0, 0, 0, 1, 0, 0, 2, 5, 6, 3, 2, 0, 0, 0, 2, 1, 0, 0, 2, 0 },
        };

        BuildLevel();
    }

    void BuildLevel()
    {
        GridResolver resolver = new GridResolver(grid);
        grid = resolver.FixIsolatedRegions(4, 0);

        // create 2d array for loop
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                int objectId = grid[i, j];

                if (objectId == 0)
                {
                    // check if at least 1 neighbor is not zero, place invisible wall so player cannot fall.
                    if (HasNonZeroNeighbor(i, j))
                    {
                        Vector3 wallPos = new Vector3(i * tileSize, 0, j * tileSize);
                        CreateObject(levelObjects[objectId], wallPos, Quaternion.identity);
                    }
                    continue;
                }


                Vector3 pos = new Vector3(i * tileSize, yAdjustObject, j * tileSize);

                GameObject obj = CreateObject(levelObjects[objectId], pos, Quaternion.identity);

                // If a target or bonus teleporter
                if (objectId == 2 || objectId == 7 || objectId == 8)
                {
                    // Add a floor underneath
                    CreateObject(levelObjects[1], pos, Quaternion.identity);


                    // Adjust y of obj
                    obj.transform.position = new Vector3(pos.x, yAdjustTarget, pos.z);
                }

                // If object is in extremity, Add wall in extreme row and column
                if (i == 0 || i == grid.GetLength(0) - 1 || j == 0 || j == grid.GetLength(1) - 1) AddWallInExtremity(i, j);


            }
        }
    }

    private bool HasNonZeroNeighbor(int row, int col)
    {
        int rows = grid.GetLength(0);
        int cols = grid.GetLength(1);

        // Check up
        if (row > 0 && grid[row - 1, col] != 0) return true;

        // Check down
        if (row < rows - 1 && grid[row + 1, col] != 0) return true;

        // Check left
        if (col > 0 && grid[row, col - 1] != 0) return true;

        // Check right
        if (col < cols - 1 && grid[row, col + 1] != 0) return true;

        return false;
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

}
