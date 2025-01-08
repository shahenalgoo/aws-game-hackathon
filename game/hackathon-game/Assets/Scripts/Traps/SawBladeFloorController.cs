using UnityEngine;

public class SawBladeFloorController : MonoBehaviour
{
    private FMODUnity.StudioEventEmitter _eventEmitter;
    private void Awake()
    {
        _eventEmitter = GetComponent<FMODUnity.StudioEventEmitter>();
    }

    void Start()
    {
        // get grid pos from world pos
        Vector2Int gridPos = Helpers.GetGridPosition(transform, LevelBuilder.Instance.TileSize);
        int[,] grid = LevelBuilder.Instance.Grid;

        int horizontalConnections = 0;
        int verticalConnections = 0;

        //check left
        if (gridPos.y > 0 && grid[gridPos.x, gridPos.y - 1] != 0) horizontalConnections++;

        //check right
        if (gridPos.y < grid.GetLength(1) - 1 && grid[gridPos.x, gridPos.y + 1] != 0) horizontalConnections++;

        // check top
        if (gridPos.x > 0 && grid[gridPos.x - 1, gridPos.y] != 0) verticalConnections++;

        // check bottom
        if (gridPos.x < grid.GetLength(0) - 1 && grid[gridPos.x + 1, gridPos.y] != 0) verticalConnections++;

        // if vertical has more floors, rotate trap
        if (verticalConnections > horizontalConnections)
        {
            transform.Rotate(0, -90f, 0);
        }
    }

    private void OnDestroy()
    {
        if (_eventEmitter != null)
        {
            _eventEmitter.Stop();
        }
    }
}
