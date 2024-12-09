using UnityEngine;

public class SawBladeFloorController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        int tileSize = LevelBuilder.Instance.TileSize;
        // get grid pos from world pos
        Vector2Int gridPos = new Vector2Int(Mathf.RoundToInt(transform.position.x / tileSize), Mathf.RoundToInt(transform.position.z / tileSize));


        int[,] grid = LevelBuilder.Instance.Grid;

        // if both left and right are 0, rotate floor
        if (grid[gridPos.x, gridPos.y - 1] == 0 && grid[gridPos.x, gridPos.y + 1] == 0)
        {
            transform.Rotate(0, -90f, 0);
        }
    }
}
