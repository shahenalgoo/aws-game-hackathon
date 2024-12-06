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

    int layerMask = 1 << LayerMask.NameToLayer("Target");
    private Vector3 _startingPoint;

    private float _stationaryDashDuration = 0.25f; // Adjust this value as needed
    private bool _stationaryDashTimeElapsed = false;
    private float _stationaryDashTimer = 0f;

    private bool _stationaryDash = false;
    public float _normalDashTime;
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

        // Setup trail
        ToggleTrails(true);

        // Toggle lightning
        Ctx._dashLightning.Play();

    }

    public void ActivateStationaryDash()
    {
        _stationaryDashDuration -= _normalDashTime;
        _stationaryDash = true;
        Ctx._dashLightning.Clear();
        Ctx._dashLightning.Stop();
        Ctx._dashLightningOnHit.Play();
    }

    public void ToggleTrails(bool value)
    {
        for (int i = 0; i < Ctx._dashTrails.Length; i++)
        {
            Ctx._dashTrails[i].emitting = value;
        }
    }
    private void StartStationaryDashTimer()
    {
        _stationaryDashTimer += Time.deltaTime;
        if (_stationaryDashTimer >= _stationaryDashDuration)
        {
            _stationaryDashTimeElapsed = true;
        }
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
        if (_stationaryDash)
        {

            // Stay in place but continue the dash animation/effects
            if (!_isDashOver)
            {
                // You might want to add a timer here to control how long the stationary dash lasts
                StartStationaryDashTimer();
            }
        }
        else if (Vector3.Distance(_startingPoint, Ctx.transform.position) <= _dashDistance)
        {
            // Normal dash movement
            _dash = Ctx.transform.forward * Ctx.DashSpeed * Time.deltaTime;

            if (Ctx.CharController.enabled)
            {
                Ctx.CharController.Move(_dash);
            }
            else
            {
                Ctx.transform.position += _dash;
            }

            _normalDashTime += Time.deltaTime;
        }

        if (!_isDashOver && ((_stationaryDash && _stationaryDashTimeElapsed) ||
            (!_stationaryDash && Vector3.Distance(_startingPoint, Ctx.transform.position) > _dashDistance)))
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


        ToggleTrails(false);

        // Reset trail positions and colors
        if (_stationaryDash)
        {
            // reset trail pos
            for (int i = 0; i < Ctx._dashTrails.Length; i++)
            {
                Ctx._dashTrails[i].transform.localPosition = Ctx._dashTrailsInitialPos[i];
            }
        }

        // // Disable particle system
        Ctx._dashLightning.Stop();
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

        if (hit.gameObject.layer == LayerMask.NameToLayer("Target"))
        {
            ActivateStationaryDash();
        }


        if (hit.gameObject.layer == LayerMask.NameToLayer("InvisibleWall"))
        {
            // Get the wall's normal
            Vector3 wallNormal = hit.normal;

            // Calculate the angle between player's forward direction and the wall
            float angle = Vector3.Angle(Ctx.transform.forward, -wallNormal);

            // If the angle is greater than a threshold (e.g., 45 degrees)
            // This means the player is approaching the wall at an angle
            if (angle > 42f)
            {
                // Calculate the direction the player should move along the wall
                Vector3 newDirection = Vector3.ProjectOnPlane(Ctx.transform.forward, wallNormal).normalized;

                // Rotate the player to face this new direction
                Ctx.transform.rotation = Quaternion.LookRotation(newDirection);

                // Adjust the remaining dash distance based on how far we've already dashed
                float distanceCovered = Vector3.Distance(_startingPoint, Ctx.transform.position);
                _dashDistance = Mathf.Max(_dashDistance - distanceCovered, 0f);

                // Update the starting point for the new dash direction
                _startingPoint = Ctx.transform.position;

                // Continue with normal dash
                return;
            }

            ActivateStationaryDash();
        }



    }

    public override void OnTriggerEventHandler(Collider other)
    {
        if (other.gameObject.CompareTag("DashBooster"))
        {
            _dashDistance *= 2;

            other.GetComponentInParent<PitfallController>().PlayBooster();
        }
    }



}