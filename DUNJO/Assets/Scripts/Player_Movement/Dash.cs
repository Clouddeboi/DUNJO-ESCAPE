using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class Dash : MonoBehaviour
{
    [Header("Dash Settings")]
    private PlayerMovementManager pm; //Link to player movement script
    [SerializeField] private Rigidbody2D rb;

    private void Awake() 
    {
        pm = GetComponent<PlayerMovementManager>();
    }

    public void DashInput(InputAction.CallbackContext context)
    {
        if (context.performed && pm.canDash)
        {
            //AudioManager.PlaySFX(AudioManager.Dash);
            if(pm.WaitTime >= pm.dashingCooldown)
            {
                pm.WaitTime = 0f;
                Invoke("Dashing",0);
            }
        }
    }
    

    public void Dashing()
    {
        pm.canDash = false;
        pm.isDashing = true;
        pm.tr.emitting = true;
        rb.gravityScale = pm.dashGravity;

        if(pm.horizontal == 0)
        {
            if(pm.isFacingRight)
            {
                rb.velocity = new Vector2(transform.localScale.x * pm.dashingPower,0);
            }
            if(!pm.isFacingRight)
            {
                rb.velocity = new Vector2(transform.localScale.x * pm.dashingPower,0);
            }
        }
        else
        {
            rb.velocity = new Vector2(pm.horizontal * pm.dashingPower,0);
        }
        Invoke("StopDash",pm.dashingTime);
    }

    public void StopDash()
    {
        pm.canDash = true;
        pm.isDashing = false;
        pm.tr.emitting = false;
        rb.gravityScale = pm.gravityScale;
    }
}