using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;

public class GameManager : MonoBehaviour
{
    // Static instance of GameManager which allows it to be accessed by any other script
    public static GameManager Instance { get; private set; }

    [Header("TARGET & REWARD LOOT")]
    [SerializeField] private int _totalTargets;
    public int TotalTargets { get { return _totalTargets; } set { _totalTargets = value; } }
    [SerializeField] private int _lootCollected;
    public int LootCollected { get { return _lootCollected; } }
    [SerializeField] private GameObject _targetLoot;
    public GameObject TargetLoot { get { return _targetLoot; } }

    [Header("TIMER")]
    [SerializeField] private bool _canCountTime = true;
    public bool CanCountTime { get { return _canCountTime; } set { _canCountTime = value; } }
    public float GameTimer { get { return _gameTimer; } }
    [SerializeField] private float _gameTimer;

    [Header("PLAYER OPTIONS")]
    [SerializeField] private bool _usePlayerEntranceAnimation;
    public bool UsePlayerEntranceAnimation { get => _usePlayerEntranceAnimation; }


    /** EXTERNAL COMM **/
    [DllImport("__Internal")]
    private static extern void SubmitScore(float time);
    private void Awake()
    {
        SingletonCheck();

        // Clear possible previous states
        Time.timeScale = 1f;
        if (AudioManager.Instance != null) AudioManager.Instance.PauseAudio(false);
    }

    public void Start()
    {
        _gameTimer = PlayerPrefs.GetFloat(PlayerConstants.TIMER_PREF_KEY, 0f);
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

        if (_lootCollected == _totalTargets)
        {
            // activate platform
            LevelBuilder.Instance.ExtractionController.ActivatePlatform();
        }
    }

    public void TimeCounter()
    {
        _gameTimer += Time.deltaTime;
    }

    public bool HasCollectedAll()
    {
        return _lootCollected == _totalTargets;
    }

    public void GoToNextLevel()
    {
        // Save time
        PlayerPrefs.SetFloat(PlayerConstants.TIMER_PREF_KEY, _gameTimer);
        PlayerPrefs.Save();

        // Getting playlist data
        string playlistString = PlayerPrefs.GetString(PlayerConstants.PLAYLIST_PREF_KEY, "");
        string[] playlist = playlistString.Split(new[] { "###" }, StringSplitOptions.None);
        int playlistIndex = PlayerPrefs.GetInt(PlayerConstants.PLAYLIST_TRACKER_PREF_KEY, 0);

        // Check if levels are over, send to boss fight
        if (playlistIndex == playlist.Length - 1)
        {
            SceneManager.LoadScene(SceneIndexes.BossFightSceneIndex);
        }
        else
        {
            // else increment and reload scene
            PlayerPrefs.SetInt(PlayerConstants.PLAYLIST_TRACKER_PREF_KEY, playlistIndex + 1);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

    }

    public void StartSubmission()
    {
        // #if UNITY_EDITOR == true
        // #endif

#if UNITY_WEBGL == true && UNITY_EDITOR == false
            SubmitScore(_gameTimer);
#endif

        Debug.Log("Submitted time: " + _gameTimer);
    }


}
