using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Static instance of GameManager which allows it to be accessed by any other script
    public static GameManager Instance { get; private set; }
    public GameObject _targetLoot;
    public GameObject TargetLoot { get { return _targetLoot; } }
    [SerializeField] private int _lootCollected;
    public int LootCollected { get { return _lootCollected; } }

    [SerializeField] private int _totalTargets;
    public int TotalTargets { get { return _totalTargets; } set { _totalTargets = value; } }


    [SerializeField] private bool _canCountTime = true;
    public bool CanCountTime { get { return _canCountTime; } set { _canCountTime = value; } }
    public float GameTimer { get { return _gameTimer; } }
    [SerializeField] private float _gameTimer;


    private void Awake()
    {
        SingletonCheck();
        Time.timeScale = 1f;
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

    void Update()
    {
        if (_canCountTime) TimeCounter();
    }


    public void IncrementLoot()
    {
        _lootCollected++;
        HUDManager._lootUpdater?.Invoke(_lootCollected);
    }

    public void TimeCounter()
    {
        _gameTimer += Time.deltaTime;
    }

    public bool HasCollectedAll()
    {
        return _lootCollected == _totalTargets;
    }
}
