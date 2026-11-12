using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;




public class PlayerController : MonoBehaviour
{

    //intialise variables

    public float speed = 8f;

    public float jumpPower = 14f;

    public float dashSpeed = 20f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 0.5f;

    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private float moveInput;
    private TrailRenderer trailRenderer;
    private int facingDirection = 1;
    private float defaultGravityScale;
    private bool isDashing;
    private bool canDash = true;

    private void Awake()
    {
     rb = GetComponent<Rigidbody2D>();
     trailRenderer = GetComponent<TrailRenderer>();
        defaultGravityScale = rb.gravityScale;
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
            Jump();
        }

        //dash input
        if(kb.leftShiftKey.wasPressedThisFrame && canDash)
        {
            StartCoroutine(Dash());
        }
    }

    private void Update()
    {
        ReadInput();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Jump()
    {
        //stops jumping while dash is happening
        if (isDashing)
        {
            return;
        }
        if (isGrounded())
        {
            rb.linearVelocityY = jumpPower;
        }
    }

    private bool isGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void Move()
    {
        //stops moevement while dashing
        if (isDashing)
        {
            return;
        }
        rb.linearVelocityX = moveInput * speed;
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


  
   

}
