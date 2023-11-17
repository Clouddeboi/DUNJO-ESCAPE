using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class GroundPound : MonoBehaviour
{
    private PlayerMovementManager pm;//THIS IS OUR PLAYER MOVEMENT SCRIPT (pm = Player Movement)
    private Dash d;

    [SerializeField] public Rigidbody2D rb;

    public bool isGroundpounding = false;
        private void Awake()
        {
            pm = GetComponent<PlayerMovementManager>();
            d = GetComponent<Dash>();
        }

    public void PoundInput(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            if(!pm.IsGrounded())
            {
                pm.doGroundPound = true;
            }
        }
    }

    public void OnCollisionEnter2D(Collision2D other)
    {
        if(other.contacts[0].normal.y > 0.5)
        {
            CompleteGroundPound();
        }
    }

    public void GroundPoundAttack()
    {
        pm.enabled = false;
        StopAndSpin();
        StartCoroutine("DropAndSmash");
    }

    public void StopAndSpin()
    {
        isGroundpounding = true;
        pm.canDash = false;
        pm.doubleJump = false;
        pm.speed = 0f;
        ClearForces();  
        //rb.gravityScale = 0;
        //we can change how we freeze our position here 
    }

    public IEnumerator DropAndSmash()
    {
        yield return new WaitForSeconds(pm.stopTime);
        rb.AddForce(Vector2.down * pm.dropForce, ForceMode2D.Impulse);
    }

    public void CompleteGroundPound()
    {
        rb.gravityScale = pm.gravityScale;
        pm.enabled = true;
        isGroundpounding = false;
        pm.canDash = true;
        pm.doubleJump = true;
        pm.speed = 20f;
    }

    public void ClearForces()
    {
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0;
    }
}
