using System.Collections;
using UnityEngine;

public class BossTurret : MonoBehaviour
{
    private Transform _player;
    private PlayerHealth _playerHealth;
    [SerializeField] private float _rotationSpeed = 5f;
    [SerializeField] private float _burstAttackInterval = 0.2f;
    [SerializeField] private int _burstAttackAmount = 5;
    public int BurstAttackAmount { get { return _burstAttackAmount; } set { _burstAttackAmount = value; } }

    private Recoil _recoil;

    [SerializeField] private Transform _spawnPoint;

    void Start()
    {
        _recoil = GetComponent<Recoil>();
        FindPlayer();
        StartRepeatingBurstAttack();
    }

    public void StartRepeatingBurstAttack()
    {
        InvokeRepeating(nameof(StartBurstAttack), 3f, 5f);
    }

    public void StartBurstAttack()
    {
        StartCoroutine(BurstAttack());
    }

    private IEnumerator BurstAttack()
    {
        for (int i = 0; i < _burstAttackAmount; i++)
        {
            yield return new WaitForSeconds(_burstAttackInterval);
            ShootPlayer();
        }
    }

    void FindPlayer()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _playerHealth = _player.gameObject.GetComponent<PlayerHealth>();
    }

    void Update()
    {

        if (_player == null) return;
        if (_playerHealth.IsDead) return;

        LookAtPlayer();
    }

    void LookAtPlayer()
    {
        // Calculate direction to player
        Vector3 directionToPlayer = _player.position - transform.position;
        directionToPlayer.y = 0; // Keep rotation only on Y axis if this is a ground unit

        // Create the rotation we want to achieve
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);

        // Smoothly rotate towards the player
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
    }

    void ShootPlayer()
    {
        // Convert the direction into a rotation
        Quaternion bulletRotation = Quaternion.LookRotation(transform.forward);
        TargetBulletManager._bulletSpawner?.Invoke(_spawnPoint.position, bulletRotation);

        // Play sfx
        AudioManager.Instance.PlaySfx(AudioManager.Instance._targetShotSfx);

        // Trigger the recoil effect
        _recoil.TriggerRecoil();
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
}
