using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Reflection.Metadata.Ecma335;



public class PlayerController : MonoBehaviour
{
    //intialise variables

    //variables for jumping and moving
    public float speed = 1.0f;
    public float jumpForce = 10f;
    private Rigidbody2D rb;
    private Boolean onGround = true;
    private float moveInput;

    //varibales for dashing
    public float dashSpeed = 20f;
    public float dashLength = 0.2f;
    public float dashCD = 0.5f;

    private TrailRenderer trailRenderer;
    private int facingDirection = 1;
    private float defaultGravityScale;
    private bool isDashing;
    private bool canDash = true;


        


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        trailRenderer = rb.GetComponent<TrailRenderer>();

        defaultGravityScale = rb.gravityScale;
    }



    private void Jump()
    {
        rb.linearVelocityY = jumpForce;
    }

        void Start()
        {



        }


        void Update()
        {
        //gets the keyboard keys currenty in use
        Keyboard kb = Keyboard.current;

        //checking for move
        moveInput = 0f;

        if (isDashing)
        {
            //stops player doing other things while dashing
            return;
        }
        if (kb.aKey.isPressed)
        {
            moveInput = -1f;
        }
        if (kb.dKey.isPressed)
        {
            moveInput = 1f;
        }
        if(moveInput != 0f)

        {
            facingDirection = (int)moveInput;
        }
        //check for jump 
        if (kb.spaceKey.wasPressedThisFrame && onGround)
        {
            onGround = false;
            Jump();
        }
        if(kb.leftShiftKey.wasPressedThisFrame &&canDash)
        {
            StartCoroutine(Dash());
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocityX = moveInput * speed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        onGround = true;
    }

    private IEnumerator Dash()
    {
        isDashing = true;
        canDash = false;
        rb.gravityScale = 0;
        rb.linearVelocityX = facingDirection * dashSpeed;
        trailRenderer.emitting = true;

        yield return new WaitForSeconds(dashLength);

        isDashing = false;
        rb.gravityScale = defaultGravityScale;
        rb.linearVelocityX *= 0.5f;
        trailRenderer.emitting = false;

        yield return new WaitForSeconds(dashCD);
    }

}
