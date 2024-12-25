using UnityEngine;

public class SawBladeFloorController : MonoBehaviour
{
    private FMODUnity.StudioEventEmitter _eventEmitter;
    private void Awake()
    {
        _eventEmitter = GetComponent<FMODUnity.StudioEventEmitter>();
    }
    // Start is called before the first frame update
    void Start()
    {
        // get grid pos from world pos
        Vector2Int gridPos = Helpers.GetGridPosition(transform);
        int[,] grid = LevelBuilder.Instance.Grid;

        // if both left and right are 0, rotate floor
        if (CheckWithinColumnRange(gridPos, grid) && grid[gridPos.x, gridPos.y - 1] == 0 && grid[gridPos.x, gridPos.y + 1] == 0)
        {
            transform.Rotate(0, -90f, 0);
        }
    }

    private bool CheckWithinColumnRange(Vector2Int gridPos, int[,] grid)
    {
        if (gridPos.y == 0 || gridPos.y == grid.GetLength(1) - 1)
        {
            return false;
        }

        return true;
    }

    private void OnDestroy()
    {
        if (_eventEmitter != null)
        {
            _eventEmitter.Stop();
        }
    }
}
