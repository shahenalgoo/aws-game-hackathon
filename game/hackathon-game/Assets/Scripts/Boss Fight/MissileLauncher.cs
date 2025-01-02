using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileLauncher : MonoBehaviour
{
    private Transform _player;
    private PlayerHealth _playerHealth;
    [SerializeField] private GameObject _missilePrefab;
    [SerializeField] private GameObject _missileDropPointer;
    [SerializeField] private float _intervalBetweenMissiles = 0.5f;
    [SerializeField] private int _missileAmountPerAttack = 3;
    private int[,] _gridSize = new int[4, 4];

    [Header("Getting a ref to the boss' positions so we can ignore it when calculating target locations")]
    [SerializeField] private Vector2Int _bossPos;
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        InvokeRepeating(nameof(StartMissileAttack), 3f, 5f);
    }

    public void StartMissileAttack()
    {
        StartCoroutine(MissileAttack());
    }

    private IEnumerator MissileAttack()
    {
        // Spawn drop zones
        Vector2Int playerGridPos = Helpers.GetGridPosition(_player, 8);
        playerGridPos = new Vector2Int(playerGridPos.x - 1, playerGridPos.y);
        List<Vector2Int> targetPositions = GetMissileTargetGridPositions(playerGridPos);
        List<Vector3> targetWorldPositions = new();
        for (int i = 0; i < targetPositions.Count; i++)
        {
            Vector3 worldPos = new Vector3((targetPositions[i].x + 1f) * 8f, 0f, targetPositions[i].y * 8f);
            targetWorldPositions.Add(worldPos);
            GameObject missilePointer = Instantiate(_missileDropPointer, worldPos, Quaternion.identity);
            Destroy(missilePointer, 2f);
        }

        // Shoot missiles
        for (int i = 0; i < _missileAmountPerAttack; i++)
        {
            yield return new WaitForSeconds(_intervalBetweenMissiles);
            // Shoot missile
            GameObject missile = Instantiate(_missilePrefab, transform.position, Quaternion.identity);
            missile.GetComponent<MissileController>().DropPosition = targetWorldPositions[i];
        }
    }

    private List<Vector2Int> GetMissileTargetGridPositions(Vector2Int playerGridPos)
    {
        List<Vector2Int> targetPositions = new List<Vector2Int>();
        List<Vector2Int> validPositions = new List<Vector2Int>();
        int remainingPositionsToSelect = _missileAmountPerAttack;

        // Always add player's position first
        if (playerGridPos != _bossPos)
        {
            targetPositions.Add(playerGridPos);
            remainingPositionsToSelect--;
        }

        // All possible adjacent positions (including diagonals)
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                // Skip the player's current position (0,0)
                if (x == 0 && y == 0) continue;

                Vector2Int adjacentPos = new Vector2Int(
                    playerGridPos.x + x,
                    playerGridPos.y + y
                );

                // Check if the position is within grid bounds (4x4)
                if (adjacentPos.x >= 0 && adjacentPos.x < 4 &&
                    adjacentPos.y >= 0 && adjacentPos.y < 4 &&
                    adjacentPos != _bossPos)
                {
                    validPositions.Add(adjacentPos);
                }
            }
        }

        // Randomly select remaining positions (total should be _missileAmountPerAttack)
        int positionsToSelect = Mathf.Min(remainingPositionsToSelect, validPositions.Count);

        for (int i = 0; i < positionsToSelect; i++)
        {
            int randomIndex = Random.Range(0, validPositions.Count);
            targetPositions.Add(validPositions[randomIndex]);
            Debug.Log(validPositions[randomIndex]);
            validPositions.RemoveAt(randomIndex);
        }

        return targetPositions;
    }


}
