using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                // Search for existing instance
                _instance = (T)FindObjectOfType(typeof(T));

                // Create new instance if one doesn't already exist
                if (_instance == null)
                {
                    // Create new GameObject
                    GameObject singletonObject = new GameObject();
                    _instance = singletonObject.AddComponent<T>();
                    singletonObject.name = $"{typeof(T)} (Singleton)";
                }
            }

            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this as T;
    }

}
