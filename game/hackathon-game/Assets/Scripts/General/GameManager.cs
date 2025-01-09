using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;
using System.Collections;

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


    /** EXTERNAL COMM **/
    [DllImport("__Internal")]
    private static extern void SubmitTime(float time);
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
        StartCoroutine(RequestVoiceline());
    }

    private IEnumerator RequestVoiceline()
    {
        yield return new WaitForSeconds(2f);
        // Play voiceline
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

    public void StartSubmission()
    {
        Time.timeScale = 0;
        AudioManager.Instance.PauseAudio(true);


#if UNITY_WEBGL == true && UNITY_EDITOR == false
            SubmitTime(_gameTimer);
#endif

        Debug.Log("Submitted time: " + _gameTimer);
    }


}
