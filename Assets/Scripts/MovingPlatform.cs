using UnityEngine;

//means whatever object this script is added to must also have a rigidbody 2D component 
[RequireComponent(typeof(Rigidbody2D))]

public class MovingPlatform : MonoBehaviour
{
    //adds two variables of same type in one line
    public Transform pointA, pointB;
    public float speed = 2f;

    private Rigidbody2D rb;
    private Vector2 target;

    public Vector2 platformSpeed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        target = pointB.position;
    }

    private void FixedUpdate()
    {
        //next will be next posiiton in the next frame
        Vector2 next = Vector2.MoveTowards(rb.position, target, speed*Time.deltaTime);

        //gives platform velocity to share with player Controller script
        platformSpeed = (next - rb.position) / Time.fixedDeltaTime;

        rb.MovePosition(next);

        // here we check if our target is equal to the position we are aiming for, and if it is we switch our targets around
        if (Vector2.Distance(rb.position, target) < 0.05f)
        {
            //this code is called a ternary operator - basically just an if statment in one line
            target = (target == (Vector2)pointA.position) ? pointB.position : pointA.position;
        }    
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //lets player stick to moving platform
        if(collision.gameObject.tag == "Player")
        {
            collision.gameObject.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        //lets player unstick from moving platform
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.transform.SetParent(null);
        }
    }
}
   
