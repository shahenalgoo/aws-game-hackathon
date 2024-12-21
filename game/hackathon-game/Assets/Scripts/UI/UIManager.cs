using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UIManager : Singleton<UIManager>
{
    [Header("Pause Panel")]

    public GameObject _pausePanel;
    private bool _isPaused = false;
    public bool IsPaused { get { return _isPaused; } }
    [SerializeField] private InputActionAsset inputActions;
    private InputActionMap _gameplayActions;
    private Vector3 _lastKnownCursorPosition;

    [Header("Death Panel")]
    public GameObject _deathPanel;
    private bool _playerDied;
    [SerializeField] private float _enableDeathPanelDelay = 2f;



    [SerializeField] private PlayerStateMachine _psm;
    [SerializeField] private Minimap _map;


    public void Start()
    {
        _gameplayActions = inputActions.FindActionMap("Gameplay");
    }

    public void TogglePauseGame()
    {
        // cannot pause if dead
        if (_playerDied) return;

        _isPaused = !_isPaused;
        _pausePanel.SetActive(_isPaused);

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

        EnableDeathPanel();
    }

    private void EnableDeathPanel()
    {
        Time.timeScale = 0;
        AudioManager.Instance.PauseAudio(true);
        _deathPanel.SetActive(true);
    }

}
