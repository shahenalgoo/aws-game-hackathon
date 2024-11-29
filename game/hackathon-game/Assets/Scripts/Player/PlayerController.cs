using System.Collections;
using System;
using UnityEngine;
using UnityEngine.InputSystem;


public enum InputDevice { MKB, Controller }
public class PlayerController : MonoBehaviour
{
    [Header("User Input variables")]
    [SerializeField] private Vector3 movementInput;
    [SerializeField] private Vector3 rightStickInput;
    [SerializeField] private Vector3 cursorPosition;
    [SerializeField] private Vector3 pointToLook;
    [SerializeField] private InputDevice currentInputDevice;


    [Header("Movement variables")]
    [SerializeField] private Vector3 moveDirection;
    [SerializeField] private float moveSpeed;
    [SerializeField] private bool isRunning = false;
    public bool IsRunning { get { return isRunning; } }
    private float lookYOffset = 1.7f;
    private int turnSpeed = 360;
    private int turnSpeedMagnitude = 10;
    private Action lookAtAim;


    [Header("Dash variables")]
    [SerializeField] private bool isDashing = true;
    [SerializeField] private float dashDistance;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashTime;

    [Header("Fight Mode variables")]
    [SerializeField] private bool fightMode;
    [SerializeField] private float fightModeCountdown;
    [SerializeField] private float currentFightModeCountdown;
    [SerializeField] private bool canCountdownFightMode;
    private float velocityX;
    private float velocityZ;


    [Header("Component/Script references")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Animator animator;
    private Transform playerModel;
    private Camera mainCamera;
    public void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        playerModel = GetComponent<Transform>();
        mainCamera = Camera.main;
        lookAtAim += LookAtAim;

        currentFightModeCountdown = fightModeCountdown;
    }

    /* GATHERING INPUT FROM KEYBOARD OR CONTROLLER  - FOR MOVEMENT*/
    public void GetMovementInput(InputAction.CallbackContext ctx)
    {

        Vector2 stickValue = ctx.ReadValue<Vector2>();
        movementInput = new Vector3(stickValue.x, 0, stickValue.y);

        if (ctx.performed & !isRunning)
        {
            isRunning = true;
            animator.SetBool("isRunning", true);
        }
        else if (!ctx.performed)
        {
            isRunning = false;
            animator.SetBool("isRunning", false);
            movementInput = Vector3.zero;
        }

    }

    public void GetCursorPosition(InputAction.CallbackContext ctx)
    {
        Vector2 cursorValue = ctx.ReadValue<Vector2>();
        cursorPosition = new Vector3(cursorValue.x, cursorValue.y, 0);

        if (currentInputDevice != InputDevice.MKB) currentInputDevice = InputDevice.MKB;
    }

    public void GetRightStickInput(InputAction.CallbackContext ctx)
    {
        Vector2 stickValue = ctx.ReadValue<Vector2>();
        rightStickInput = new Vector3(stickValue.x, 0, stickValue.y);

        if (currentInputDevice != InputDevice.Controller) currentInputDevice = InputDevice.Controller;

    }

    public void GetShootInput(InputAction.CallbackContext ctx)
    {
        if (!fightMode)
        {
            fightMode = true;
            animator.SetBool("isFightMode", true);
        }
        bool isShooting = ctx.ReadValue<float>() > 0.1f;

        if (isShooting)
        {
            if (canCountdownFightMode) canCountdownFightMode = false;
        }
        else
        {
            // If stop shooting, start countdown to disable fightMode
            canCountdownFightMode = true;
            currentFightModeCountdown = fightModeCountdown;
        }
    }

    public void FightModeCountdown()
    {
        if (!canCountdownFightMode) return;

        currentFightModeCountdown -= Time.deltaTime;

        if (currentFightModeCountdown <= 0)
        {
            fightMode = false;
            animator.SetBool("isFightMode", false);
            canCountdownFightMode = false;
        }

    }

    public Vector3 TargetMousePosition()
    {
        Ray cameraRay = mainCamera.ScreenPointToRay(cursorPosition);
        Plane groundPlane = new Plane(Vector3.up, new Vector3(0, lookYOffset, 0));
        float rayLength;


        if (groundPlane.Raycast(cameraRay, out rayLength))
        {
            Vector3 collidedPoint = cameraRay.GetPoint(rayLength);
            Debug.DrawLine(cameraRay.origin, collidedPoint, Color.blue);
            return collidedPoint;
        }

        return Vector3.zero;

    }

    public void LookAtAim()
    {
        // Look at cursor or controller right stick direction in fight mode
        if (!fightMode) return;

        if (currentInputDevice == InputDevice.MKB)
        {
            pointToLook = TargetMousePosition();
            // // Look at point
            Vector3 aimVector = new Vector3(pointToLook.x, transform.position.y, pointToLook.z);
            transform.LookAt(aimVector);
        }
        else
        {
            // WARNING: Currently clashing with LookAt for motion - need to add fightMode
            if (rightStickInput != Vector3.zero)
            {
                Quaternion rotationR = Quaternion.LookRotation(rightStickInput.ToIso(), Vector3.up);
                playerModel.rotation = Quaternion.RotateTowards(playerModel.rotation, rotationR, 2 * turnSpeed * turnSpeedMagnitude * Time.deltaTime);
            }
        }


    }

    public void Run()
    {

        // use an idle state as improvement
        if (!isRunning || isDashing) return;

        // move character
        Vector3 moveVelocity = movementInput.normalized * moveSpeed;
        characterController.Move(moveVelocity.ToIso() * Time.deltaTime);

        // adjust body to always face the direction of motion depending on fight mode
        if (fightMode) return;
        transform.LookAt(moveDirection);
    }

    public void OnDash(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && !isDashing)
        {
            // Stop looking at cursor
            lookAtAim -= LookAtAim;

            // Turn body in direction of motion
            transform.LookAt(moveDirection);

            // Start animation
            animator.Play("Dash");
            animator.SetBool("isDashing", true);
            isDashing = true;
            Invoke("StopDash", dashTime);
        }
    }

    public void Dash()
    {
        if (!isDashing) return;

        Vector3 dash = transform.forward * dashDistance;
        characterController.Move(dash * dashSpeed * Time.deltaTime);
    }

    public void StopDash()
    {
        animator.SetBool("isDashing", false);
        isDashing = false;

        // Follow cursor
        lookAtAim += LookAtAim;
    }

    public void Update()
    {
        // Always track movement direction
        moveDirection = transform.position + movementInput.ToIso();

        velocityZ = Vector3.Dot(movementInput.normalized.ToIso(), transform.forward);
        velocityX = Vector3.Dot(movementInput.normalized.ToIso(), transform.right);
        animator.SetFloat("VelocityZ", velocityZ, 0.1f, Time.deltaTime);
        animator.SetFloat("VelocityX", velocityX, 0.1f, Time.deltaTime);

        Run();
        Dash();
        lookAtAim?.Invoke();
        FightModeCountdown();
    }
}
