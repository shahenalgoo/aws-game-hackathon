using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject _pausePanel;

    private bool _isPaused = false;
    public bool IsPaused { get { return _isPaused; } }
    [SerializeField] private InputActionAsset inputActions;
    private InputActionMap _gameplayActions;
    private Vector3 _lastKnownCursorPosition;

    [SerializeField] private PlayerStateMachine psm;

    [SerializeField] private Minimap map;

    public void Awake()
    {
        _gameplayActions = inputActions.FindActionMap("PlayerControls");
    }

    public void TogglePauseGame()
    {
        _isPaused = !_isPaused;
        _pausePanel.SetActive(_isPaused);

        if (_isPaused)
        {
            // disable map if active
            if (map != null && map.MinimapContainer.gameObject.activeSelf) map.MinimapContainer.gameObject.SetActive(false);
            // deactivate player controls
            if (psm != null) _lastKnownCursorPosition = psm.CursorPosition;
            _gameplayActions.Disable();
            Time.timeScale = 0;
            if (psm != null) psm.CursorPosition = _lastKnownCursorPosition;


        }
        else
        {
            // reactiveate player controls
            _gameplayActions.Enable();
            Time.timeScale = 1;
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
}
