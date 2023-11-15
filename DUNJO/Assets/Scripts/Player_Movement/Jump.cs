using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class Jump : MonoBehaviour
{
    private PlayerMovementManager pm;
    [SerializeField] private Rigidbody2D rb;

    private void Awake() 
    {
        pm = GetComponent<PlayerMovementManager>();
    }

    public void JumpButton(InputAction.CallbackContext context)
    {
        if(pm.IsGrounded())
        {
            pm.coyoteTimeCounter = pm.coyoteTime;
            pm.doubleJump = false;
        }
        else
        {
            pm.coyoteTimeCounter -= Time.deltaTime;
        }

        if(context.performed)
        {
            pm.jumpBufferCounter = pm.jumpBufferTime;
        }
        else
        {
            pm.jumpBufferCounter -= Time.deltaTime;
        }

        if (pm.jumpBufferCounter > 0f && pm.coyoteTimeCounter > 0f || pm.jumpBufferCounter > 0f && pm.doubleJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, pm.jumpingPower);

            pm.jumpBufferCounter = 0f;

            pm.doubleJump = !pm.doubleJump;
        }

        if (context.canceled && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);

            pm.coyoteTimeCounter = 0f;
        }
        
        if (pm.isWallSliding)
        {
            
            pm.isWallJumping = false;
            pm.wallJumpingDirection = -transform.localScale.x;
            pm.wallJumpingCounter = pm.wallJumpingTime;
            pm.cannotTurnForTimer = pm.wallJumpingDuration;

            CancelInvoke(nameof(StopWallJumping));
        }
        else
        {
            pm.wallJumpingCounter -= Time.deltaTime;
        }

        if (context.performed && pm.wallJumpingCounter > 0f)
        {
            if (pm.isWallSliding) 
            {
                pm.Flip();
            }

            pm.isWallJumping = true;
            rb.velocity = new Vector2(pm.wallJumpingDirection * pm.wallJumpingPower.x, pm.wallJumpingPower.y);
            pm.wallJumpingCounter = 0f;

            Invoke(nameof(StopWallJumping), pm.wallJumpingDuration);
        }
    }

    public void WallSlide()
    {
        if(pm.IsWalled() && !pm.IsGrounded() && pm.horizontal != 0f)//if we arent on the ground and we are at a wall set wall sliding to true
        {
            pm.isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -pm.WallSlidingSpeed, float.MaxValue));
        }
        else 
        {
            pm.isWallSliding = false;//else set to false
        }
   }

    public void StopWallJumping()
    {
        pm.isWallJumping = false;
    }

}
