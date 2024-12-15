using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerEntrance : MonoBehaviour
{
    private Animator _animator;
    [SerializeField] private InputActionAsset inputActions;
    private InputActionMap _gameplayActions;
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private CharacterController _characterController;
    void Start()
    {
        if (_playerTransform == null) return;

        // Disable player controls
        _gameplayActions = inputActions.FindActionMap("PlayerControls");
        _gameplayActions.Disable();

        // set player as parent of this
        _playerTransform.SetParent(transform);

        // Set parent and position
        _playerTransform.SetParent(transform);
        _playerTransform.localPosition = Vector3.zero;

        // Go to hovering state
        _animator = _playerTransform.GetComponent<Animator>();
        _animator.Play("Hovering");

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
        _playerTransform.GetComponent<PlayerStateMachine>().GravityMultiplier = 1;
        _playerTransform.SetParent(null);

        Destroy(gameObject);
    }

    void Update()
    {
        _playerTransform.transform.position = gameObject.transform.position;
    }
}
