using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;

public class GameManager : MonoBehaviour
{
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


    // For survival mode
    private bool _isSurvivalMode = false;
    public bool IsSurvivalMode { get => _isSurvivalMode; }
    private int _roundReached;
    public int RoundReached { get => _roundReached; }


    /** EXTERNAL COMM **/
    [DllImport("__Internal")]
    private static extern void SubmitTime(float time);
    [DllImport("__Internal")]
    private static extern void SubmitSurvivalData(float time, int round);
    [DllImport("__Internal")]
    private static extern void RequestSurvivalLevel();
    [DllImport("__Internal")]
    private static extern void PlayVoiceline(string type);
    private void Awake()
    {
        SingletonCheck();

        // Clear possible previous states
        Time.timeScale = 1f;
        AudioManager.Instance?.PauseAudio(false);
    }

    public void Start()
    {
        _gameTimer = PlayerPrefs.GetFloat(PlayerConstants.TIMER_PREF_KEY, 0f);

        _isSurvivalMode = Helpers.IsSurvivalMode();
        if (_isSurvivalMode)
        {
            // Increment round and save, update on hud
            _roundReached = PlayerPrefs.GetInt(PlayerConstants.ROUNDS_SURVIVED_PREF_KEY, 0) + 1;
            Helpers.RecordRoundReached(_roundReached);
            HUDManager.Instance?.ShowRounds(_roundReached);

        }

#if UNITY_WEBGL == true && UNITY_EDITOR == false
        if (SceneManager.GetActiveScene().buildIndex == SceneIndexes.GameSceneIndex)
        {
            PlayVoiceline("spawn");
        }
        else if (SceneManager.GetActiveScene().buildIndex == SceneIndexes.BossFightSceneIndex)
        {
            PlayVoiceline("beforeBossFight");
        }
#endif
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

    public bool HasCollectedAll() => _lootCollected == _totalTargets;

    public void StopTimeCount()
    {
        _canCountTime = false;
    }

    public void GoToNextLevel()
    {
        Helpers.RecordTime(_gameTimer);

        if (_isSurvivalMode)
        {
            // request new level
#if UNITY_WEBGL == true && UNITY_EDITOR == false
            RequestSurvivalLevel();
#endif

#if UNITY_WEBGL == false || UNITY_EDITOR == true
            StartNewRoundTest();
#endif
            return;
        }


        // If we are in a boss fight, trigger submission directly
        if (SceneManager.GetActiveScene().buildIndex == SceneIndexes.BossFightSceneIndex)
        {
            Debug.Log("Boss defeated");
            StartSubmission();
            return;
        }

        // Getting playlist data
        string playlistString = PlayerPrefs.GetString(PlayerConstants.PLAYLIST_PREF_KEY, "");
        string[] playlist = playlistString.Split(new[] { "###" }, StringSplitOptions.None);
        int playlistIndex = PlayerPrefs.GetInt(PlayerConstants.PLAYLIST_TRACKER_PREF_KEY, 0);

        // Check if levels are over, send to boss fight if any
        if (playlistIndex == playlist.Length - 1)
        {
            bool hasBossFight = PlayerPrefs.GetInt(PlayerConstants.HAS_BOSS_PREF_KEY, 0) == 1;

            if (hasBossFight)
            {

                SceneManager.LoadScene(SceneIndexes.BossFightSceneIndex);
            }
            else
            {
                Debug.Log("No boss fight in this mode, show after game screen");
                StartSubmission();
            }
        }
        else
        {
            // else increment and reload scene
            PlayerPrefs.SetInt(PlayerConstants.PLAYLIST_TRACKER_PREF_KEY, playlistIndex + 1);
            PlayerPrefs.Save();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

    }

    public void StartNewRoundTest()
    {
        StartNewRound("{\"grid\":[[0,0,0,2,1,2,1,2,0,0,2,4,0,0,0,0,3,2,0,0],[0,2,7,5,0,7,0,6,0,0,1,3,0,2,1,4,1,0,0,0],[1,1,2,1,0,2,0,1,2,0,5,1,2,7,5,0,6,0,0,0],[4,8,0,3,0,1,0,0,1,0,0,0,0,2,1,0,2,0,0,0],[1,6,0,1,2,5,2,0,4,0,0,0,0,7,3,0,1,2,0,0],[0,1,0,0,0,1,6,0,1,2,0,0,2,1,4,0,5,0,0,0],[0,3,2,0,0,0,1,0,0,7,0,2,6,0,1,0,2,0,0,0],[0,1,5,4,2,0,3,1,0,2,0,1,2,0,5,0,1,2,0,0],[0,0,0,0,1,2,0,2,0,1,2,0,0,0,2,0,0,1,2,0]]}");
    }

    public void StartNewRound(string gridData)
    {
        // Set up array
        string[] playlist = new string[1];
        playlist[0] = gridData;

        // Set playlist
        Helpers.SetPlaylist(playlist);

        // Reload scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }

    public void StartSubmission()
    {
        Time.timeScale = 0;
        AudioManager.Instance.PauseAudio(true);


#if UNITY_WEBGL == true && UNITY_EDITOR == false
            SubmitTime(_gameTimer);
#endif

        Debug.Log("Submitted time: " + _gameTimer);
    }

    public void StartSubmissionSurvival()
    {
        // already being paused in death menu request
#if UNITY_WEBGL == true && UNITY_EDITOR == false
            SubmitSurvivalData(_gameTimer, _roundReached);
#endif

        Debug.Log("Submitted time and round: " + _gameTimer + ", " + _roundReached);
    }


}
