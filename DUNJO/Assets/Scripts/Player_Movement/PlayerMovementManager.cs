using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PlayerMovementManager : MonoBehaviour
{
    [Header("Scripts")]

    public GroundPound GP;//Ground Pound Script
    public Dash dash; //Dash Script
    public Jump J;

    [Header("Components")]

    public Rigidbody2D rb;
    public Transform groundCheck;
    public LayerMask groundLayer;

    [Header("Sprite Settings")]
    public SpriteRenderer playerSprite;
    public Color ogPlayerColour;
    [SerializeField] public bool isFacingRight = true;
  
    [Header("Jump Settings")]
    private bool isGrounded;
    public float coyoteTime = 0.2f;
    public float coyoteTimeCounter;
    public float jumpBufferTime = 0.2f;
    public float jumpBufferCounter;
    public float jumpingPower = 30f;
    public bool doubleJump;
    public int jumpsRemaining;
    public int maxJumps =2;

    // [Header("Gravity")]
    // public float baseGravity = 10f;
    // public float maxFallSpeed =15f;
    // public float fallSpeedMultiplier = 3f;


    [Header("Movement Variables")]
    public float horizontal;
    public float speed = 20f;
    public ParticleSystem dust;
    public float acceleration;
    public float maxSpeedChange;

    [Header("Wall-Jumping+Sliding Variables")]

    public float WallSlidingSpeed = 2f;
    public float wallJumpingTime = 0.2f;//time wall jumping
    public float wallJumpingCounter;//wall jump counter
    //public float cannotTurnForTimer = 0f; //Timer that counts down from wallJumpingTime, once at 0f the player can do input
    public float wallJumpingDuration = 0.4f;//wall jumping duration
    public Vector2 wallJumpingPower = new Vector2(8f, 16f);//power of wall jump
    public Transform wallcheck;
    public LayerMask wallLayer;
    public float wallJumpingDirection;
    public bool isWallSliding;//indicates wall climbing
    public bool isWallJumping;//indicates if player is wall jumping
    public bool isWalled;
    public bool WallJumpInputPressed;

    [Header("Dash Variables")]
    public bool canDash = true;//determines if player can dash
    public bool isDashing;//determines if player is already dashing
    public float dashingPower = 40f;//dashing power
    public float dashingTime = 0.2f;//time spent dashing
    public float dashingCooldown = 2f;//cooldown of dash ability
    public float WaitTimeDash;
    public float dashGravity = 0f;
    public TrailRenderer tr;

    [Header("Ground Pound Variables")]
    public bool doGroundPound;
    public float dropForce = 5f;
    public float stopTime = 0.5f;
    public float gravityScale = 10f;
    public float GroundPoundCooldown = 0.5f;
    public float WaitTimeGP;

    private void Awake()
    {
        GP = GetComponent<GroundPound>();
        dash = GetComponent<Dash>();
        J = GetComponent<Jump>();
        playerSprite = GetComponent<SpriteRenderer>();
        ogPlayerColour = playerSprite.color;
    }

    void Update()
    {
        WaitTimeDash += Time.deltaTime;
        WaitTimeGP += Time.deltaTime;
        if (isDashing)
        {
            return;
        } 
    }

    private void FixedUpdate()
    {

        if(IsGrounded())
        {
            //doubleJump = true;
            isWallJumping = false;
            wallJumpingCounter = 0f;
        }

        J.WallSlide();
        J.WallJump();
        RefreshJump();
        //Gravity();

        if (!isWallJumping)
        {
            if (!isFacingRight && horizontal > 0f)
            {
                Flip();
            }
            else if (isFacingRight && horizontal < 0f)
            {
                Flip();
            }
            if (isDashing)
            {
                return;
            } 
            if(doGroundPound && !GP.isGroundpounding)
            {
                GP.GroundPoundAttack();
                doGroundPound = false;
            }

            rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
        }


    }

    public bool IsGrounded()
    {
        /* MAYBE DELETE THESE AFTER? */isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.5f, groundLayer);
        return isGrounded;
    }

    public void RefreshJump()
    {
        if(Physics2D.OverlapCircle(groundCheck.position, 0.5f, groundLayer))
        {
            jumpsRemaining = maxJumps;
        }
    }

    // public void Gravity()
    // {
    //     if(rb.velocity.y < 0)
    //     {
    //         rb.gravityScale = baseGravity * fallSpeedMultiplier;
    //         rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -maxFallSpeed));
    //     }
    //     else
    //     {
    //         rb.gravityScale = baseGravity;
    //     }
    // }

    public bool IsWalled()
    {
        /* MAYBE DELETE THESE AFTER? */isWalled = Physics2D.OverlapCircle(wallcheck.position, 0.2f, wallLayer);
        return isWalled;    
    }

    public void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;

        if(IsGrounded())
        {
           dust.Play(); 
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontal = context.ReadValue<Vector2>().x;
    }

}

