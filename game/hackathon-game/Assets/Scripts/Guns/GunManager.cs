using System;
using UnityEngine;
using System.Collections.Generic;

public enum FireType { Automatic, Single }
public class GunManager : MonoBehaviour
{
    [Header("Gun Specs")]
    [SerializeField] private FireType _fireStyle;
    public FireType FireStyle { get { return _fireStyle; } set { _fireStyle = value; } }
    [SerializeField] private int _magSize;
    [SerializeField] private float _fireRate;
    [SerializeField] private float _spreadAmount;
    [SerializeField] private ParticleSystem _muzzleFlash;


    [Header("Tracking Variables")]
    [SerializeField] private int _currentAmmo;
    [SerializeField] private bool _canShoot;


    [Header("Components/Object References")]
    [SerializeField] private GameObject _firePoint;
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Animator _gunAnimator;
    [SerializeField] private PlayerStateMachine _player;

    //bullet pooling
    private List<GameObject> pooledBullets = new List<GameObject>();

    private void Start()
    {
        _gunAnimator = GetComponent<Animator>();
        _player = GetComponentInParent<PlayerStateMachine>();
        _player.Gun = this;

        ReloadMag();
    }

    private void LateUpdate()
    {
        if (_player.IsShooting && _player.IsShootingAllowed) ShootGun();
    }

    public void ShootGun()
    {
        if (!_canShoot) return;

        // check ammo
        if (_currentAmmo <= 0)
        {
            _player.ReloadAttempt = true;
            return;
        }

        _canShoot = false;

        //generating random spread
        UnityEngine.Random.InitState(System.DateTime.Now.Millisecond);
        float spread = UnityEngine.Random.Range(-_spreadAmount, _spreadAmount);
        Quaternion bulletDirection = transform.root.transform.rotation * Quaternion.Euler(0, spread, 0);

        // spawn bullet
        GameObject bullet = GetPooledBullets();

        if (bullet == null)
        {
            bullet = Instantiate(_bulletPrefab, _firePoint.transform.position, bulletDirection);
            bullet.GetComponent<BulletController>().SetShooter(Shooter.Player);
            pooledBullets.Add(bullet);
        }
        else
        {
            bullet.transform.position = _firePoint.transform.position;
            bullet.transform.rotation = bulletDirection;
            bullet.SetActive(true);
        }

        // animate gun recoil
        _gunAnimator.Play("Recoil");

        // play muzzle flash
        _muzzleFlash.Play(true);

        // decrement ammo
        _currentAmmo--;

        // allow next shot at fire rate
        Invoke("AllowNextShot", _fireRate);

        // Update ui
        HUDManager._ammoUpdater(_currentAmmo);

        if (_fireStyle == FireType.Single) _player.IsShooting = false;
    }

    public void AllowNextShot() => _canShoot = true;
    public bool CanReload() => _currentAmmo != _magSize;
    public void ReloadMag()
    {
        _currentAmmo = _magSize;

        // Update ui
        HUDManager._ammoUpdater(_currentAmmo);
    }
    public void InterruptMuzzleFlash() => _muzzleFlash.Clear();

    public GameObject GetPooledBullets()
    {
        for (int i = 0; i < pooledBullets.Count; i++)
        {
            if (!pooledBullets[i].activeInHierarchy)
            {
                return pooledBullets[i];
            }
        }
        return null;
    }

    public void ClearPooledBullets()
    {
        //destroy pooled bullets
        for (int i = 0; i < pooledBullets.Count; i++)
        {
            Destroy(pooledBullets[i].gameObject);
        }
        pooledBullets.Clear();
    }
}


