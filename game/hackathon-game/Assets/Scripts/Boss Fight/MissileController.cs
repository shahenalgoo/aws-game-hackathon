using System.Collections;
using UnityEngine;

// move to position
// after countdown, crash to drop location
// reset
public class MissileController : MonoBehaviour
{
    private bool _readyPosReached = false;
    [SerializeField] private Vector3 _readyPosition;
    [SerializeField] private float _readyPosReachedThreshold = 0.1f;
    [SerializeField] private float _flySpeed = 5f;
    private Vector3 _dropPosition;
    public Vector3 DropPosition { get => _dropPosition; set => _dropPosition = value; }
    private bool _canRelease = false;
    [SerializeField] private float _delayToRelease = 2f;
    private MissileLauncher _missileLauncher;
    public MissileLauncher MissileLauncher { get => _missileLauncher; set => _missileLauncher = value; }
    private GameObject _dropZoneIndicator;
    public GameObject DropZoneIndicator { get => _dropZoneIndicator; set => _dropZoneIndicator = value; }
    private bool _hasExploded = false;
    [SerializeField] private float _explosionRadius = 5f;

    [Header("Damage")]
    [SerializeField] private int _damage = 20;
    [SerializeField] private float _knockbackForce = 1f;



    // Update is called once per frame
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
                _dropZoneIndicator.GetComponent<MeshRenderer>().enabled = true;
            }
        }

        if (_canRelease)
        {
            transform.position = Vector3.MoveTowards(
                        transform.position,
                        _dropPosition,
                        _flySpeed * Time.deltaTime
                    );

            transform.LookAt(_dropPosition);
        }
    }

    private IEnumerator StartReleaseWithDelay()
    {
        yield return new WaitForSeconds(_delayToRelease);
        _canRelease = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (_hasExploded) return;
        if (!_canRelease) return;
        if (other.gameObject.CompareTag("Player")) return;

        _hasExploded = true;

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
                    psm.ApplyKnockback(-psm.transform.forward * _knockbackForce);
                }
            }
        }

        GameObject explosion = Instantiate(_missileLauncher._explosionPrefab, transform.position, Quaternion.identity);
        Destroy(_dropZoneIndicator);
        Destroy(gameObject);

    }

    private void OnDrawGizmos()
    {
        // This will show the explosion radius in the Scene view
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _explosionRadius);
    }


}
