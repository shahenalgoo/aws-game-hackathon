using UnityEngine;

public class PlayerDashState : PlayerBaseState
{
    public PlayerDashState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {
        IsRootState = true;
        InitializeSubState();
    }
    private float _dashDistance;
    private bool _isDashOver;
    private Vector3 _dash;

    int layerMask = 1 << 6;
    private Vector3 _startingPoint;

    public override void EnterState()
    {

        // Set control variables
        _dashDistance = Ctx.DashDistance;
        _isDashOver = false;

        // Determine valid destination
        // CalculatePossibleStopPoint();

        // Turn body in direction of motion 
        Ctx.transform.LookAt(Ctx.MoveDirection);

        // Start animation
        Ctx.CharacterAnimator.Play("Dash");
        Ctx.CharacterAnimator.SetBool("isDashing", true);
        Ctx.IsDashing = true;

        // Disallow shooting
        Ctx.IsShootingAllowed = false;
        if (Ctx.IsShooting && Ctx.Gun != null) Ctx.Gun.InterruptMuzzleFlash();


        // Disable rig
        Ctx.ToggleRigAndWeapon(false);

        _startingPoint = Ctx.transform.position;

    }

    public void CalculatePossibleStopPoint()
    {
        // We shoot a ray out to the player in the forward vector, with the length of dash distance
        RaycastHit hit;

        if (Physics.Raycast(Ctx.transform.position, Ctx.transform.TransformDirection(Vector3.forward), out hit, Ctx.DashDistance + 0.1f, layerMask))
        {
            Debug.DrawRay(Ctx.transform.position, Ctx.transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);

            // if object is too far away, we will stop at it, else we check if we can go through it
            if (hit.distance <= Ctx.DashDistance * 0.7f)
            {
                // let's assume we are going through the object, so let's disable the player colliders
                Ctx.CharController.enabled = false;

                bool spotFound = false;

                // let's check at usual stop point
                // if stop point is free, we go there, else we check the next closest
                for (int i = 0; i < Ctx.DashDistance * 2; i++)
                {
                    float distanceToCheck = Ctx.DashDistance - ((float)i / 2f);
                    Vector3 pointToCheck = Ctx.transform.position + (Ctx.transform.forward * distanceToCheck);
                    Collider[] hitColliders = Physics.OverlapSphere(pointToCheck, Ctx.CharController.radius, layerMask);
                    if (hitColliders.Length == 0)
                    {
                        // this means no obstacles were found at this location, adjust dash distance so we stop there
                        _dashDistance = distanceToCheck;
                        spotFound = true;
                        break;
                    }
                }

                // if no valid destination is found, we reactivate player collider right away
                if (!spotFound) Ctx.CharController.enabled = true;
            }
        }

    }

    public override void UpdateState()
    {

        if (Vector3.Distance(_startingPoint, Ctx.transform.position) <= _dashDistance)
        {
            // Apply forward motion
            _dash = Ctx.transform.forward * Ctx.DashSpeed * Time.deltaTime;

            if (Ctx.CharController.enabled)
            {
                Ctx.CharController.Move(_dash);
            }
            else
            {
                // we still need to move the player even if the character controller is disabled
                Ctx.transform.position += _dash;
            }


        }
        else if (!_isDashOver)
        {
            Ctx.IsDashing = false;
            CheckSwitchStates();
            _isDashOver = true;
        }

    }

    public override void ExitState()
    {
        Ctx.CharacterAnimator.SetBool("isDashing", false);

        // Toggle weapon and rig if fight mode
        Ctx.ToggleRigAndWeapon(Ctx.IsFightMode);

        // Allow shooting after a few ms so that player doesn't shoot in dash direction
        Ctx.AllowShootDelayed();

        // Reactivate Character Controller
        if (!Ctx.CharController.enabled) Ctx.CharController.enabled = true;


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

    public override void CollisionEventHandler(ControllerColliderHit hit)
    {
        if (hit.gameObject.layer == LayerMask.NameToLayer("InvisibleWall"))
        {
            Ctx.IsDashing = false;
            CheckSwitchStates();
        }
    }



}