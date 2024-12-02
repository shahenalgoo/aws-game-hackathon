using System.Linq;
using UnityEditor.Animations;
using UnityEngine;

public class PlayerStunnedState : PlayerBaseState
{
    public PlayerStunnedState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {
        IsRootState = true;
        InitializeSubState();
    }

    private bool isOver = false;
    private float _stunTime = 0.4f;

    private float _counter;

    public override void EnterState()
    {
        // Start animation
        Ctx.CharacterAnimator.Play("Stunned");

        // Disallow shooting
        Ctx.IsShootingAllowed = false;
        if (Ctx.IsShooting && Ctx.Gun != null) Ctx.Gun.InterruptMuzzleFlash();

        // Disallow dashing
        Ctx.CanDash = false;

        // Disable rig
        Ctx.ToggleRigAndWeapon(false);

        _counter = 0;
    }
    public override void UpdateState()
    {
        // Track move direction in case of dash
        // Ctx.TrackMovement();

        _counter += Time.deltaTime;

        if (_counter >= _stunTime && !isOver)
        {
            isOver = true;

            // exit state
            CheckSwitchStates();
        }

    }

    public override void ExitState()
    {
        // Toggle weapon and rig if fight mode
        Ctx.ToggleRigAndWeapon(Ctx.IsFightMode);

        // Allow dashing
        Ctx.CanDash = true;

        Ctx.AllowShootDelayed();

        Ctx.IsStunned = false;
    }

    public override void CheckSwitchStates()
    {
        SwitchState(Ctx.MovementInput == Vector3.zero ? Factory.Idle() : Factory.Run());

    }

    public override void InitializeSubState()
    {
        // get ref to reload sub state to exit if needed
        if (Ctx.IsReloading)
        {
            SetSubState(Factory.Reload());
            CurrentSubState.ExitState();
            ReloadBar._cancelReloadSlider?.Invoke();
            CurrentSubState = null;
        }
    }

    public override void CollisionEventHandler(ControllerColliderHit hit) { }



}