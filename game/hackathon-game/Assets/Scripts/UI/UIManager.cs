using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : Singleton<UIManager>
{
    public GameObject _player;
    public GameObject _pausePanel;

    private bool _isPaused = false;
    public bool IsPaused { get { return _isPaused; } }

    public void TogglePauseGame()
    {
        _isPaused = !_isPaused;
        _pausePanel.SetActive(_isPaused);
        Time.timeScale = _isPaused ? 0 : 1;

    }

    public void GetPauseInput(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) TogglePauseGame();
    }
}
