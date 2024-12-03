using System;
using UnityEngine;

public class TargetBulletManager : ObjectPooler
{
    public static Action<Vector3, Quaternion> _bulletSpawner;

    public void OnEnable()
    {
        _bulletSpawner += SpawnBullet;
    }

    public void OnDisable()
    {
        _bulletSpawner -= SpawnBullet;

    }
    public void SpawnBullet(Vector3 spawnPoint, Quaternion direction)
    {
        PooledObjType bullet = GetObject();

        bullet.obj.transform.position = spawnPoint;
        bullet.obj.transform.rotation = direction;

        if (!bullet.isNewlyCreated)
        {
            bullet.obj.SetActive(true);
        }
    }
}
