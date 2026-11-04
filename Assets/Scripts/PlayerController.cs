using System;
using UnityEngine;
using UnityEngine.InputSystem;



public class PlayerController : MonoBehaviour
{
    //intialise variables
    public float speed = 1.0f;
    public float jumpForce = 10f;
    private Rigidbody2D rb;
    private Boolean onGround = true;
    private float moveInput;



    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
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
        if (kb.aKey.isPressed)
        {
            moveInput = -1f;
        }
        if (kb.dKey.isPressed)
        {
            moveInput = 1f;
        }

        //check for jump 
        if (kb.spaceKey.wasPressedThisFrame && onGround)
        {
            onGround = false;
            Jump();
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


}
