using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float speed = 1.0f;
    public float jumpForce = 1.0f;
    Rigidbody2D rb;
    private Boolean onGround = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
 
  
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            {
            
            rb.AddForce(-transform.right * speed);
        }
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            
            rb.AddForce(transform.right * speed);
        }
        if (Input.GetKey(KeyCode.Space) && onGround)
            {
                
                rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
            onGround = false;
            }
        if (Input.GetKey(KeyCode.LeftControl))
        {
            Debug.Log("L ctrl pressed");
            
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        onGround = true;
    }
}
