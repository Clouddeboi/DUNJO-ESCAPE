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

    public void doDash(InputAction.CallbackContext context)
    {
        if (pm.canDash)
        {
            //AudioManager.PlaySFX(AudioManager.Dash);
            StartCoroutine(doDash());
        }
    }

    private IEnumerator doDash()
    {
        pm.canDash = false;
        pm.isDashing = true;
        pm.playerSprite.color = new Color(pm.playerSprite.color.r * 4, pm.playerSprite.color.g * 2, pm.playerSprite.color.b * 2);
        float originalGravity = rb.gravityScale;//this is because we dont want our player to be affected by gravity while dashing
        rb.gravityScale = 0f;//variable that stores gravity since we want to apply concept above^
        rb.velocity = new Vector2(transform.localScale.x * pm.dashingPower, 0f);//indicates direction player is facing
        pm.tr.emitting = true;//emits trail
        yield return new WaitForSeconds(pm.dashingTime);//stop dashing for a few secs
        pm.tr.emitting = false;//trail emitting off
        rb.gravityScale = originalGravity;//sets gravity back to original
        pm.isDashing = false;//we cant dash
        yield return new WaitForSeconds(pm.dashingCooldown);//waits for a few seconds(correspondng to dashing cooldown)
        pm.canDash = true;//sets can dash back to true afterwards
        pm.playerSprite.color = pm.ogPlayerColour;
    }
}