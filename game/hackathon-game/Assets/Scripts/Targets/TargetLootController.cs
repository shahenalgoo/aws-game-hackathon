using UnityEngine;

public class TargetLootController : MonoBehaviour
{
    [SerializeField] private float _magneticRange = 5f; // Range at which magnetic effect starts
    [SerializeField] private float _moveSpeed = 10f; // Speed at which item moves to _player
    private Transform _player;
    private bool _isBeingPulled = false;
    private Vector2Int _gridPos;

    [SerializeField] private ParticleSystem _lootCollectVFX;
    [SerializeField] private GameObject _trails;

    private void Start()
    {
        // Get reference to the _player - assuming there's only one _player with "Player" tag
        _player = GameObject.FindGameObjectWithTag("Player").transform;

        // Get grid pos from world pos
        _gridPos = Helpers.GetGridPosition(transform, LevelBuilder.Instance.TileSize);

    }

    private void Update()
    {
        if (_player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, _player.position);

            // Once pulled, keep pulling regardless of distance
            if (!_isBeingPulled && distanceToPlayer <= _magneticRange)
            {
                _isBeingPulled = true;

                // activate trails
                _trails.SetActive(true);

                // stop hovering
                GetComponent<Animator>().Play("Stop");
            }

            // If being pulled, continue pulling regardless of distance
            if (_isBeingPulled)
            {
                Vector3 targetPosition = _player.position + new Vector3(0, 1, 0); // Slightly above _player
                transform.parent.LookAt(targetPosition);
                transform.parent.position = Vector3.MoveTowards(
                    transform.parent.position,
                    targetPosition,
                    _moveSpeed * Time.deltaTime
                );
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameManager.Instance?.IncrementLoot();

            // Spawn VFX
            _lootCollectVFX.transform.parent = null;
            _lootCollectVFX.Play();

            // Play SFX
            AudioManager.Instance.PlaySfx(AudioManager.Instance._starCollectedSfx);

            // Update Minimap

            Minimap minimap = FindObjectOfType<Minimap>();
            minimap.AmendGrid(_gridPos, 1);


            Destroy(gameObject);
        }
    }
}
