using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Status")]
    [SerializeField] float mass;
    [SerializeField] float drag;
    private float currentSpeed;

    public float sprintSpeed = 1.4f;
    public float walkSpeed = 0.6f;
    public float normalSpeed = 3f;
    public bool isGround = false;
    [Header("Crouch")]
    public float crouchSpeed = 0.3f;
    private float originHeight;
    public LayerMask whatIsGround;
    public float playerHeight = 1f;
    public float jumpForce = 8f;
    private PlayerInputHandler input;
    private CharacterController charCtrl;
    Vector3 direction;
    float terminalVelocity = 55f;
    public float jumpTimeout = .1f;
    public float fallTimeout = .15f;
    float verticalVelocity = 0f;
    // timeout deltatime
    private float jumpTimeoutDelta;
    private float fallTimeoutDelta;


    public enum MovementState
    {
        idle,
        normal,
        walking,
        sprinting,
        crouching,
        air
    }

    public MovementState state;

    private void Awake()
    {
        TryGetComponent(out input);
        TryGetComponent(out charCtrl);
    }

    private void Start()
    {
        jumpTimeoutDelta = jumpTimeout;
        fallTimeoutDelta = fallTimeout;

        originHeight = charCtrl.height;
    }

    private void Update()
    {
        OnGround();
        OnCeil();
        Movement();
        StateHandler();
    }

    private void FixedUpdate()
    {
        MovementPlayer();
        MovementPlayerJump();
        MovementPlayerCrouch();
        MovementPlayerGravity();
    }

    private void OnGround()
    {
        isGround = Physics.Raycast(transform.position + new Vector3(0f, charCtrl.height * .5f), Vector3.down, charCtrl.height * .5f + .3f, whatIsGround);
    }
    private void OnCeil()
    {
        if (verticalVelocity <= 0) return;

        if (!Physics.Raycast(transform.position + new Vector3(0f, charCtrl.height * .5f), Vector3.up, charCtrl.height * .5f + .1f, whatIsGround)) return;

        verticalVelocity *= -1f;
    }
    private void Movement()
    {
        direction = gameObject.transform.forward * input.move.y + gameObject.transform.right * input.move.x;

        direction *= currentSpeed;
        direction.y = verticalVelocity;
    }

    private void MovementPlayer()
    {
        charCtrl.Move(direction * Time.deltaTime);
    }

    private void MovementPlayerJump()
    {
        if (isGround)
        {
            fallTimeoutDelta = fallTimeout;

            if (verticalVelocity < 0f)
            {
                verticalVelocity = -2f;
            }

            if (input.isJump && jumpTimeoutDelta <= 0f)
            {
                verticalVelocity = Mathf.Sqrt(jumpForce * -2f * -9.81f);
            }

            if (jumpTimeoutDelta >= 0f)
            {
                jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            jumpTimeoutDelta = jumpTimeout;

            if (fallTimeoutDelta >= 0f)
            {
                fallTimeoutDelta -= Time.deltaTime;
            }
        }
    }


    private void MovementPlayerCrouch()
    {
        if (state == MovementState.crouching)
        {
            charCtrl.height = originHeight * .5f;

        }
        else
        {
            charCtrl.height = originHeight;
        }
    }
    private void MovementPlayerGravity()
    {
        if (verticalVelocity < terminalVelocity)
        {
            verticalVelocity += -9.81f * mass * drag * Time.deltaTime;
        }
    }
    private void StateHandler()
    {
        if (input.isWalk)
        {
            state = MovementState.walking;
            currentSpeed = walkSpeed;
        }
        else if (input.isSprint)
        {
            state = MovementState.sprinting;
            currentSpeed = sprintSpeed;
        }
        else if (input.isCrouch)
        {
            state = MovementState.crouching;
            currentSpeed = crouchSpeed;
        }
        else if (isGround)
        {
            state = MovementState.normal;
            currentSpeed = normalSpeed;
        }
        else
        {
            state = MovementState.air;
        }
    }
}
