using System;
using UnityEngine;

public class BulletImpactManager : ObjectPooler
{
    public static Action<Vector3, Quaternion> _impactSpawner;

    public void OnEnable()
    {
        _impactSpawner += SpawnImpact;
    }

    public void OnDisable()
    {
        _impactSpawner -= SpawnImpact;

    }
    public void SpawnImpact(Vector3 spawnPoint, Quaternion rotation)
    {
        PooledObjType impact = GetObject();

        impact.obj.transform.position = spawnPoint;
        impact.obj.transform.rotation = rotation;

        if (impact.isNewlyCreated)
        {
            impact.obj.transform.SetParent(transform);
        }
        else
        {
            impact.obj.gameObject.SetActive(true);
        }


    }
}
