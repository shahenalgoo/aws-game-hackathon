using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Animations.Rigging;
public class PlayerStateMachine : MonoBehaviour
{
    [Header("User Input variables")]
    [SerializeField] private Vector3 _movementInput;
    [SerializeField] private Vector3 _rightStickInput;
    [SerializeField] private Vector3 _cursorPosition;
    [SerializeField] private Vector3 _pointToLook;
    [SerializeField] private InputDevice _currentInputDevice;
    [SerializeField] private float _leftStickDeadzone;
    public Vector3 MovementInput { get { return _movementInput; } set { _movementInput = value; } }


    [Header("Movement variables")]
    [SerializeField] private Vector3 _moveDirection;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private bool _isRunning = false;
    [SerializeField] private float _lookYOffset = 0f;
    private int _turnSpeed = 360;
    private int _turnSpeedMagnitude = 10;
    public Vector3 MoveDirection { get { return _moveDirection; } set { _moveDirection = value; } }
    public float MoveSpeed { get { return _moveSpeed; } }
    public bool IsRunning { get { return _isRunning; } set { _isRunning = value; } }


    [Header("Dash variables")]
    [SerializeField] private bool _isDashing = true;
    [SerializeField] private float _dashDistance;
    [SerializeField] private float _dashSpeed;
    [SerializeField] private float _dashTime;
    [SerializeField] private bool _canDash = true;
    [SerializeField] private float _dashCooldown;
    [SerializeField] private float _dashCooldownCounter;
    public bool IsDashing { get { return _isDashing; } set { _isDashing = value; } }
    public float DashDistance { get { return _dashDistance; } }
    public float DashSpeed { get { return _dashSpeed; } }
    public float DashTime { get { return _dashTime; } }
    public float DashCooldown { get { return _dashCooldown; } }
    public bool CanDash { get { return _canDash; } }


    [Header("Fight Mode variables")]
    [SerializeField] private bool _fightMode;
    [SerializeField] private float _fightModeCountdown;
    [SerializeField] private float _currentFightModeCountdown;
    [SerializeField] private bool _canCountdownFightMode;
    [SerializeField] private float _velocityX;
    [SerializeField] private float _velocityZ;
    private Action _lookAtAim;
    public bool IsFightMode { get { return _fightMode; } }
    public float VelocityX { get { return _velocityX; } set { _velocityX = value; } }
    public float VelocityZ { get { return _velocityZ; } set { _velocityZ = value; } }
    public Action AimGun { get { return _lookAtAim; } set { _lookAtAim = value; } }


    [Header("Weapon variables")]
    [SerializeField] private GunManager _gun;
    [SerializeField] private bool _isShootingAllowed;
    [SerializeField] private bool _isShooting;
    public GunManager Gun { get { return _gun; } set { _gun = value; } }
    public bool IsShootingAllowed { get { return _isShootingAllowed; } set { _isShootingAllowed = value; } }
    public bool IsShooting { get { return _isShooting; } set { _isShooting = value; } }


    [Header("Reload Variables")]
    [SerializeField] private bool _isReloading = false;
    [SerializeField] private bool _reloadAttempt = false;
    public bool IsReloading { get { return _isReloading; } set { _isReloading = value; } }
    public bool ReloadAttempt { get { return _reloadAttempt; } set { _reloadAttempt = value; } }


    [Header("Animation Rigging variables")]
    [SerializeField] private Rig _rig;
    [SerializeField] private GameObject _weapon;
    [SerializeField] private TwoBoneIKConstraint _leftHand;
    public TwoBoneIKConstraint LeftHand { get { return _leftHand; } set { _leftHand = value; } }


    [Header("Component/Script references")]
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private Animator _characterAnimator;
    private Transform _playerModel;
    private Camera _mainCamera;
    public CharacterController CharController { get { return _characterController; } set { _characterController = value; } }
    public Animator CharacterAnimator { get { return _characterAnimator; } set { _characterAnimator = value; } }


    [Header("State Variables")]
    PlayerBaseState _currentState;
    PlayerStateFactory _states;
    public PlayerBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }


    public void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _characterAnimator = GetComponent<Animator>();
        _playerModel = GetComponent<Transform>();
        _mainCamera = Camera.main;

        _currentFightModeCountdown = _fightModeCountdown;
        _fightMode = false;

        // Setup states
        _states = new PlayerStateFactory(this);
        _currentState = _states.Idle();
        _currentState.EnterState();

        // Animation rigging
        ToggleRigAndWeapon(false);
    }


    /* GET INPUT FROM KEYBOARD OR CONTROLLER  - FOR MOVEMENT*/
    public void GetMovementInput(InputAction.CallbackContext ctx)
    {
        Vector2 stickValue = ctx.ReadValue<Vector2>();
        _movementInput = stickValue.magnitude < _leftStickDeadzone ? Vector3.zero : new Vector3(stickValue.x, 0, stickValue.y);
    }


    /* GET INPUT FROM MOUSE CURSOR - FOR AIMING*/
    public void GetCursorPosition(InputAction.CallbackContext ctx)
    {
        Vector2 cursorValue = ctx.ReadValue<Vector2>();
        _cursorPosition = new Vector3(cursorValue.x, cursorValue.y, 0);

        if (_currentInputDevice != InputDevice.MKB) _currentInputDevice = InputDevice.MKB;
    }


    /* GET INPUT FROM RIGHT STICK [CONTROLLER] - FOR AIMING*/
    public void GetRightStickInput(InputAction.CallbackContext ctx)
    {
        Vector2 stickValue = ctx.ReadValue<Vector2>();
        _rightStickInput = new Vector3(stickValue.x, 0, stickValue.y);

        if (_currentInputDevice != InputDevice.Controller) _currentInputDevice = InputDevice.Controller;
    }


    /* GET INPUT FOR DASH */
    public void GetDashInput(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && !_isDashing && _canDash)
        {
            _canDash = false;
            _currentState.SwitchState(_states.Dash());
        }
    }


    /* GET INPUT FOR RELOAD */
    public void GetReloadInput(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && !_reloadAttempt && Gun.CanReload()) _reloadAttempt = true;
    }


    /* GET INPUT FOR SHOOTING AUTOMATIC*/
    public void GetAutomaticShootInput(InputAction.CallbackContext ctx)
    {
        if (Gun.FireStyle != FireType.Automatic) return;
        ActivateFightMode();
        _isShooting = ctx.ReadValue<float>() > 0.1f;
        CheckFightMode(_isShooting);
    }

    /*SINGLE FIRE*/
    public void GetSingleShotInput(InputAction.CallbackContext ctx)
    {
        if (Gun.FireStyle != FireType.Single) return;
        if (ctx.performed)
        {
            _isShooting = true;
            ActivateFightMode();
        }
        else
        {
            _isShooting = false;
        }

        CheckFightMode(_isShooting);
    }


    public void Update()
    {
        _currentState.UpdateStates();
        FightModeCountdown();
        CooldownDash();
    }

    public void LookAtAim()
    {
        // Look at cursor or controller right stick direction in fight mode
        if (_currentInputDevice == InputDevice.MKB)
        {
            _pointToLook = Helpers.TargetMousePosition(_mainCamera, _cursorPosition, _lookYOffset);
            // // Look at point
            Vector3 aimVector = new Vector3(_pointToLook.x, transform.position.y, _pointToLook.z);
            transform.LookAt(aimVector);
        }
        else
        {
            // WARNING: Currently clashing with LookAt for motion - need to add fightMode
            if (_rightStickInput != Vector3.zero)
            {
                Quaternion rotationR = Quaternion.LookRotation(_rightStickInput.ToIso(), Vector3.up);
                _playerModel.rotation = Quaternion.RotateTowards(_playerModel.rotation, rotationR, 2 * _turnSpeed * _turnSpeedMagnitude * Time.deltaTime);
            }
        }
    }

    public void CooldownDash()
    {
        if (_isDashing) return;
        if (_canDash) return;

        _dashCooldownCounter += Time.deltaTime;

        if (_dashCooldownCounter >= _dashCooldown)
        {
            _canDash = true;
            _dashCooldownCounter = 0;
        }
    }

    public void ActivateFightMode()
    {
        if (_fightMode) return;

        _fightMode = true;
        _characterAnimator.SetBool("isFightMode", true);
        _lookAtAim += LookAtAim;

        ToggleRigAndWeapon(_fightMode);
    }

    public void CheckFightMode(bool isAttacking)
    {
        if (isAttacking)
        {
            if (_canCountdownFightMode) _canCountdownFightMode = false;
        }
        else
        {
            // If stop attacking, start countdown to disable fightMode
            _canCountdownFightMode = true;
            _currentFightModeCountdown = _fightModeCountdown;
        }
    }

    public void FightModeCountdown()
    {
        if (!_canCountdownFightMode) return;

        _currentFightModeCountdown -= Time.deltaTime;

        if (_currentFightModeCountdown <= 0)
        {
            _fightMode = false;
            _characterAnimator.SetBool("isFightMode", false);
            _canCountdownFightMode = false;
            _lookAtAim -= LookAtAim;

            ToggleRigAndWeapon(_fightMode);
        }
    }

    public void ToggleRigAndWeapon(bool value)
    {
        _rig.weight = value ? 1 : 0;
        _weapon.GetComponent<MeshRenderer>().enabled = value;
    }

    public void AllowShootDelayed() => Invoke("AllowShoot", 0.1f);
    public void AllowShoot() => _isShootingAllowed = true;

    // Feed collision info to current state
    private void OnControllerColliderHit(ControllerColliderHit hit) => _currentState.CollisionHandler?.Invoke(hit);

}
