using System;
using UnityEngine;

public abstract class PlayerBaseState
{
    private bool _isRootState = false;
    private PlayerStateMachine _ctx;
    private PlayerStateFactory _factory;
    private PlayerBaseState _currentSubState;
    private PlayerBaseState _currentSuperState;
    private Action<ControllerColliderHit> _collisionHandler;
    private Action<Collider> _triggerHandler;

    public bool IsRootState { set { _isRootState = value; } }
    public PlayerStateMachine Ctx { get { return _ctx; } set { _ctx = value; } }
    public PlayerStateFactory Factory { get { return _factory; } set { _factory = value; } }
    public PlayerBaseState CurrentSubState { get { return _currentSubState; } set { _currentSubState = value; } }
    public PlayerBaseState CurrentSuperState { get { return _currentSuperState; } set { _currentSuperState = value; } }
    public Action<ControllerColliderHit> CollisionHandler { get { return _collisionHandler; } set { _collisionHandler = value; } }
    public Action<Collider> TriggerHandler { get { return _triggerHandler; } set { _triggerHandler = value; } }

    public PlayerBaseState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    {
        _ctx = currentContext;
        _factory = playerStateFactory;
        _collisionHandler += CollisionEventHandler;
        _triggerHandler += OnTriggerEventHandler;
    }

    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
    public abstract void CheckSwitchStates();
    public abstract void InitializeSubState();
    public abstract void CollisionEventHandler(ControllerColliderHit hit);
    public abstract void OnTriggerEventHandler(Collider other);

    public void UpdateStates()
    {
        UpdateState();
        if (_currentSubState != null)
        {
            _currentSubState.UpdateStates();
        }
    }

    public void SwitchState(PlayerBaseState newState)
    {
        // Cannot switch to any other states while dashing
        if (_ctx.IsDashing) return;

        // current state exit
        ExitState();

        // collision handler exit 
        _collisionHandler -= CollisionEventHandler;
        _triggerHandler -= OnTriggerEventHandler;

        //new state enters state
        newState.EnterState();

        // switch current state of context
        _ctx.CurrentState = newState;
    }
    void SetSuperState(PlayerBaseState newSuperState)
    {
        _currentSuperState = newSuperState;
    }
    public void SetSubState(PlayerBaseState newSubState)
    {
        _currentSubState = newSubState;
        newSubState.SetSuperState(this);
    }
}
