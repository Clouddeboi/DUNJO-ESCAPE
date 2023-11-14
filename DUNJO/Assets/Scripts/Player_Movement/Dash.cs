using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class Dash : MonoBehaviour
{
    [Header("Dash Settings")]
    [SerializeField] private NEWPlayerMovement pm; //Link to player movement script
    public bool canDash = true;//determines if player can dash
    public bool isDashing;//determines if player is already dashing
    public float dashingPower = 20f;//dashing power
    public float dashingTime = 0.2f;//time spent dashing
    public float dashingCooldown = 1f;//cooldown of dash ability
    public TrailRenderer tr;

    private void Awake() 
    {
        pm = GetComponent<NEWPlayerMovement>();
    }

    public void doDash(InputAction.CallbackContext context)
    {
        if (canDash)
        {
            //AudioManager.PlaySFX(AudioManager.Dash);
            StartCoroutine(doDash());
        }
    }

    private IEnumerator doDash()
    {
        canDash = false;
        isDashing = true;
        pm.playerSprite.color = new Color(pm.playerSprite.color.r * 4, pm.playerSprite.color.g * 2, pm.playerSprite.color.b * 2);
        float originalGravity = pm.rb.gravityScale;//this is because we dont want our player to be affected by gravity while dashing
        pm.rb.gravityScale = 0f;//variable that stores gravity since we want to apply concept above^
        pm.rb.velocity = new Vector2(transform.localScale.x * dashingPower, 0f);//indicates direction player is facing
        tr.emitting = true;//emits trail
        yield return new WaitForSeconds(dashingTime);//stop dashing for a few secs
        tr.emitting = false;//trail emitting off
        pm.rb.gravityScale = originalGravity;//sets gravity back to original
        isDashing = false;//we cant dash
        yield return new WaitForSeconds(dashingCooldown);//waits for a few seconds(correspondng to dashing cooldown)
        canDash = true;//sets can dash back to true afterwards
        pm.playerSprite.color = pm.ogPlayerColour;
    }
}