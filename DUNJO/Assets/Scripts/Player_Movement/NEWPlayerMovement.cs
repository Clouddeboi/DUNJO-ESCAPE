using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private float WallSlidingSpeed = 2f;
    private float WallJumpingDirection;//wall jumping direction
    [SerializeField] private float wallJumpingTime = 0.2f;//time wall jumping
    private float wallJumpingCounter;//wall jump counter
    [SerializeField] private float wallJumpingDuration = 0.4f;//wall jumping duration
    private Vector2 wallJumpingPower = new Vector2(8f, 16f);//power of wall jump

    [SerializeField] private Transform wallcheck;
    [SerializeField] private LayerMask wallLayer;

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

        WallSlide();

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

        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
        
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
            if (IsWalled() && wallJumpingCounter > 0f) 
            {
                WallJump();
            }

            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);

            jumpBufferCounter = 0f;

            canDoubleJump = !canDoubleJump;
        }

        if (context.canceled && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);

            coyoteTimeCounter = 0f;
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private bool IsWalled()
    {
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

    //WHERE DOES THE ARC JUMP THING GO? Where the player is pushed away from the wall and can't influence direction
    //Doesnt it have to be a coroutine in here somewhere?
    private void WallJump()
   {
        if(isWallSliding)
        {
            isWallJumping = false;//if we are wall sliding we are not wall jumping
            WallJumpingDirection = -transform.localScale.x;//flips the direction that the player is facing
            wallJumpingCounter = wallJumpingTime;
            CancelInvoke(nameof(StopWallJumping));//cancels method if player is wall sliding
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }

        if(wallJumpingCounter > 0f)
        {
            //Audio goes here
            isWallJumping = true;
            canDoubleJump = true;//this is so we can double jump off the walls
            rb.velocity = new Vector2(WallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            wallJumpingCounter = 0f;//prevents spamming jump button

            if(transform.localScale.x != WallJumpingDirection)//flips player to face direction of movement
            {
                isFacingRight = !isFacingRight;
                Vector3 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }

            Invoke(nameof(StopWallJumping), wallJumpingDuration);//invoke method with a delay
        }
   }

   private void StopWallJumping()
   {
        isWallJumping = false;
   }
}

