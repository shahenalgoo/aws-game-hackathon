using System.Collections;
using UnityEngine;

public class MissileController : MonoBehaviour
{
    private bool _readyPosReached = false;
    private Vector3 _readyPosition;
    public Vector3 ReadyPosition { get { return _readyPosition; } set { _readyPosition = value; } }
    [SerializeField] private float _readyPosReachedThreshold = 0.1f;
    [SerializeField] private float _flySpeed = 5f;
    private Vector2Int _targetGridPos;
    private Vector3 _targetPosition;
    private bool _canRelease = false;
    [SerializeField] private float _delayToRelease = 2f;
    [SerializeField] private float _delayToRetry = 2f;
    private MissileLauncher _missileLauncher;
    public MissileLauncher MissileLauncher { set { _missileLauncher = value; } }
    private GameObject _dropZoneIndicator;

    [Header("Damage")]
    [SerializeField] private int _damage = 20;
    [SerializeField] private float _knockbackForce = 1f;
    [SerializeField] private float _explosionRadius = 5f;

    void Update()
    {

        if (!_readyPosReached)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                _readyPosition,
                _flySpeed * Time.deltaTime
            );

            transform.LookAt(_readyPosition);

            if (Vector3.Distance(transform.position, _readyPosition) < _readyPosReachedThreshold)
            {
                _readyPosReached = true;
                StartCoroutine(StartReleaseWithDelay());

            }
        }

        if (_canRelease)
        {
            transform.position = Vector3.MoveTowards(
                        transform.position,
                        _targetPosition,
                        _flySpeed * Time.deltaTime
                    );

            transform.LookAt(_targetPosition);
        }
    }

    private IEnumerator StartReleaseWithDelay()
    {
        _targetGridPos = _missileLauncher.GetTargetPosition();

        // handle no available positions
        if (_targetGridPos == new Vector2Int(-1, -1))
        {
            Debug.Log("No available positions, trying again soon");
            yield return new WaitForSeconds(_delayToRetry);
            StartCoroutine(StartReleaseWithDelay());
            yield break;
        }

        _targetPosition = new Vector3((_targetGridPos.x + 1f) * 8f, 0f, _targetGridPos.y * 8f);
        _dropZoneIndicator = Instantiate(_missileLauncher._missileDropIndicatorPrefab, _targetPosition, Quaternion.identity);

        yield return new WaitForSeconds(_delayToRelease);
        _canRelease = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Floor"))
        {
            // Create overlap sphere check
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, _explosionRadius);
            foreach (Collider hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Player"))
                {
                    // Player was caught in explosion radius
                    PlayerHealth playerHealth = hitCollider.gameObject.GetComponent<PlayerHealth>();
                    playerHealth.TakeDamage(_damage);
                    playerHealth.DamageVfx.Play();

                    // Apply the knockback through your player movement script
                    PlayerStateMachine psm = hitCollider.gameObject.GetComponent<PlayerStateMachine>();
                    if (psm != null)
                    {
                        Vector3 dir = (psm.transform.position - transform.position).normalized;
                        psm.ApplyKnockback(dir * _knockbackForce);
                    }
                }
            }

            // Free space
            _missileLauncher.FreeOccupiedPosition(_targetGridPos);

            // Explosion
            Instantiate(_missileLauncher._explosionPrefab, transform.position, Quaternion.identity);

            // Play sfx
            AudioManager.Instance.PlaySfx(AudioManager.Instance._missileExplodeSfx);

            Destroy(_dropZoneIndicator);
            Destroy(gameObject);
        };


    }

    private void OnDrawGizmos()
    {
        // This will show the explosion radius in the Scene view
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _explosionRadius);
    }


}
