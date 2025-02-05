using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerEntrance : MonoBehaviour
{
    private Animator _animator;
    [SerializeField] private InputActionAsset _inputActions;
    [SerializeField] private InputActionMap _gameplayActions;
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private string _playerObjective = "Collect all stars to extract";

    private PlayerStateMachine _psm;
    void Awake()
    {
        if (_inputActions == null) Debug.Log("input actions not found");
        _gameplayActions = _inputActions.FindActionMap("Gameplay");
        if (_gameplayActions == null) Debug.Log("action map not found");
        // Disable player controls
        _gameplayActions.Disable();
        GameManager.Instance?.StopTimeCount();
    }
    void Start()
    {
        if (GameManager.Instance != null && !GameManager.Instance.UsePlayerEntranceAnimation)
        {
            _gameplayActions.Enable();
            GameManager.Instance.CanCountTime = true;
            Destroy(gameObject);
            return;
        }

        if (_playerTransform == null) return;

        // Turn on hover tornado
        _psm = _playerTransform.GetComponent<PlayerStateMachine>();
        _psm.ToggleHoverTornado(true);

        // Go to hovering state
        _animator = _playerTransform.GetComponent<Animator>();
        _animator.Play("Hovering");

        // Play SFX
        AudioManager.Instance.PlaySfx(AudioManager.Instance._playerEntranceSfx);

        // disable gravity
        _playerTransform.GetComponent<PlayerStateMachine>().GravityMultiplier = 0;

        // play animation
        GetComponent<Animator>().Play("Entrance");

        StartCoroutine(AfterEntranceCompleted());
    }

    private IEnumerator AfterEntranceCompleted()
    {
        yield return new WaitForSeconds(1.5f);
        _animator.Play("Idle");
        _gameplayActions.Enable();

        // Turn off hover tornado
        _psm.ToggleHoverTornado(false);

        _psm.GravityMultiplier = 1;
        _playerTransform.SetParent(null);
        LevelBuilder.Instance?.InitializeMinimap();

        // Give objective
        HUDManager._noticeUpdater?.Invoke(_playerObjective, 3f);

        // Count time
        if (TutorialManager.Instance == null) GameManager.Instance.CanCountTime = true;

        Destroy(gameObject);
    }

    void LateUpdate()
    {
        if (GameManager.Instance != null && !GameManager.Instance.UsePlayerEntranceAnimation) return;
        _playerTransform.transform.position = gameObject.transform.position;
    }
}
