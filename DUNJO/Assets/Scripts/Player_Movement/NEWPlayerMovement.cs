using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class NEWPlayerMovement : MonoBehaviour
{
    [Header("Component Links")]
    [SerializeField] private Rigidbody2D rb;
    public Transform groundCheck;
    public LayerMask groundLayer;

    [SerializeField]private float coyoteTime = 0.2f;
    private float coyoteTimeCounter;

    [SerializeField] private float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;

    private float horizontal;

    [Header("Jump Settings")]

    [SerializeField] private float speed = 8f;
    [SerializeField] private float jumpingPower = 16f;
    [SerializeField] private float dashingPower = 24f;//dashing power
    [SerializeField] private float dashingTime = 0.2f;//time spent dashing
    [SerializeField] private float dashingCooldown = 1f;//cooldown of dash ability
    [SerializeField] private TrailRenderer tr;

    [Header ("Booleans")]
    [SerializeField] private bool isWallSliding;//indicadtes wall climbing
    [SerializeField] private bool isWallJumping;//indicates if player is wall jumping
    [SerializeField] private bool canDash = true;//determines if player can dash
    [SerializeField] private bool isFacingRight = true;
    [SerializeField] private bool isDashing;//determines if player is already dashing
    [SerializeField] private bool canDoubleJump;
    
    private SpriteRenderer playerSprite;
    private Color ogPlayerColour;
    private float WallJumpingDirection;//wall jumping direction
    [SerializeField] private float wallJumpingTime = 0.2f;//time wall jumping
    private float wallJumpingCounter;//wall jump counter
    [SerializeField] private float wallJumpingDuration = 0.4f;//wall jumping duration
    private Vector2 wallJumpingPower = new Vector2(8f, 16f);//power of wall jump
    

    [SerializeField] private Transform wallcheck;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float WallSlidingSpeed = 2f;

    public float baseGravity = 2f;
    public float maxFallSpeed = 18f;
    public float fallGravityMult = 2f;
    private void Start()
    {
        playerSprite = GetComponent<SpriteRenderer>();
        ogPlayerColour = playerSprite.color;
    }

    void Update()
    {

        if (!isFacingRight && horizontal > 0f)
        {
            Flip();
        }
        else if (isFacingRight && horizontal < 0f)
        {
            Flip();
        }
        ProcessWallSlide();
        //ProcessGravity();
        ProcessWallJump();

        if(!isWallJumping)
        {
            rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
            Flip();
        }


    }

    private void FixedUpdate()
    {
        if (IsGrounded()) {
            canDoubleJump = true;
        }

        if (isDashing)
        {
            return;
        }

        //rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
        
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if(IsGrounded())
        {
            coyoteTimeCounter = coyoteTime;
            canDoubleJump = false;
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

        if ((jumpBufferCounter > 0f && coyoteTimeCounter > 0f) || (jumpBufferCounter > 0f && canDoubleJump) || (jumpBufferCounter > 0f && IsWalled()))
        {
            //using iswalled() makes it so that you just have to be next to wall to refresh jump
            //using iswallsliding makes it so you have to hold direction into wall to refresh jump

            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);

            jumpBufferCounter = 0f;

            canDoubleJump = !canDoubleJump;
        }

        if (context.canceled && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);

            coyoteTimeCounter = 0f;
        }

        if(context.performed && wallJumpingCounter > 0f)
        {
            isWallJumping = true;
            rb.velocity = new Vector2(WallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            wallJumpingCounter = 0;

            if(transform.localScale.x != WallJumpingDirection)
            {
                isFacingRight = !isFacingRight;
                Vector3 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }

            Invoke(nameof(CancelWallJump), wallJumpingTime + 0.1f);
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    // private void ProcessGravity()
    // {
    //     if(rb.velocity.y < 0)
    //     {
    //         rb.gravityScale = baseGravity * fallGravityMult;
    //         rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -maxFallSpeed));
    //     }
    //     else
    //     {
    //         rb.gravityScale = baseGravity;
    //     }
    // }
  
    
    private void ProcessWallSlide()
    {
        if(!IsGrounded() && IsWalled() && horizontal != 0)
        {
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -WallSlidingSpeed));
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void CancelWallJump()
    {
        isWallJumping = false;
    }

    private void ProcessWallJump()
    {
        if(isWallSliding)
        {   
            isWallJumping = false;
            WallJumpingDirection = -transform.localScale.x;
            wallJumpingCounter = wallJumpingTime;

            CancelInvoke(nameof(CancelWallJump));
        }
        else if(wallJumpingCounter > 0f)
        {
            wallJumpingCounter -= Time.deltaTime;
        }
    }
    
    private bool IsWalled()
    {
        return Physics2D.OverlapCircle(wallcheck.position, 0.2f, wallLayer);//checks if the player is colliding with a wall     
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
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if (canDash)
        {
            //AudioManager.PlaySFX(AudioManager.Dash);
            StartCoroutine(Dash());
        }

    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        playerSprite.color = new Color(playerSprite.color.r * 4, playerSprite.color.g * 2, playerSprite.color.b * 2);
        float originalGravity = rb.gravityScale;//this is because we dont want our player to be affected by gravity while dashing
        rb.gravityScale = 0f;//variable that stores gravity since we want to apply concept above^
        rb.velocity = new Vector2(transform.localScale.x * dashingPower, 0f);//indicates direction player is facing
        tr.emitting = true;//emits trail
        yield return new WaitForSeconds(dashingTime);//stop dashing for a few secs
        tr.emitting = false;//trail emitting off
        rb.gravityScale = originalGravity;//sets gravity back to original
        isDashing = false;//we cant dash
        yield return new WaitForSeconds(dashingCooldown);//waits for a few seconds(correspondng to dashing cooldown)
        canDash = true;//sets can dash back to true afterwards
        playerSprite.color = ogPlayerColour;
    }

}

