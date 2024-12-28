using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Runtime.InteropServices;


public class UIManager : Singleton<UIManager>
{
    [Header("General")]
    public GameObject _screenFader;

    [Header("Pause Panel")]

    public GameObject _pausePanel;
    private bool _isPaused = false;
    public bool IsPaused { get { return _isPaused; } }
    [SerializeField] private InputActionAsset inputActions;
    private InputActionMap _gameplayActions;
    public InputActionMap GameplayActions { get { return _gameplayActions; } }
    private Vector3 _lastKnownCursorPosition;

    [Header("Death Panel")]
    public GameObject _deathPanel;
    private bool _playerDied;
    [SerializeField] private float _enableDeathPanelDelay = 2f;

    [Header("Audio Toggles")]
    [SerializeField] private Toggle _sfxToggle;
    [SerializeField] private Toggle _musicToggle;

    [SerializeField] private PlayerStateMachine _psm;
    [SerializeField] private Minimap _map;



    // For React
    [DllImport("__Internal")]
    private static extern void ActivatePauseMenu(int sfxMute, int musicMute);
    [DllImport("__Internal")]
    private static extern void ActivateDeathMenu();


    public void Start()
    {
        _gameplayActions = inputActions.FindActionMap("Gameplay");
    }

    public void TogglePauseGame()
    {
        // cannot pause if dead
        if (_playerDied) return;

        _isPaused = !_isPaused;

#if UNITY_WEBGL == true && UNITY_EDITOR == false
        if (_isPaused)
        {
            int sfxMute = PlayerPrefs.GetInt(PlayerConstants.SFX_MUTE_PREF_KEY, 0);
            int musicMute = PlayerPrefs.GetInt(PlayerConstants.MUSIC_MUTE_PREF_KEY, 0);
            ActivatePauseMenu(sfxMute, musicMute);
            Debug.Log("Pause menu requested");
        }
#endif

#if UNITY_EDITOR == true
        _pausePanel.SetActive(_isPaused);
#endif

        if (_isPaused)
        {
            // disable map if active
            if (_map != null && _map.MinimapContainer.gameObject.activeSelf) _map.MinimapContainer.gameObject.SetActive(false);
            // deactivate player controls
            if (_psm != null) _lastKnownCursorPosition = _psm.CursorPosition;
            _gameplayActions.Disable();
            Time.timeScale = 0;
            if (_psm != null) _psm.CursorPosition = _lastKnownCursorPosition;
            AudioManager.Instance.PauseAudio(true);

            LoadToggleStates();

        }
        else
        {
            // reactiveate player controls
            _gameplayActions.Enable();
            Time.timeScale = 1;
            AudioManager.Instance.PauseAudio(false);
        }

    }

    public void GetPauseInput(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) TogglePauseGame();
    }

    public void RestartLevel()
    {
        Helpers.ResetTrackingVariables();
        if (Time.timeScale == 0) Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitToMainMenu()
    {
        SceneManager.LoadScene(SceneIndexes.MainMenuSceneIndex);
    }

    public IEnumerator DeathPanelSetup()
    {
        _playerDied = true;
        _gameplayActions.Disable();

        yield return new WaitForSeconds(_enableDeathPanelDelay);

#if UNITY_WEBGL == true && UNITY_EDITOR == false
    ActivateDeathMenu();
    Debug.Log("Death menu requested");
#endif

#if UNITY_EDITOR == true
        EnableDeathPanel();
#endif

    }

    private void EnableDeathPanel()
    {
        Time.timeScale = 0;
        AudioManager.Instance.PauseAudio(true);
        _deathPanel.SetActive(true);
    }

    public void LoadToggleStates()
    {
        _sfxToggle.isOn = PlayerPrefs.GetInt(PlayerConstants.SFX_MUTE_PREF_KEY, 0) == 0;
        _musicToggle.isOn = PlayerPrefs.GetInt(PlayerConstants.MUSIC_MUTE_PREF_KEY, 0) == 0;
    }

    public void ToggleSFX(Toggle toggle)
    {
        AudioManager.Instance.SetSFXMute(!toggle.isOn);
    }

    public void ToggleMusic(Toggle toggle)
    {
        AudioManager.Instance.SetMusicMute(!toggle.isOn);
        if (toggle.isOn) AudioManager.Instance.SetMusic(true);
    }
    public void ToggleSFXFromReact()
    {
        AudioManager.Instance.SetSFXMute(!AudioManager.Instance.SfxMuted);
    }
    public void ToggleMusicFromReact()
    {
        AudioManager.Instance.SetSFXMute(!AudioManager.Instance.MusicMuted);
    }

    private void OnDestroy()
    {
        AudioManager.Instance.StopAllSFXEvents();
    }

}
