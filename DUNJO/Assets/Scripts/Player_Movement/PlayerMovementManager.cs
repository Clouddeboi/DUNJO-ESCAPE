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


    [Header("Movement Variables")]
    public float horizontal;
    public float speed = 20f;

    [Header("Wall-Jumping+Sliding Variables")]

    public float WallSlidingSpeed = 2f;
    public float wallJumpingTime = 0.2f;//time wall jumping
    public float wallJumpingCounter;//wall jump counter
    public float cannotTurnForTimer = 0f; //Timer that counts down from wallJumpingTime, once at 0f the player can do input
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
    public float dashingPower = 20f;//dashing power
    public float dashingTime = 0.2f;//time spent dashing
    public float dashingCooldown = 2f;//cooldown of dash ability
    public float WaitTime;
    public float dashGravity = 0f;
    public TrailRenderer tr;

    [Header("Ground Pound Variables")]
    public bool doGroundPound;
    public float dropForce = 5f;
    public float stopTime = 0.5f;
    public float gravityScale = 10f;

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
        WaitTime += Time.deltaTime;
        if (isDashing)
        {
            return;
        } 
    }

    private void FixedUpdate()
    {

        if(IsGrounded())
        {
            doubleJump = true;
            isWallJumping = false;
            wallJumpingCounter = 0f;
        }

        J.WallSlide();
        J.WallJump();

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
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontal = context.ReadValue<Vector2>().x;
    }

}

