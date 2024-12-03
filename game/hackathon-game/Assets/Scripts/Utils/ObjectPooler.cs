using System.Collections.Generic;
using UnityEngine;

public struct PooledObjType
{
    public GameObject obj;
    public bool isNewlyCreated;
}

public class ObjectPooler : MonoBehaviour
{
    protected List<GameObject> _pooledObjects = new List<GameObject>();
    [SerializeField] protected GameObject _objectPrefab;

    public PooledObjType GetObject()
    {
        // spawn obj
        GameObject gobj = GetPooledObjects();
        bool isNew = false;

        if (gobj == null)
        {
            gobj = Instantiate(_objectPrefab, transform.position, Quaternion.identity);
            _pooledObjects.Add(gobj);
            isNew = true;
        }

        PooledObjType pooledObj = new PooledObjType
        {
            obj = gobj,
            isNewlyCreated = isNew
        };
        return pooledObj;
    }
    public GameObject GetPooledObjects()
    {
        for (int i = 0; i < _pooledObjects.Count; i++)
        {
            if (!_pooledObjects[i].activeInHierarchy)
            {
                return _pooledObjects[i];
            }
        }
        return null;
    }

    public void ClearPooledObjects()
    {
        //destroy pooled objs
        for (int i = 0; i < _pooledObjects.Count; i++)
        {
            Destroy(_pooledObjects[i].gameObject);
        }
        _pooledObjects.Clear();
    }
}
