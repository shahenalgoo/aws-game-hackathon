using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetBulletManager : MonoBehaviour
{
    public static TargetBulletManager Instance { get; private set; }

    private List<GameObject> pooledBullets = new List<GameObject>();
    public GameObject _targetBulletPrefab;

    private void Awake()
    {
        SingletonCheck();
    }

    void SingletonCheck()
    {
        // If there is an instance, and it's not this one, delete this one
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Set the instance
        Instance = this;
    }
    public void SpawnBullet(Vector3 spawnPoint, Quaternion direction)
    {
        // spawn bullet
        GameObject bullet = GetPooledBullets();

        if (bullet == null)
        {
            bullet = Instantiate(_targetBulletPrefab, spawnPoint, direction);
            bullet.GetComponent<BulletController>().SetShooter(Shooter.Target);
            pooledBullets.Add(bullet);
        }
        else
        {
            bullet.transform.position = spawnPoint;
            bullet.transform.rotation = direction;
            bullet.SetActive(true);
        }
    }
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
