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
    public int MissileAmountPerAttack { get { return _missileAmountPerAttack; } set { _missileAmountPerAttack = value; } }
    private int[,] _gridSize = new int[4, 4];

    [Header("Getting a ref to the boss' positions so we can ignore it when calculating target locations")]
    [SerializeField] private Vector2Int _bossPos;

    [Header("Explosion")]
    [SerializeField] public GameObject _explosionPrefab;

    private MissileController[] _activeMissiles;
    [SerializeField] private Vector3[] _dropLocations;
    private bool _newBatch;
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        StartRepeatingAttack();
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
        StopCoroutine(ShootMissiles());

    }

    public void OnDisable()
    {
        StopAttack();
    }

    private IEnumerator ShootMissiles()
    {
        _newBatch = true;
        _activeMissiles = new MissileController[_missileAmountPerAttack];
        _dropLocations = new Vector3[_missileAmountPerAttack];

        // Shoot missiles
        for (int i = 0; i < _missileAmountPerAttack; i++)
        {
            yield return new WaitForSeconds(_intervalBetweenMissiles);
            // Shoot missile
            GameObject missile = Instantiate(_missilePrefab, transform.position, Quaternion.identity);
            MissileController missileController = missile.GetComponent<MissileController>();

            missileController.MissileLauncher = this;
            missileController.MissileIndex = i;
            missileController.ReadyPosition = new Vector3(transform.position.x, 40f, transform.position.z);
            _activeMissiles[i] = missileController;

            // Play sfx
            AudioManager.Instance.PlaySfx(AudioManager.Instance._missileLaunchSfx);
        }
    }

    public void GetDropLocation(int missileIndex)
    {
        // The drop locations will be chosen on the first request only by missile index 0
        if (_newBatch)
        {
            // Spawn drop zones
            Vector2Int playerGridPos = Helpers.GetGridPosition(_player, 8);
            playerGridPos = new Vector2Int(playerGridPos.x - 1, playerGridPos.y);
            List<Vector2Int> targetPositions = GetMissileTargetGridPositions(playerGridPos);

            for (int i = 0; i < targetPositions.Count; i++)
            {
                try
                {
                    Vector3 worldPos = new Vector3((targetPositions[i].x + 1f) * 8f, 0f, targetPositions[i].y * 8f);
                    _dropLocations[i] = worldPos;
                }
                catch (System.Exception)
                {
                    Debug.LogWarning("Missile index error: " + i);

                    throw;
                }
            }
            _newBatch = false;
        }

        GameObject[] dropZoneIndicators = new GameObject[_missileAmountPerAttack];
        GameObject dropZone = Instantiate(_missileDropPointer, _dropLocations[missileIndex], Quaternion.identity);
        dropZone.GetComponent<MeshRenderer>().enabled = false;
        dropZoneIndicators[missileIndex] = dropZone;
        _activeMissiles[missileIndex].DropPosition = _dropLocations[missileIndex];
        _activeMissiles[missileIndex].DropZoneIndicator = dropZoneIndicators[missileIndex];
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
            validPositions.RemoveAt(randomIndex);
        }

        return targetPositions;
    }


}
