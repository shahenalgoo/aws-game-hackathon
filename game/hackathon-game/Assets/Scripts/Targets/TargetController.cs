using System;
using System.Collections;
using UnityEngine;

public class TargetController : MonoBehaviour
{
    [SerializeField] private float _shootingDelayOnStart = 2f;
    [SerializeField] private float _detectionRadius = 10f;
    [SerializeField] private float _meleeAttackRadius = 1.5f;
    private bool _canMelee = true;
    public bool CanMelee => _canMelee;
    [SerializeField] private float _meleeCooldown = 2f;
    [SerializeField] private Animator _meleeAnimator;
    public Animator MeleeAnimator => _meleeAnimator;

    [SerializeField] private float _fireRate = 5f; // Seconds between shots
    private float _cooldown = 0f;
    private bool _canShoot = false;
    private Transform _player;
    public Transform Player { get { return _player; } set { _player = value; } }
    private PlayerHealth _playerHealth;
    [SerializeField] private float _rotationSpeed = 5f;
    [SerializeField] private GameObject _bulletSpawnPoint;
    [SerializeField] private GameObject _targetBarrel;
    [SerializeField] private GameObject _meleeObject;
    public GameObject TargetBarrel => _targetBarrel;

    void Start()
    {
        Invoke("FindPlayer", _shootingDelayOnStart / 2);
        Invoke("ActivateShooting", _shootingDelayOnStart);
        _meleeObject.SetActive(false);
    }
    void FindPlayer()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _playerHealth = _player.gameObject.GetComponent<PlayerHealth>();

    }

    void ActivateShooting()
    {
        _canShoot = true;
        _cooldown = 0f;
    }

    // Update is called once per frame
    void Update()
    {

        if (_player == null) return;
        if (_playerHealth.IsDead) return;

        LookAtPlayer();

        if (_canShoot)
        {
            if (CheckPlayerDistance(_detectionRadius)) ShootPlayer();
        }
        else
        {
            _cooldown += Time.deltaTime;

            if (_cooldown >= _fireRate)
            {
                ActivateShooting();
            }
        }

        // Check for melee attack
        if (_canMelee && CheckPlayerDistance(_meleeAttackRadius))
        {
            TriggerMeleeAttack();
        }
    }

    void LookAtPlayer()
    {
        // Calculate direction to player
        // Vector3 playerPos = _player.position + new Vector3(0, 1f, 0);
        // Vector3 directionToPlayer = playerPos - _targetBarrel.transform.position;
        Vector3 directionToPlayer = _player.position - _targetBarrel.transform.position;
        directionToPlayer.y = 0; // Keep rotation only on Y axis if this is a ground unit

        // Create the rotation we want to achieve
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);

        // Smoothly rotate towards the player
        _targetBarrel.transform.rotation = Quaternion.Slerp(_targetBarrel.transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
    }

    bool CheckPlayerDistance(float minDistance)
    {
        float distanceToPlayer = Vector3.Distance(transform.position, _player.position);
        return distanceToPlayer <= minDistance;
    }

    void ShootPlayer()
    {
        // Convert the direction into a rotation
        Quaternion bulletRotation = Quaternion.LookRotation(_targetBarrel.transform.forward);
        TargetBulletManager._bulletSpawner?.Invoke(_bulletSpawnPoint.transform.position, bulletRotation);

        // Play sfx
        AudioManager.Instance.PlaySfx(AudioManager.Instance._targetShotSfx);

        _canShoot = false;
    }

    void TriggerMeleeAttack()
    {
        _canMelee = false;
        _meleeAnimator.Play("SpinAttack");
        StartCoroutine(PlayMeleeSfx());
        StartCoroutine(ResetMelee());
        _meleeObject.SetActive(true);
    }

    private IEnumerator PlayMeleeSfx()
    {
        // Play sfx
        AudioManager.Instance.PlaySfx(AudioManager.Instance._targetMeleeSfx);
        yield return new WaitForSeconds(0.5f);
        // Play sfx
        AudioManager.Instance.PlaySfx(AudioManager.Instance._targetMeleeSfx);

    }

    private IEnumerator ResetMelee()
    {
        yield return new WaitForSeconds(_meleeCooldown);
        _canMelee = true;
        _meleeObject.SetActive(false);

    }

    // Optional: Visualize the detection radius in the editorß
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _detectionRadius);
    }
}
