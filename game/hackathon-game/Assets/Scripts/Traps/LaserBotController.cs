using UnityEngine;

public class LaserBotController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 2f;
    [SerializeField] private float _padding = 0.5f; // to be removed from bounds
    private float _minBoundZAxis;
    private float _maxBoundZAxis;
    private int _direction = 1;

    private FMODUnity.StudioEventEmitter _eventEmitter;

    private void Awake()
    {
        _eventEmitter = GetComponent<FMODUnity.StudioEventEmitter>();
    }
    void Start()
    {
        // get grid pos from world pos
        Vector2Int gridPos = Helpers.GetGridPosition(transform);
        int[,] grid = LevelBuilder.Instance.Grid;

        // count all the non-zero floors to the left
        int leftSide = 0;
        for (int i = 1; i <= gridPos.y; i++)
        {
            if (grid[gridPos.x, gridPos.y - i] != 0)
            {
                leftSide += 1;
            }
            else
            {
                break;
            }
        }

        // count to the right
        int rightSide = 0;
        for (int i = gridPos.y + 1; i < grid.GetLength(1); i++)
        {
            if (grid[gridPos.x, i] != 0)
            {
                rightSide += 1;
            }
            else
            {
                break;
            }
        }

        // count top
        int topSide = 0;
        for (int i = 1; i <= gridPos.x; i++)
        {
            if (grid[gridPos.x - i, gridPos.y] != 0)
            {
                topSide += 1;
            }
            else
            {
                break;
            }
        }

        // count down 
        int bottomSide = 0;
        for (int i = gridPos.x + 1; i < grid.GetLength(0); i++)
        {
            if (grid[i, gridPos.y] != 0)
            {
                bottomSide += 1;
            }
            else
            {
                break;
            }
        }

        // Debug.Log("Grid Pos: " + gridPos + ", total left: " + leftSide + ", total right: " + rightSide + ", total top: " + topSide + ", total bottom: " + bottomSide);

        // Compare columns/corridors
        int tileSize = LevelBuilder.Instance.TileSize;

        if (leftSide + rightSide >= topSide + bottomSide)
        {
            // This means the horizontal corridor is longer (or same size), so we will make the bots move in this direction
            _minBoundZAxis = ((0.5f * tileSize) + (leftSide * tileSize) - _padding) * -1f;
            _maxBoundZAxis = (0.5f * tileSize) + (rightSide * tileSize) - _padding;
        }
        else
        {
            // Rotate floor 90 degrees since we want to go top/bottom
            transform.parent.transform.Rotate(0, 90f, 0);

            _minBoundZAxis = ((0.5f * tileSize) + (topSide * tileSize) - _padding) * -1f;
            _maxBoundZAxis = (0.5f * tileSize) + (bottomSide * tileSize) - _padding;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // motion code
        // float newZPosition = Mathf.PingPong(Time.deltaTime * _moveSpeed, _maxBoundZAxis - _minBoundZAxis) + _minBoundZAxis;
        // transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, newZPosition);

        // Move the object
        transform.Translate(0, 0, _moveSpeed * _direction * Time.deltaTime);

        // Check bounds and reverse direction
        if (transform.localPosition.z >= _maxBoundZAxis)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, _maxBoundZAxis);
            _direction = -1;
        }
        else if (transform.localPosition.z <= _minBoundZAxis)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, _minBoundZAxis);
            _direction = 1;
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
