using UnityEngine;

public class PlayerRunState : PlayerBaseState
{
    public PlayerRunState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {
        IsRootState = true;
        InitializeSubState();
    }



    public override void EnterState()
    {
        Ctx.IsRunning = true;
        Ctx.CharacterAnimator.SetBool("isRunning", true);
    }

    public override void UpdateState()
    {
        // Apply motion
        Vector3 moveVelocity = Ctx.MovementInput.normalized * Ctx.MoveSpeed;
        Ctx.CharController.Move(moveVelocity.ToIso() * Time.deltaTime);

        // Return to ground, if ever we go up
        if (!Ctx.CharController.isGrounded)
        {
            Ctx.CharController.Move(Physics.gravity * Ctx.GravityMultiplier * Time.deltaTime);
        }

        // should check fight mode in order to follow cursor or not
        Ctx.AimGun?.Invoke();

        // adjust body to always face the direction of motion depending on fight mode

        if (!Ctx.IsFightMode)
        {
            Ctx.transform.LookAt(Ctx.MoveDirection);
        }
        else
        {
            // In fight mode, blend feet motion accordingly if aiming at cursor
            Ctx.VelocityZ = Vector3.Dot(Ctx.MovementInput.normalized.ToIso(), Ctx.transform.forward);
            Ctx.VelocityX = Vector3.Dot(Ctx.MovementInput.normalized.ToIso(), Ctx.transform.right);
            Ctx.CharacterAnimator.SetFloat("VelocityZ", Ctx.VelocityZ, 0.1f, Time.deltaTime);
            Ctx.CharacterAnimator.SetFloat("VelocityX", Ctx.VelocityX, 0.1f, Time.deltaTime);
        }

        CheckSwitchStates();


    }

    public override void ExitState()
    {
        Ctx.CharacterAnimator.SetFloat("VelocityZ", 0);
        Ctx.CharacterAnimator.SetFloat("VelocityX", 0);
    }

    public override void CheckSwitchStates()
    {
        // Check stunned state
        if (Ctx.IsStunned)
        {
            SwitchState(Factory.Stunned());
        }

        //if movement input is 0, we have to switch to idle
        if (Ctx.MovementInput == Vector3.zero) SwitchState(Factory.Idle());

        // Add a reload sub state 
        if (Ctx.ReloadAttempt && !Ctx.IsReloading)
        {
            SetSubState(Factory.Reload());
            CurrentSubState.EnterState();
        }

    }

    public override void InitializeSubState()
    {

        if (Ctx.IsReloading) SetSubState(Factory.Reload());
    }

    public override void CollisionEventHandler(ControllerColliderHit hit) { }
    public override void OnTriggerEventHandler(Collider other) { }


}
