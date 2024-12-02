using System.Collections;
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

    [SerializeField] private float _gameTimer;
    public float GameTimer { get { return _gameTimer; } }

    private void Awake()
    {
        SingletonCheck();
        InitializeGameState();
    }

    private void InitializeGameState()
    {
        _lootCollected = 0;
        _gameTimer = 0;
        HUDManager._lootUpdater?.Invoke(_lootCollected);
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
        TimeCounter();
    }


    public void IncrementLoot()
    {
        _lootCollected++;
        HUDManager._lootUpdater?.Invoke(_lootCollected);
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(0);
    }

    public void TimeCounter()
    {
        _gameTimer += Time.deltaTime;
    }
}
