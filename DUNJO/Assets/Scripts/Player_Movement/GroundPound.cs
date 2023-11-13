using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class GroundPound : MonoBehaviour
{
    private NEWPlayerMovement pm;//THIS IS OUR PLAYER MOVEMENT SCRIPT (pm = Player Movement)
    [SerializeField]private Rigidbody2D rb;
    [SerializeField] private bool doGroundPound;
    [SerializeField] private float dropForce = 5f;
    [SerializeField] private float stopTime = 0.5f;
    [SerializeField] private float gravityScale = 1f;

    private bool isGroundpounding = false;
        private void Awake()
        {
            pm = GetComponent<NEWPlayerMovement>();
        }

        private void FixedUpdate()
        {
            if(!pm.isWallJumping)
            {
                if(doGroundPound && !isGroundpounding)
                {
                    GroundPoundAttack();
                }
                doGroundPound = false;
            }
            
        }

    public void PoundInput(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            if(!pm.IsGrounded())
            {
                doGroundPound = true;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.contacts[0].normal.y > 0.5)
        {
            CompleteGroundPound();
        }
    }

    private void GroundPoundAttack()
    {
        pm.enabled = false;
        StopAndSpin();
        StartCoroutine("DropAndSmash");
    }

    private void StopAndSpin()
    {
        isGroundpounding = true;
        pm.canDash = false;
        pm.doubleJump = false;
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
        pm.enabled = true;
        isGroundpounding = false;
        pm.canDash = true;
        pm.doubleJump = true;
    }

    private void ClearForces()
    {
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0;
    }
}
