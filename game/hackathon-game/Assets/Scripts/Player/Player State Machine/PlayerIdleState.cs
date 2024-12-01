using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    public PlayerIdleState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {
        IsRootState = true;
        InitializeSubState();
    }


    public override void EnterState()
    {
        Ctx.IsRunning = false;
        Ctx.CharacterAnimator.SetBool("isRunning", false);
    }

    public override void UpdateState()
    {
        //should follow cursor if fight mode
        Ctx.AimGun?.Invoke();

        // Apply Gravity
        if (!Ctx.CharController.isGrounded)
        {
            Ctx.CharController.Move(Physics.gravity * Time.deltaTime);
        }

        CheckSwitchStates();

    }

    public override void CheckSwitchStates()
    {
        //if movement input is not 0, we have to switch to run
        if (Ctx.MovementInput != Vector3.zero) SwitchState(Factory.Run());

        // Add a reload sub state 
        if (Ctx.ReloadAttempt && !Ctx.IsReloading)
        {
            SetSubState(Factory.Reload());
            CurrentSubState.EnterState();
        }

        if (Ctx.IsStunned)
        {
            SwitchState(Factory.Stunned());
        }
    }

    public override void ExitState() { }
    public override void InitializeSubState()
    {
        if (Ctx.IsReloading) SetSubState(Factory.Reload());
    }

    public override void CollisionEventHandler(ControllerColliderHit hit) { }

}
