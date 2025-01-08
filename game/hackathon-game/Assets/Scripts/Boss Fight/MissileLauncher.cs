using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileLauncher : MonoBehaviour
{
    private Transform _player;
    private PlayerHealth _playerHealth;
    private Vector2Int _gridSize = new Vector2Int(4, 4);
    [SerializeField] private float _intervalBetweenMissiles = 0.5f;
    [SerializeField] private int _missileAmountPerAttack = 3;
    [SerializeField] public GameObject _missilePrefab;
    [SerializeField] public GameObject _missileDropIndicatorPrefab;
    [SerializeField] public GameObject _explosionPrefab;
    public int MissileAmountPerAttack { get { return _missileAmountPerAttack; } set { _missileAmountPerAttack = value; } }

    [Header("Getting a ref to the boss' positions so we can ignore it when calculating target locations")]
    [SerializeField] private Vector2Int _bossPos;
    [SerializeField] private List<Vector2Int> _targetPositions;
    [SerializeField] private Vector2Int _playerGridPos;
    private List<Vector2Int> _occupiedPositions = new List<Vector2Int>();


    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _playerGridPos = GetPlayerGrisPosition();
        _targetPositions = new List<Vector2Int>();
        GetAvailableTargetPositions();
        StartRepeatingAttack();
    }

    Vector2Int GetPlayerGrisPosition()
    {
        Vector2Int playerGridPos = Helpers.GetGridPosition(_player, 8);
        // adjusting for boss scene
        return new Vector2Int(playerGridPos.x - 1, playerGridPos.y);
    }

    void Update()
    {
        Vector2Int currentPlayerGridPos = GetPlayerGrisPosition();

        if (currentPlayerGridPos != _playerGridPos)
        {
            _playerGridPos = currentPlayerGridPos;
            // This means we went to a new grid position
            GetAvailableTargetPositions();
        }
    }

    public void GetAvailableTargetPositions()
    {
        _targetPositions = GetAllAvailableTargetPositions();
    }

    public void StartRepeatingAttack()
    {
        InvokeRepeating(nameof(StartMissileAttack), 1f, 5f);
    }

    public void StartMissileAttack()
    {
        StartCoroutine(ShootMissiles());
    }

    public void StopAttack()
    {
        CancelInvoke();
        StopAllCoroutines();

    }

    public void OnDisable()
    {
        StopAttack();
    }

    private IEnumerator ShootMissiles()
    {
        // Shoot missiles
        for (int i = 0; i < _missileAmountPerAttack; i++)
        {
            yield return new WaitForSeconds(_intervalBetweenMissiles);
            // Shoot missile
            GameObject missile = Instantiate(_missilePrefab, transform.position, Quaternion.identity);
            MissileController missileController = missile.GetComponent<MissileController>();

            missileController.ReadyPosition = new Vector3(transform.position.x, 40f, transform.position.z);
            missileController.MissileLauncher = this;

            // Play sfx
            AudioManager.Instance.PlaySfx(AudioManager.Instance._missileLaunchSfx);
        }
    }

    private List<Vector2Int> GetAllAvailableTargetPositions()
    {
        List<Vector2Int> targetPositions = new List<Vector2Int>();

        // First, add player's position if it's not the boss position
        if (_playerGridPos != _bossPos && !_occupiedPositions.Contains(_playerGridPos))
        {
            targetPositions.Add(_playerGridPos);
        }

        // Check all adjacent positions (including diagonals)
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                // Skip the center position as it's already added
                if (x == 0 && y == 0) continue;

                Vector2Int adjacentPos = new Vector2Int(
                    _playerGridPos.x + x,
                    _playerGridPos.y + y
                );

                // Add position if it's within grid bounds and not the boss position
                if (adjacentPos.x >= 0 && adjacentPos.x < _gridSize.x &&
                    adjacentPos.y >= 0 && adjacentPos.y < _gridSize.y &&
                    adjacentPos != _bossPos
                    && !_occupiedPositions.Contains(_playerGridPos))
                {
                    targetPositions.Add(adjacentPos);
                }
            }
        }

        return targetPositions;
    }

    public Vector2Int GetTargetPosition()
    {
        // If no positions available, return an invalid position
        if (_targetPositions.Count == 0)
        {
            GetAvailableTargetPositions();

            if (_targetPositions.Count == 0)
            {
                return new Vector2Int(-1, -1);
            }
        }

        Vector2Int targetPos;

        // Check if player grid pos is available
        if (_targetPositions.Contains(_playerGridPos))
        {
            targetPos = _playerGridPos;
        }
        else
        {
            // Get random position from available positions
            int randomIndex = Random.Range(0, _targetPositions.Count);
            targetPos = _targetPositions[randomIndex];
        }

        // Mark the position as occupied
        _occupiedPositions.Add(targetPos);
        _targetPositions.Remove(targetPos);
        return targetPos;
    }

    public void FreeOccupiedPosition(Vector2Int gridPos)
    {
        _occupiedPositions.Remove(gridPos);
    }


}
