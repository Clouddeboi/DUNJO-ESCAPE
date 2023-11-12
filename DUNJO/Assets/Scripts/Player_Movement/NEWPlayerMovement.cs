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
    [SerializeField]private float horizontal;

    [Header("Jump Settings")]
    [SerializeField] private float speed = 8f;
    [SerializeField] private float jumpingPower = 30f;
    [SerializeField] private float dashingPower = 20f;//dashing power
    [SerializeField] private float dashingTime = 0.2f;//time spent dashing
    [SerializeField] private float dashingCooldown = 1f;//cooldown of dash ability
    [SerializeField] private TrailRenderer tr;
    private SpriteRenderer playerSprite;
    private Color ogPlayerColour;
    [SerializeField] private float WallSlidingSpeed = 2f;
    [SerializeField] private float wallJumpingTime = 0.2f;//time wall jumping
    [SerializeField] private float wallJumpingCounter;//wall jump counter
    [SerializeField] private float cannotTurnForTimer = 0f; //Timer that counts down from wallJumpingTime, once at 0f the player can do input
    [SerializeField] private float wallJumpingDuration = 0.4f;//wall jumping duration
    [SerializeField] private Vector2 wallJumpingPower = new Vector2(8f, 16f);//power of wall jump
    [SerializeField] private Transform wallcheck;
    [SerializeField] private LayerMask wallLayer;
    private float wallJumpingDirection;
    [SerializeField] private float stopTime = 0.5f;
    [SerializeField] private float gravityScale = 1f;

    [Header ("Booleans")]
    [SerializeField] private bool isWallSliding;//indicates wall climbing
    [SerializeField] private bool isWallJumping;//indicates if player is wall jumping
    [SerializeField] private bool isFacingRight = true;
    [SerializeField] private bool canDash = true;//determines if player can dash
    [SerializeField] private bool isDashing;//determines if player is already dashing
    [SerializeField] private bool doubleJump;
    [SerializeField] private bool doGroundPound;
    [SerializeField] private float dropForce = 5f;
    /* MAYBE DELETE THESE AFTER? */[SerializeField] private bool isGrounded;
    /* MAYBE DELETE THESE AFTER? */[SerializeField] private bool isWalled;

    private void Start()
    {
        playerSprite = GetComponent<SpriteRenderer>();
        ogPlayerColour = playerSprite.color;
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
            if (isDashing)
            {
                return;
            }
            rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);

            if(doGroundPound)
            {
                GroundPoundAttack();
            }
            doGroundPound = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.contacts[0].normal.y > 0.5)
        {
            CompleteGroundPound();
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
    private bool IsGrounded()
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

public void GroundPound(InputAction.CallbackContext context)
{
    if(context.performed)
    {
        if(!IsGrounded())
        {
            doGroundPound = true;
        }
    }
}

private void GroundPoundAttack()
{
    StopAndSpin();
    StartCoroutine("DropAndSmash");
}

private void StopAndSpin()
{
    ClearForces();  
    rb.gravityScale = 0;
    //we can change how we freeze our position here 
}

private IEnumerator DropAndSmash()
{
    yield return new WaitForSeconds(stopTime);
    rb.AddForce(Vector2.down * dropForce, ForceMode2D.Impulse);
}

private void CompleteGroundPound()
{
    rb.gravityScale = gravityScale;
}

private void ClearForces()
{
    rb.velocity = Vector2.zero;
    rb.angularVelocity = 0;
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

