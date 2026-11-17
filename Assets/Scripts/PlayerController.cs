using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;




public class PlayerController : MonoBehaviour
{

    //intialise variables

    public float speed = 8f;
    
    public float jumpPower = 14f;
    
    //variables for dashing
    public float dashSpeed = 20f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 0.5f;
    private TrailRenderer trailRenderer;
    private int facingDirection = 1;
    private float defaultGravityScale;
    private bool isDashing;
    private bool canDash = true;

    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    //variables for wall sliding and jumping
    public float wallSlideSpeed = 2f;
    public float wallCheckDistance = 0.5f;
    public Vector2 wallJumpForce = new Vector2(10f, 14f);
    public float wallJumpDuration = 0.2f;
    private bool isWallSliding;
    private bool isWalljumping;

    public float jumpBufferTime = 0.1f;
    private float jumpBufferCounter;

    public float coyoteTime = 0.1f;
    private float coyoteTimer;

    //range means this variable can never be outside this range
    [Range(0f, 1f)]public float jumpCutMultiplier = 0.5f;
    public float fallMultiplier = 1.8f;
    public float lowJumpMultiplier = 2.5f;

    //ladder variables
    public float climbSpeed = 5f;
    public float ladderCheckRadius = 0.5f;
    public LayerMask LadderLayer;
    private float verticalInput;
    private bool isClimbing;


    private Rigidbody2D rb;
    private float moveInput;

    private MovingPlatform movingPlatform;

    public float crouchSpeedMultiplier = 0.4f;
    public float cellingCheckHeight = 0.475f;
    public float cellingCheckRadius = 0.2f;
    private bool isCrouching;
    private bool isCollidorCrouched;

    public Collider2D standingCollidor;
    public Collider2D crouchingCollidor;

    private Animator animator;

    private void Awake()
    {
     rb = GetComponent<Rigidbody2D>();
     trailRenderer = GetComponent<TrailRenderer>();
        defaultGravityScale = rb.gravityScale;
        standingCollidor = GetComponent<PolygonCollider2D>();
        crouchingCollidor = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
    }

    private void ReadInput()
    {
        Keyboard kb = Keyboard.current;

        moveInput = 0f;

        //left and right inputs
        if (kb.aKey.isPressed)
        {
            moveInput = -1f;
        }
        if (kb.dKey.isPressed)
        {
            moveInput = 1f;
        }

        //get direction for dsahing
        if(moveInput != 0f)
        {
            facingDirection = (int)moveInput;
        }

        //jump input
        if(kb.spaceKey.wasPressedThisFrame)
        {
            jumpBufferCounter = jumpBufferTime;
        }
        if(kb.spaceKey.wasReleasedThisFrame)
        {
            CutJump();
        }

        //dash input
        if(kb.leftShiftKey.wasPressedThisFrame && canDash)
        {
            StartCoroutine(Dash());
        }

        //ladder inputs
        verticalInput = 0f;
        if (kb.wKey.isPressed)
        {
            verticalInput = 1f;
        }
        if(kb.sKey.isPressed)
        {
            verticalInput = -1f;
        }
    }

    private void Update()
    {
        UpdateJumpTimers();
        ReadInput();
        Jump();
        UpdateAnimation();
    }

    private void FixedUpdate()
    {
        HandleCrouch();
        Move();
        WallSlide();
        ApplyBetterGravity();
        HandleLadder();
    }

    private void Jump()
    {
        //stops jumping while dash is happening
        if (isDashing || jumpBufferCounter <= 0f)
        {
            return;
        }

        //lets player jump off ladders
        if (isClimbing)
        {
            isClimbing = false;
            rb.gravityScale = defaultGravityScale;
            rb.linearVelocityY = jumpPower;
            jumpBufferCounter = 0f;
            return;
        }

        if (coyoteTimer > 0f)
        {
            rb.linearVelocityY = jumpPower;
            jumpBufferCounter = 0f;
            coyoteTimer = 0f;
        }
        //we want to jump off wall but no ground
        else if (isWallSliding)
        {
            StartCoroutine(wallJump());
            jumpBufferCounter = 0f;
        }
    }

    private bool isGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void Move()
    {
        //stops moevement while dashing or walljumping
        if (isDashing || isWalljumping)
        {
            return;
        }

        Vector2 movingPlatformSpeed = Vector2.zero;
        if(movingPlatform != null)
        {
            movingPlatformSpeed = movingPlatform.platformSpeed;
        }

        // ? makes if statement like "if : else"
        float crouchMoveSpeed = isCrouching ? crouchSpeedMultiplier : 1f;

        rb.linearVelocityX = crouchMoveSpeed * moveInput * speed + movingPlatformSpeed.x;
    }
       
    private IEnumerator Dash()
    {
        isDashing = true;
        canDash = false;
        rb.gravityScale = 0;
        rb.linearVelocity = new Vector2(dashSpeed * facingDirection, 0f);
        trailRenderer.emitting = true;

        yield return new WaitForSeconds(dashDuration);

        isDashing = false;
        rb.gravityScale = defaultGravityScale;
        rb.linearVelocityX *= 0.5f;
        trailRenderer.emitting = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    private bool isTouchingWall()
    {
        Vector2 checkPos = (Vector2)transform.position + Vector2.right * facingDirection * wallCheckDistance;
        return Physics2D.OverlapCircle(checkPos, groundCheckRadius, groundLayer);
    }
  
   private void WallSlide()
    {
        isWallSliding = isTouchingWall() && !isGrounded() && !isWalljumping && !isClimbing;

        if(isWallSliding)
        {
            rb.linearVelocityY = Mathf.Max(rb.linearVelocityY, -wallSlideSpeed);
        }

        trailRenderer.emitting = isWallSliding || isDashing;
    }

    private IEnumerator wallJump()
    {
        isWalljumping = true;
        facingDirection = -facingDirection;
        rb.linearVelocity = new Vector2(facingDirection * wallJumpForce.x, wallJumpForce.y);

        yield return new WaitForSeconds(wallJumpDuration);

        isWalljumping = false;
    }

    private void UpdateJumpTimers()
    {
        if (isGrounded() && rb.linearVelocityY <= 0.01f)
        {
            coyoteTimer = coyoteTime;
        }
        else
        {
            coyoteTimer = coyoteTimer -Time.deltaTime;
        }
            jumpBufferCounter = jumpBufferCounter - Time.deltaTime;
    }

    private void CutJump()

    {
        //checks if we are moving up - so probably jumping
        if(rb.linearVelocityY > 0f)
        {
            rb.linearVelocityY = rb.linearVelocityY * jumpCutMultiplier;
        }
    }

    private void ApplyBetterGravity()
    {
        if(isDashing || isWallSliding) 
            { return; }

        //if y < 0 we are falling
        if (rb.linearVelocityY < 0f)
        {
            rb.gravityScale = defaultGravityScale * fallMultiplier;
        }
        else if (rb.linearVelocityY > 0f && !Keyboard.current.spaceKey.isPressed)
        {
            rb.gravityScale = defaultGravityScale * lowJumpMultiplier;
        }
        else
        {
            rb.gravityScale = defaultGravityScale;
        }
    }

    private bool isOnLadder()
    {
        //creates a circle. If cirlce overlaps with ladder, it will return true, otherwise the method will return false
        return Physics2D.OverlapCircle(transform.position, ladderCheckRadius, LadderLayer);
    }

    private void HandleLadder()
    {
        bool onLadder = isOnLadder();

        //checks if player on ladder and moving
        if (onLadder && verticalInput != 0f)
        {
            isClimbing = true;
        }

       //ensures nothing under this will happen if not on ladder or dashing
        if (!onLadder)
        {
            rb.gravityScale = defaultGravityScale;
            isClimbing = false;
            return;
        }
        if(isDashing)
        {
            return;
        }

        if (isClimbing)
        {
            rb.gravityScale = 0f;
            rb.linearVelocityY = verticalInput * climbSpeed;
        }
        else
        {
            rb.gravityScale = defaultGravityScale;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.GetComponent<MovingPlatform>() != null)
        {
            movingPlatform = collision.gameObject.GetComponent<MovingPlatform>();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<MovingPlatform>() != null)
        {
            movingPlatform = null;
        }
    }

    private void HandleCrouch()
    {
        if(isClimbing)
        {
            return;
        }

        //checks if we're pressing the s key, as if we are, this will be true
        bool wantsToCrouch = verticalInput < 0f;

        //either if we pressing s key or stuck in a small hole already crouching
        isCrouching = wantsToCrouch || (isCrouching && HasCelling());

        SetCrouched(isCrouching);

    }



    private void SetCrouched(bool crouched)
    {
        if (crouched == isCollidorCrouched) return;
        isCollidorCrouched = crouched;

        if (crouched)
        {
            standingCollidor.enabled = false;
            crouchingCollidor.enabled = true;
        }
        else
        {
            standingCollidor.enabled = true;
            crouchingCollidor.enabled = false;
        }
    }

    private bool HasCelling()
    {
        Vector2 checkPos = (Vector2)transform.position + Vector2.up * cellingCheckHeight;
        //creates circle and checks if touching celling of a ground layer object
        return Physics2D.OverlapCircle(checkPos, cellingCheckRadius, groundLayer);
    }

    void UpdateAnimation()
    {
        if (!isGrounded())
        {
            if (isClimbing)
            {
                animator.Play("playerClimb");
            }
            else
            {
                animator.Play("playerJump");
            }
        }

        else if (moveInput != 0f)
        {
            if (isCrouching)
            {
                animator.Play("playerCrouch");
            }

            else
            {
                animator.Play("playerWalk");
            }
        }

        else if(isCrouching)
        {
            animator.Play("playerCrouch");
        }
       
      


        else
        {
            animator.Play("playerIdle");
        }
    }

}


