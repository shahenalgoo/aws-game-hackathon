
using UnityEngine;

public class PlayerReloadState : PlayerBaseState
{
    public PlayerReloadState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory) { }

    public override void EnterState()
    {
        if (!Ctx.IsFightMode) Ctx.ActivateFightMode();
        Ctx.CheckFightMode(true);
        Ctx.IsReloading = true;
        Ctx.CharacterAnimator.SetBool("isReloading", true);
        Ctx.LeftHand.weight = 0;

        Ctx.IsShootingAllowed = false;

        // Activate reload bar
        ReloadBar._activateReloadSlider?.Invoke();

        // Play sfx
        AudioManager.Instance.PlaySfx(AudioManager.Instance._magOutSfx);
    }

    public override void UpdateState()
    {
        if (Ctx.CharacterAnimator.GetCurrentAnimatorStateInfo(1).normalizedTime > 1f && Ctx.CharacterAnimator.GetCurrentAnimatorStateInfo(1).IsName("Reload") && Ctx.IsReloading)
        {
            // Reward with ammo
            Ctx.Gun.ReloadMag();

            // Reloaded sfx
            AudioManager.Instance.PlaySfx(AudioManager.Instance._reloadedSfx);


            // exit state
            ExitState();
        }
    }

    public override void ExitState()
    {

        // Check fight mode
        Ctx.CheckFightMode(false);

        Ctx.CharacterAnimator.SetBool("isReloading", false);
        Ctx.IsReloading = false;
        Ctx.ReloadAttempt = false;
        Ctx.LeftHand.weight = 1;
        CurrentSuperState.CurrentSubState = null;

        Ctx.AllowShoot();
    }

    public override void CheckSwitchStates() { }

    public override void InitializeSubState() { }

    public override void CollisionEventHandler(ControllerColliderHit hit) { }

    public override void OnTriggerEventHandler(Collider other) { }

}