using UnityEngine;

public class TargetController : MonoBehaviour
{
    [SerializeField] private float _delayOnStart = 2f;

    [SerializeField] private float _detectionRadius = 10f;
    [SerializeField] private float _fireRate = 5f; // Seconds between shots
    private Transform _player;
    private float _nextFireTime;
    private bool _playerInRange;

    void Start()
    {
        Invoke("FindPlayer", _delayOnStart);
    }

    void FindPlayer()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;

    }

    // Update is called once per frame
    void Update()
    {

        if (_player == null) return;

        CheckPlayerDistance();

        if (_playerInRange && Time.time >= _nextFireTime)
        {
            // Calculate direction from target to player
            Vector3 playerPos = new Vector3(_player.transform.position.x, transform.position.y, _player.transform.position.z);
            Vector3 direction = (playerPos - transform.position).normalized;

            // Convert the direction into a rotation
            Quaternion bulletRotation = Quaternion.LookRotation(direction);

            // Vector3 direction = (player.position - firePoint.position).normalized;
            Vector3 spawnPos = new Vector3(transform.position.x, _player.transform.position.y, transform.position.z);
            TargetBulletManager._bulletSpawner?.Invoke(transform.position, bulletRotation);
            _nextFireTime = Time.time + _fireRate;
        }
    }

    void CheckPlayerDistance()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, _player.position);
        _playerInRange = distanceToPlayer <= _detectionRadius;
    }


    // Optional: Visualize the detection radius in the editorß
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _detectionRadius);
    }
}
