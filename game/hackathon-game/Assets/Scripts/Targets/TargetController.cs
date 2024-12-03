using UnityEngine;

public class TargetController : MonoBehaviour
{
    [SerializeField] private float _shootingDelayOnStart = 2f;
    [SerializeField] private float _detectionRadius = 10f;
    [SerializeField] private float _fireRate = 5f; // Seconds between shots
    private float _nextFireTime;
    private bool _canShoot = false;
    private Transform _player;
    private bool _playerInRange;
    [SerializeField] private float _rotationSpeed = 5f;
    [SerializeField] private GameObject _bulletSpawnPoint;

    void Start()
    {
        // FindPlayer();
        Invoke("FindPlayer", _shootingDelayOnStart / 2);
        Invoke("ActivateShooting", _shootingDelayOnStart);
    }

    void FindPlayer()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;

    }

    void ActivateShooting()
    {
        _canShoot = true;
    }

    // Update is called once per frame
    void Update()
    {

        if (_player == null) return;

        CheckPlayerDistance();

        if (_playerInRange)
        {
            // Calculate direction to player
            Vector3 directionToPlayer = _player.position - transform.position;
            directionToPlayer.y = 0; // Keep rotation only on Y axis if this is a ground unit

            // Create the rotation we want to achieve
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);

            // Smoothly rotate towards the player
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);

            if (Time.time >= _nextFireTime && _canShoot)
            {
                // Calculate direction from target to player
                // Vector3 playerPos = new Vector3(_player.transform.position.x, transform.position.y, _player.transform.position.z);
                // Vector3 direction = (playerPos - transform.position).normalized;

                // Convert the direction into a rotation
                Quaternion bulletRotation = Quaternion.LookRotation(transform.forward);

                // Vector3 direction = (player.position - firePoint.position).normalized;
                Vector3 spawnPos = new Vector3(transform.position.x, _player.transform.position.y, transform.position.z);
                TargetBulletManager._bulletSpawner?.Invoke(_bulletSpawnPoint.transform.position, bulletRotation);
                _nextFireTime = Time.time + _fireRate;
            }

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
