using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class NEWPlayerMovement : MonoBehaviour
{
    [Header("Component Links")]
    public Dash dash; //Dash Script
    public Rigidbody2D rb;
    public Transform groundCheck;
    public LayerMask groundLayer;
    [SerializeField]private float coyoteTime = 0.2f;
    private float coyoteTimeCounter;
    [SerializeField] private float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;
    [SerializeField]private float horizontal;

    [Header("Jump Settings")]
    [SerializeField] private float speed = 8f;
    [SerializeField] private float jumpingPower = 30f;
    public SpriteRenderer playerSprite;
    public Color ogPlayerColour;
    [SerializeField] private float WallSlidingSpeed = 2f;
    [SerializeField] private float wallJumpingTime = 0.2f;//time wall jumping
    [SerializeField] private float wallJumpingCounter;//wall jump counter
    [SerializeField] private float cannotTurnForTimer = 0f; //Timer that counts down from wallJumpingTime, once at 0f the player can do input
    [SerializeField] private float wallJumpingDuration = 0.4f;//wall jumping duration
    [SerializeField] private Vector2 wallJumpingPower = new Vector2(8f, 16f);//power of wall jump
    [SerializeField] private Transform wallcheck;
    [SerializeField] private LayerMask wallLayer;
    private float wallJumpingDirection;

    [Header ("Booleans")]
    [SerializeField] private bool isWallSliding;//indicates wall climbing
    [SerializeField] public bool isWallJumping;//indicates if player is wall jumping
    [SerializeField] private bool isFacingRight = true;
    [SerializeField] public bool doubleJump;
    /* MAYBE DELETE THESE AFTER? */[SerializeField] private bool isGrounded;
    /* MAYBE DELETE THESE AFTER? */[SerializeField] private bool isWalled;

    private void Awake()
    {
        dash = GetComponent<Dash>();
        playerSprite = GetComponent<SpriteRenderer>();
        ogPlayerColour = playerSprite.color;
    }

    private void Start()
    {
        
    }

    void Update()
    {
        WallSlide();
    }

    private void FixedUpdate()
    {
        if (!isFacingRight && horizontal > 0f && cannotTurnForTimer <= 0f)
        {
            Flip();
        }
        else if (isFacingRight && horizontal < 0f && cannotTurnForTimer <= 0f)
        {
            Flip();
        }

        if(IsGrounded())
        {
            doubleJump = true;
            isWallJumping = false;
            wallJumpingCounter = 0f;
            cannotTurnForTimer = 0f;
        }

        else {
            cannotTurnForTimer -= Time.deltaTime;
        }
        
        if(!isWallSliding && wallJumpingCounter > 0f)
        {
            Flip();
        }

        if (!isWallJumping)
        {
            if (dash.isDashing)
            {
                return;
            }
            rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
        }
    }



    public void Jump(InputAction.CallbackContext context)
    {
        if(IsGrounded())
        {
            coyoteTimeCounter = coyoteTime;
            doubleJump = false;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if(context.performed)
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f || jumpBufferCounter > 0f && doubleJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);

            jumpBufferCounter = 0f;

            doubleJump = !doubleJump;
        }

        if (context.canceled && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);

            coyoteTimeCounter = 0f;
        }
        
        if (isWallSliding)
        {
            
            isWallJumping = false;
            wallJumpingDirection = -transform.localScale.x;
            wallJumpingCounter = wallJumpingTime;
            cannotTurnForTimer = wallJumpingDuration;

            CancelInvoke(nameof(StopWallJumping));
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }

        if (context.performed && wallJumpingCounter > 0f)
        {
            if (isWallSliding) {
                Flip();
            }

            isWallJumping = true;
            rb.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            wallJumpingCounter = 0f;

            Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }
    }

    private void StopWallJumping()
    {
        isWallJumping = false;
    }
    public bool IsGrounded()
    {
        /* MAYBE DELETE THESE AFTER? */isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private bool IsWalled()
    {
        /* MAYBE DELETE THESE AFTER? */isWalled = Physics2D.OverlapCircle(wallcheck.position, 0.2f, wallLayer);
        return Physics2D.OverlapCircle(wallcheck.position, 0.2f, wallLayer);//checks if the player is colliding with a wall     
    }

    private void WallSlide()
   {
        if(IsWalled() && !IsGrounded() && horizontal != 0f)//if we arent on the ground and we are at a wall set wall sliding to true
        {
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -WallSlidingSpeed, float.MaxValue));
        }
        else 
        {
            isWallSliding = false;//else set to false
        }
   }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontal = context.ReadValue<Vector2>().x;

        if (horizontal > 0f)
        {
            if (!isFacingRight)
            {
                Flip();
            }
        }
        else if (horizontal < 0f)
        {
            if (isFacingRight)
            {
                Flip();
            }
        }
    }

}

