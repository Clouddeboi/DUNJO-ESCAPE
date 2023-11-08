using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
//variables
    //Animations variables (Video followed: https://www.youtube.com/watch?v=hkaysu1Z-N8&t=567s)
    //public Animator animator;
    //basic movement and jump variables (Video Reference/Guide Follow: https://www.youtube.com/watch?v=K1xZ-rycYY8)
    private float horizontal;
    [SerializeField] private float speed = 8f;
    [SerializeField] private float jumpingPower = 16f;
    private bool isFacingRight = true;

    //coyote Time Variables(Video Reference/Guide Follow:https://www.youtube.com/watch?v=RFix_Kg2Di0&t=55s)
    [SerializeField] private float coyoteTime = 0.2f;//the higher the value the longer u have coyote time
    private float coyoteTimeCounter;

    //double jump variables (Video Reference/Guide Follow: https://www.youtube.com/watch?v=RdhgngSUco0)
    private bool doubleJump;

    //Wall slide and jump Variables (Video Reference/Guide Follow: https://www.youtube.com/watch?v=O6VX6Ro7EtA)
    private bool isWallSliding;//indicadtes wall climbing
    [SerializeField] private float WallSlidingSpeed = 2f;
    private bool isWallJumping;//indicates if player is wall jumping
    private float WallJumpingDirection;//wall jumping direction
    [SerializeField] private float wallJumpingTime = 0.2f;//time wall jumping
    private float wallJumpingCounter;//wall jump counter
    [SerializeField] private float wallJumpingDuration = 0.4f;//wall jumping duration
    private Vector2 wallJumpingPower = new Vector2(8f, 16f);//power of wall jump
    private SpriteRenderer playerSprite;
    private Color ogPlayerColour;

    //dash variables (Video Reference/Guide Follow: https://www.youtube.com/watch?v=2kFGmuPHiA0)
    private bool canDash = true;//determines if player can dash
    private bool isDashing;//determines if player is already dashing
    [SerializeField] private float dashingPower = 24f;//dashing power
    [SerializeField] private float dashingTime = 0.2f;//time spent dashing
    [SerializeField] private float dashingCooldown = 1f;//cooldown of dash ability

    //respawn and death variables
    Vector2 CheckpointPos;//players starting position(Edit: Changed to checkpoint position, its only starting pos at the start of the game)

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private TrailRenderer tr;
    [SerializeField] private Transform wallcheck;
    [SerializeField] private LayerMask wallLayer;

    

    // AudioManager AudioManager;

    // private void Awake()
    // {
    //     AudioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    // }

    //video follwed for death and respawn https://www.youtube.com/watch?v=odStG_LfPMQ&ab_channel=RehopeGames
    
    //video follwed for death and respawn https://www.youtube.com/watch?v=odStG_LfPMQ&ab_channel=RehopeGames
    private void Start()
    {
        CheckpointPos = transform.position;
        rb = GetComponent<Rigidbody2D>();
        playerSprite = GetComponent<SpriteRenderer>();
        ogPlayerColour = playerSprite.color;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Obstacle"))
        {
            //AudioManager.PlaySFX(AudioManager.Death);
            Die();
        }
    }

    public void UpdateCheckpoint(Vector2 pos)
    {
        CheckpointPos = pos;
    }

    void Die()
    {
        Respawn();
    }

    void Respawn()
    {
        transform.position = CheckpointPos; 
    }
    
    // Update is called once per frame
    private void Update()
    {

        
        
        horizontal = Input.GetAxisRaw("Horizontal");

        if(IsGrounded())
        {
            coyoteTimeCounter = coyoteTime;//if we are grounded set the time counter to the coyote time
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;//if we arent grounded subtract the time
        }

        if (coyoteTimeCounter > 0f && Input.GetButtonDown("Jump"))//check if coyote time is greater than 0 and if jump button is pressed
        {
            //AudioManager.PlaySFX(AudioManager.Jump);
            doubleJump = false;
        }

        if (!doubleJump) 
        {
            
        }

        if (Input.GetButtonDown("Jump")) 
        {
            if (coyoteTimeCounter > 0f || doubleJump)
            {
                //AudioManager.PlaySFX(AudioManager.Jump);
                rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
                doubleJump = !doubleJump;
            }
        }

        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)//allows the player to jump higher by pressing the jump button(space) longer by multiply it by 0.5
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            coyoteTimeCounter = 0f;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash || Input.GetButtonDown("Fire3")&& canDash)//FINALLY FIXED THIS ISSUE IM SUCH A DUMBASS HDYISBHUBHBVHGIYUUG
        {
            //AudioManager.PlaySFX(AudioManager.Dash);
            StartCoroutine(Dash());
        }

        WallSlide();
        WallJump();

        if(!isWallJumping)
        {
            Flip();
        }
    }

    private void FixedUpdate()
    {
        if(!isWallJumping)
        {
            if (isDashing)
            {
                return;
            }
            
            rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
        }
    }

    private bool IsGrounded()//checks if player is grounded
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

        if(Input.GetButtonDown("Jump") && wallJumpingCounter > 0f)
        {
            isWallJumping = true;
            //doubleJump = true;//this is so we can double jump off the walls
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

    private void Flip()
    {
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            Vector3 localScale = transform.localScale;
            isFacingRight = !isFacingRight;
            localScale.x *= -1f;
            transform.localScale = localScale;
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
